using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PortKit.Extensions
{
    [DebuggerDisplay("{_member.Name}")]
    public sealed class BindingLink : IDisposable
    {
        private readonly MemberInfo _member;
        private readonly Action _callback;
        private object _instance;
        private bool _subscribe;

        public BindingLink Next { get; set; }

        public BindingLink(MemberInfo member, Action callback)
        {
            _member = member;
            _callback = callback;
        }

        public static BindingLink[] FromExpression<TProperty>(
            Expression<Func<TProperty>> expression,
            object instance,
            Action callback)
        {
            var members = MemberUtils.GetMembers(expression, instance);
            if (!members.Any())
            {
                return Array.Empty<BindingLink>();
            }

            var current = new BindingLink(members.Pop(), callback);
            var list = new List<BindingLink> {current};

            foreach (var member in members)
            {
                current.Next = new BindingLink(member, callback);
                current = current.Next;
                list.Add(current);
            }

            return list.ToArray();
        }

        public void Dispose()
        {
            Unsubscribe();
            Next?.Dispose();
        }

        public BindingLink Bind(object instance)
        {
            Unsubscribe();
            _instance = instance;
            return this;
        }

        public void Evaluate(bool subscribe)
        {
            _subscribe = subscribe;
            if (_subscribe)
            {
                Subscribe();
            }

            EvaluateNext();
        }

        public bool TryGetValue(out object value) =>
            MemberUtils.TryGetValue(_instance, _member, out value);

        public void SetValue(object value) =>
            MemberUtils.SetValue(_instance, _member, value);

        private void EvaluateNext()
        {
            Next?.Unsubscribe();

            if (!TryGetValue(out var value))
            {
                return;
            }

            if (_subscribe)
            {
                if (value is INotifyCollectionChanged ncc)
                {
                    void OnValueChanged(object s, NotifyCollectionChangedEventArgs e) =>
                        _callback?.Invoke();

                    ncc.CollectionChanged -= OnValueChanged;
                    ncc.CollectionChanged += OnValueChanged;
                }
            }

            Next?.Bind(value).Evaluate(_subscribe);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_member.Name != e.PropertyName)
            {
                return;
            }

            _callback?.Invoke();
            EvaluateNext();
        }

        private void Subscribe()
        {
            if (_instance is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += OnPropertyChanged;
            }
        }

        private void Unsubscribe()
        {
            if (_instance is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged -= OnPropertyChanged;
            }
        }
    }
}
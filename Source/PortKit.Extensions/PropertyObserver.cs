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
    public sealed class PropertyObserver : IDisposable
    {
        private const BindingFlags MemberAccessFlags = BindingFlags.Default |
                                                       BindingFlags.Instance |
                                                       BindingFlags.NonPublic |
                                                       BindingFlags.Public;

        private readonly MemberInfo _member;
        private readonly Action _callback;
        private object _instance;
        private bool _subscribe;

        public PropertyObserver Next { get; set; }

        public PropertyObserver(MemberInfo member, Action callback)
        {
            _member = member;
            _callback = callback;
        }

        public static PropertyObserver[] FromExpression<T>(Expression<Func<T>> expression, object instance, Action callback)
        {
            var members = expression.GetMembers(instance);
            if (!members.Any())
            {
                return Array.Empty<PropertyObserver>();
            }

            var current = new PropertyObserver(members.Pop(), callback);
            var list = new List<PropertyObserver> {current};

            foreach (var member in members)
            {
                current.Next = new PropertyObserver(member, callback);
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

        public PropertyObserver Bind(object instance)
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

        public bool TryGetValue(out object value)
        {
            value = default;
            if (_instance == null)
            {
                return false;
            }

            var instanceType = _instance.GetType();

            switch (_member.MemberType)
            {
                case MemberTypes.Field:
                    var field = instanceType.GetField(_member.Name, MemberAccessFlags);
                    if (field == null)
                    {
                        return false;
                    }

                    value = field.GetValue(_instance);
                    return true;

                case MemberTypes.Property:
                    var property = instanceType.GetProperty(_member.Name, MemberAccessFlags);
                    if (property == null)
                    {
                        return false;
                    }

                    value = property.GetValue(_instance);
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_member));
            }
        }

        public void SetValue(object value)
        {
            switch (_member)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(_instance, value);
                    break;

                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(_instance, value);
                    break;
            }
        }

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
                    void OnValueChanged(object s, NotifyCollectionChangedEventArgs e)
                    {
                        _callback?.Invoke();
                    }

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
using System;
using System.Linq;
using System.Linq.Expressions;
using PortKit.Extensions;

namespace PortKit.Bindings
{
    public sealed class Binding<TSource, TTarget> : IDisposable
    {
        private readonly BindingLink _sourceLink;
        private readonly BindingLink _targetLink;
        private readonly Func<TSource, TTarget> _convert;
        private readonly Func<TTarget, TSource> _convertBack;
        private Action<TSource> _sourceChangedCallback;
        private Action<TTarget> _targetChangedCallback;
        private TSource _fallbackValue;

        public Binding(
            object source,
            Expression<Func<TSource>> sourceExpression,
            object target,
            Expression<Func<TTarget>> targetExpression,
            BindingMode mode,
            Func<TSource, TTarget> convert = default,
            Func<TTarget, TSource> convertBack = default)
        {
            _convert = convert ?? (x => (TTarget) Convert.ChangeType(x, typeof(TTarget)));
            _convertBack = convertBack ?? (x => (TSource) Convert.ChangeType(x, typeof(TSource)));

            var sourceNodes = BindingLink.FromExpression(sourceExpression, source, UpdateTargetValue);
            var targetNodes = BindingLink.FromExpression(targetExpression, target ?? source, UpdateSourceValue);

            _sourceLink = sourceNodes.Last();
            _targetLink = targetNodes.LastOrDefault();

            var sourceRoot = sourceNodes.First();
            sourceRoot.Bind(source);

            var targetRoot = targetNodes.FirstOrDefault();
            targetRoot?.Bind(target ?? source);

            Evaluate(mode, sourceRoot, targetRoot);
        }

        public void Dispose()
        {
            _sourceLink?.Dispose();
            _targetLink?.Dispose();
        }

        public Binding<TSource, TTarget> WithFallback(TSource fallbackValue)
        {
            _fallbackValue = fallbackValue;
            UpdateTargetValue();

            return this;
        }

        public Binding<TSource, TTarget> OnSourceChanged(Action<TSource> onSourceChanged)
        {
            _sourceChangedCallback = onSourceChanged;

            return this;
        }

        public Binding<TSource, TTarget> OnTargetChanged(Action<TTarget> onTargetChanged)
        {
            _targetChangedCallback = onTargetChanged;

            return this;
        }

        private void Evaluate(BindingMode mode, BindingLink sourceRoot, BindingLink targetRoot)
        {
            switch (mode)
            {
                case BindingMode.OneTime:
                    sourceRoot.Evaluate(false);
                    targetRoot?.Evaluate(false);
                    UpdateTargetValue();
                    break;

                case BindingMode.OneWay:
                    sourceRoot.Evaluate(true);
                    targetRoot?.Evaluate(false);
                    UpdateTargetValue();
                    break;

                case BindingMode.OneWayToSource:
                    if (targetRoot == null)
                    {
                        throw new InvalidOperationException("Target root member cannot be null.");
                    }

                    sourceRoot.Evaluate(false);
                    targetRoot.Evaluate(true);
                    UpdateSourceValue();
                    break;

                case BindingMode.TwoWay:
                    if (targetRoot == null)
                    {
                        throw new InvalidOperationException("Target root member cannot be null.");
                    }

                    sourceRoot.Evaluate(true);
                    targetRoot.Evaluate(true);
                    UpdateTargetValue();
                    UpdateSourceValue();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private void UpdateTargetValue()
        {
            if (!_sourceLink.TryGetValue(out var value))
            {
                value = _fallbackValue;
            }

            _targetLink?.SetValue(_convert((TSource) value));
            _sourceChangedCallback?.Invoke((TSource) value);
        }

        private void UpdateSourceValue()
        {
            if (!_targetLink.TryGetValue(out var value))
            {
                return;
            }

            _sourceLink?.SetValue(_convertBack((TTarget) value));
            _targetChangedCallback?.Invoke((TTarget) value);
        }
    }
}
using System;
using System.Linq;
using System.Linq.Expressions;
using PortKit.Extensions;

namespace PortKit.Bindings
{
    public sealed class Binding<TSource, TTarget> : IDisposable
    {
        private readonly PropertyObserver _sourceObserver;
        private readonly PropertyObserver _targetObserver;
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
            _convert = convert ?? (x => (TTarget)Convert.ChangeType(x, typeof(TTarget)));
            _convertBack = convertBack ?? (x => (TSource)Convert.ChangeType(x, typeof(TSource)));

            var sourceObservers = PropertyObserver.FromExpression(sourceExpression, source, UpdateTargetValue);
            var targetObservers = PropertyObserver.FromExpression(targetExpression, target ?? source, UpdateSourceValue);

            _sourceObserver = sourceObservers.Last();
            _targetObserver = targetObservers.LastOrDefault();

            var sourceRoot = sourceObservers.First();
            sourceRoot.Bind(source);

            var targetRoot = targetObservers.FirstOrDefault();
            targetRoot?.Bind(target ?? source);

            Evaluate(mode, sourceRoot, targetRoot);
        }

        public void Dispose()
        {
            _sourceObserver?.Dispose();
            _targetObserver?.Dispose();
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
            if (_sourceObserver.TryGetValue(out var value))
            {
                _sourceChangedCallback?.Invoke((TSource)value);
            }

            return this;
        }

        public Binding<TSource, TTarget> OnTargetChanged(Action<TTarget> onTargetChanged)
        {
            _targetChangedCallback = onTargetChanged;
            if (_targetObserver.TryGetValue(out var value))
            {
                _targetChangedCallback?.Invoke((TTarget)value);
            }

            return this;
        }

        private void Evaluate(BindingMode mode, PropertyObserver sourceRoot, PropertyObserver targetRoot)
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
            if (!_sourceObserver.TryGetValue(out var value))
            {
                value = _fallbackValue;
            }

            _targetObserver?.SetValue(_convert((TSource)value));
            _sourceChangedCallback?.Invoke((TSource)value);
        }

        private void UpdateSourceValue()
        {
            if (!_targetObserver.TryGetValue(out var value))
            {
                return;
            }

            _sourceObserver?.SetValue(_convertBack((TTarget)value));
            _targetChangedCallback?.Invoke((TTarget)value);
        }
    }
}
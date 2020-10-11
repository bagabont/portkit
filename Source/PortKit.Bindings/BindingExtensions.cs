using System;
using System.Linq.Expressions;

namespace PortKit.Bindings
{
    public static class BindingExtensions
    {
        public static Binding<TSource, TTarget> Set<TSource, TTarget>(
            this object source,
            Expression<Func<TSource>> sourceExpression,
            object target,
            Expression<Func<TTarget>> targetExpression,
            BindingMode mode = BindingMode.OneWay,
            Func<TSource, TTarget> converter = default,
            Func<TTarget, TSource> convertBack = default)
        {
            return new Binding<TSource, TTarget>(source, sourceExpression, target, targetExpression, mode, converter, convertBack);
        }

        public static Binding<TSource, TSource> Set<TSource>(
            this object source,
            Expression<Func<TSource>> sourceExpression)
        {
            return new Binding<TSource, TSource>(source, sourceExpression, default, default, BindingMode.OneWay);
        }

        public static Binding<TSource, TTarget> Set<TSource, TTarget>(
            this object source,
            Expression<Func<TSource>> sourceExpression,
            Expression<Func<TTarget>> targetExpression,
            BindingMode mode = BindingMode.OneWay,
            Func<TSource, TTarget> converter = default,
            Func<TTarget, TSource> convertBack = default)
        {
            var target = source;

            return new Binding<TSource, TTarget>(source, sourceExpression, target, targetExpression, mode, converter, convertBack);
        }

        public static Binding<TSource, TTarget> Set<TSource, TTarget>(
            this object source,
            Expression<Func<TSource>> sourceExpression,
            Expression<Func<TTarget>> targetExpression,
            Func<TSource, TTarget> converter = default)
        {
            return source.Set(sourceExpression, targetExpression, BindingMode.OneWay, converter);
        }
    }
}
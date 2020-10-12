using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PortKit.Extensions
{
    public static class ExpressionExtensions
    {
        private const BindingFlags MemberFlags = BindingFlags.Default |
                                                 BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public;

        private sealed class Visitor : ExpressionVisitor
        {
            public Stack<MemberInfo> Path { get; } = new Stack<MemberInfo>();

            protected override Expression VisitMember(MemberExpression node)
            {
                switch (node.Member)
                {
                    case FieldInfo _:
                    case PropertyInfo _:
                        Path.Push(node.Member);

                        return base.VisitMember(node);

                    default:
                        throw new ArgumentException("The expression path can only contain properties and fields.",
                            nameof(node));
                }
            }
        }

        public static object GetCaller<T>(this Expression<Func<T>> expression)
        {
            if (!(expression.Body is MemberExpression propertyAccessExpression))
            {
                return null;
            }

            while (propertyAccessExpression.Expression is MemberExpression memberExpression)
            {
                propertyAccessExpression = memberExpression;
            }

            if (!(propertyAccessExpression.Expression is ConstantExpression rootObjectConstantExpression))
            {
                return null;
            }

            return rootObjectConstantExpression.Value;
        }

        public static Stack<MemberInfo> GetMembers<T>(this Expression<Func<T>> expression, object instance)
        {
            var visitor = new Visitor();

            visitor.Visit(expression?.Body);

            var instanceType = instance.GetType();

            var found = false;
            while (visitor.Path.Any() && !found)
            {
                var member = visitor.Path.Peek();

                found = member.MemberType switch
                {
                    MemberTypes.Field => instanceType.GetField(member.Name, MemberFlags) != null,
                    MemberTypes.Property => instanceType.GetProperty(member.Name, MemberFlags) != null,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (!found)
                {
                    visitor.Path.Pop();
                }
            }

            return visitor.Path;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PortKit.Bindings
{
    internal static class MemberUtils
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

        public static Stack<MemberInfo> GetMembers<TProperty>(Expression<Func<TProperty>> expression, object instance)
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

        public static bool TryGetValue(object instance, MemberInfo memberInfo, out object value)
        {
            value = default;
            if (instance == null)
            {
                return false;
            }

            var instanceType = instance.GetType();

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var field = instanceType.GetField(memberInfo.Name, MemberFlags);
                    if (field == null)
                    {
                        return false;
                    }

                    value = field.GetValue(instance);
                    return true;

                case MemberTypes.Property:
                    var property = instanceType.GetProperty(memberInfo.Name, MemberFlags);
                    if (property == null)
                    {
                        return false;
                    }

                    value = property.GetValue(instance);
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo));
            }
        }

        public static void SetValue(object instance, MemberInfo memberInfo, object value)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(instance, value);
                    break;

                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(instance, value);
                    break;
            }
        }
    }
}
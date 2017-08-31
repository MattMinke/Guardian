using System;
using System.Reflection;

namespace Guardian.Internal
{
    public class ReflectiveGetterFactory : IGetterFactory
    {
        public Func<object, object> Create(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    var field = member as FieldInfo;
                    return (target) => field.GetValue(target);
                case MemberTypes.Property:
                    var property = member as PropertyInfo;
                    return (target) => property.GetValue(target);
                default:
                    throw new NotSupportedException($"Member of type {member.MemberType} is not supported");
            }
        }
    }
}

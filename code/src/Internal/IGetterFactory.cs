using System;
using System.Reflection;

namespace Guardian.Internal
{
    public interface IGetterFactory
    {
        Func<object, object> Create(MemberInfo member);
    }
}

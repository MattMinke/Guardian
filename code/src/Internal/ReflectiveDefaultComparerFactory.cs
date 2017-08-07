using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Guardian.Internal
{
    public class ReflectiveDefaultComparerFactory : IComparerFactory
    {
        private readonly ConcurrentDictionary<Type, IComparer> cache = 
            new ConcurrentDictionary<Type, IComparer>();

        public IComparer GetComparer(Type type)
        {
            return cache.GetOrAdd(type, Build);

        }

        private static IComparer Build(Type type)
        {
            return typeof(Comparer<>).MakeGenericType(type)
                .GetProperty("Default")
                .GetValue(null) as IComparer;
        }
    }
}

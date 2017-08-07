using System;
using System.Collections;
using System.Text;

namespace Guardian.Internal
{
    public interface IComparerFactory
    {
        IComparer GetComparer(Type type);
    }
}

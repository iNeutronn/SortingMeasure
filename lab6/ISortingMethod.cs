using System;
using System.Collections.Generic;
using System.Threading;

namespace lab6
{
    internal interface ISortingMethod
    {
        string Name { get; }
        void Sort<T>(IList<T> list, CancellationTokenSource cts) where T : IComparable<T>;

    }
}

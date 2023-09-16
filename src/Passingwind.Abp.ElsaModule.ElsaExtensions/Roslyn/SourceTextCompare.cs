using System;

namespace Passingwind.Abp.ElsaModule.Roslyn;

public class SourceTextCompare : IComparable<string>, IComparable
{
    public int CompareTo(string other)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            return 1;
        }

        if (obj is string x)
        {
            return CompareTo(x);
        }

        throw new ArgumentException("", nameof(obj));
    }
}

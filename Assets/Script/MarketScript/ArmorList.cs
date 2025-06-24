using System;
using System.Collections.Generic;

[Serializable]
public class ArmorList
{
    public List<string> armors;

    public HashSet<string> ToHashSet()
    {
        return new HashSet<string>(armors ?? new List<string>());
    }
}
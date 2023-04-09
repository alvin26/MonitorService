using System;
using System.Collections.Concurrent;

namespace IndoCash.MonitorService.Utils
{
    public class NamedConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public string Name { get; }

        public NamedConcurrentDictionary(string name)
        {
            Name = name;
        }
    }
}

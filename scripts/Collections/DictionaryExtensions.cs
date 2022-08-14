using System.Collections.Generic;

namespace pdxpartyparrot.ssjAug2022.Collections
{
    public static class DictionaryExtensions
    {
        public static TV GetOrAdd<TK, TV>(this Dictionary<TK, TV> dict, TK key) where TV : new()
        {
            if(dict.TryGetValue(key, out var value)) {
                return value;
            }

            value = new TV();
            dict.Add(key, value);
            return value;
        }
    }
}

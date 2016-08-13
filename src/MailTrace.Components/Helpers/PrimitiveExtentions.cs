namespace MailTrace.Components.Helpers
{
    using System.Collections.Generic;

    public static class PrimitiveExtentions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue obj;
            dictionary.TryGetValue(key, out obj);
            return obj;
        }
    }
}
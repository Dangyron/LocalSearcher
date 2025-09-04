using System.Numerics;
using System.Runtime.InteropServices;

namespace LocalSearcher.Api.Utils;

public static class DictionaryExtensions
{
    public static void AddOrIncrement<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue initial)
        where TValue : INumber<TValue> where TKey : notnull
    {
        ref var count = ref CollectionsMarshal.GetValueRefOrAddDefault(
            dictionary, 
            key, 
            out var exists
        );
            
        count = exists ? count! + TValue.One : initial;
    }
    
    public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue initial, Func<TKey, TValue, TValue> update)
        where TValue : INumber<TValue> where TKey : notnull
    {
        ref var number = ref CollectionsMarshal.GetValueRefOrAddDefault(
            dictionary, 
            key, 
            out var exists
        );
        
        number = exists ? update(key, number!) : initial;
    }
}
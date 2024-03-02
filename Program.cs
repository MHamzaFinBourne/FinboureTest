using System;
using System.Collections.Generic;

public class FInbourneCache<T>
{
    private readonly int _maxItems;
    private readonly Dictionary<string, CacheItem<T>> _cache = new Dictionary<string, CacheItem<T>>();

    public FInbourneCache(int maxItems)
    {
        _maxItems = maxItems;
    }

    public void AddOrDelete(string key, T value)
    {
        // Check if the cache is full
        if (_cache.Count >= _maxItems)
        {
            DeleteOldestValue();
        }

        // Add or update the item
        if (_cache.ContainsKey(key))
        {
            _cache[key].Value = value;
            _cache[key].OldestValue = DateTime.Now;
        }
        else
        {
            _cache[key] = new CacheItem<T>(value);
        }
    }

    public bool GetSpecificItem(string key, out T value)
    {
        if (_cache.TryGetValue(key, out CacheItem<T> item))
        {
            item.OldestValue = DateTime.Now;
            value = item.Value;
            return true;
        }

        value = default;
        return false;
    }

    private void DeleteOldestValue()
    {
        var oldestItem = default(KeyValuePair<string, CacheItem<T>>);
        foreach (var c in _cache)
        {
            if (oldestItem.Value == null || c.Value.OldestValue < oldestItem.Value.OldestValue)
            {
                oldestItem = c;
            }
        }

        if (oldestItem.Key != null)
        {
            _cache.Remove(oldestItem.Key);
        }
    }

    private class CacheItem<TValue>
    {
        public TValue Value { get; set; }
        public DateTime OldestValue { get; set; } = DateTime.Now;

        public CacheItem(TValue value)
        {
            Value = value;
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        // Create an instance of the cache with a maximum of 3 items
        var cache = new FInbourneCache<string>(3);

        // Add items to the cache
        cache.AddOrDelete("1", "Item 1");
        cache.AddOrDelete("2", "Item 2");
        cache.AddOrDelete("3", "Item 3");

        // Retrieve an item from the cache
        if (cache.GetSpecificItem("1", out string value))
        {
            Console.WriteLine($"Value retrieved: {value}");
        }

        // Add a new item, causing eviction of the least recently used item
        cache.AddOrDelete("4", "Item 4");

        // Attempt to retrieve the evicted item (should fail)
        if (!cache.GetSpecificItem("2", out value))
        {
            Console.WriteLine("Item 2 not found");
        }

        //Console.ReadLine();
    }
}
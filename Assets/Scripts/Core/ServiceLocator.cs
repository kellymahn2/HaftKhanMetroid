using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    // Each value is a HashSet<T> for the matching key type T
    private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() => Services.Clear();

    public static void Register<T>(T obj) where T : class
    {
        if (obj == null) return;

        if (!Services.TryGetValue(typeof(T), out object value))
        {
            value = new HashSet<T>();
            Services[typeof(T)] = value;
        }

        // Add already ignores duplicates, no Contains check needed
        ((HashSet<T>)value).Add(obj);
    }

    public static IReadOnlyCollection<T> Get<T>() where T : class
    {
        if (Services.TryGetValue(typeof(T), out object value))
            return (HashSet<T>)value;

        return Array.Empty<T>();
    }

    public static T GetFirst<T>() where T : class
    {
        if (Services.TryGetValue(typeof(T), out object value))
        {
            // Iterating the concrete HashSet<T> avoids boxing the enumerator
            foreach (T item in (HashSet<T>)value)
                return item;
        }

        return null;
    }

    public static void Unregister<T>(T obj) where T : class
    {
        if (obj == null) return;
        if (!Services.TryGetValue(typeof(T), out object value)) return;

        var set = (HashSet<T>)value;
        set.Remove(obj);

        if (set.Count == 0)
            Services.Remove(typeof(T));
    }
}
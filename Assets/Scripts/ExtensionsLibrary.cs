using System;
using System.Collections.Generic;
using System.Linq;

public static class ExtensionsLibrary
{

    #region IEnumerable

    public static T GetRandom<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) throw new InvalidOperationException("Collection is null.");

        IList<T> list = enumerable as IList<T> ?? enumerable.ToList();

        if (list.Count == 0) throw new InvalidOperationException("Collection is empty.");

        Random random = new Random();

        return list[random.Next(0, list.Count - 1)];
    }

    //You can use the Action<T> delegate to pass a method as a parameter
    //without explicitly declaring a custom delegate.
    //The encapsulated method must correspond to the method signature that is defined by this delegate.
    //This means that the encapsulated method must have one parameter that is passed to it by value,
    //and it must not return a value.
    //In this case the action needs to take in same type as T!
    public static void EachDoAction<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        if (enumerable == null || action == null) throw new InvalidOperationException("Collection or action is null.");

        IList<T> list = enumerable as IList<T> ?? enumerable.ToList();

        if (list.Count == 0) throw new InvalidOperationException("Collection is empty.");

        foreach (var item in list) action(item);
    }

    #endregion
}

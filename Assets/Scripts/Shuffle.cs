using System.Collections.Generic;

public static class Shuffle
{
    static readonly System.Random _random = new System.Random();
    /// Used in Shuffle(T).
    /// </summary>
    public static void ListShuffle<T>(ref List<T> list)
    {
        if (list.Count == 0 || list == null)
            return;
        for (int i = 0; i < list.Count - 1; i++)
        {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            int r = i + _random.Next(list.Count - i);
            T t = list[r];
            list[r] = list[i];
            list[i] = t;
        }
    }
    public static void ArrayShuffle<T>(ref T[] array)
    {
        if (array.Length == 0)
            return;
        for (int i = 0; i < array.Length - 1; i++)
        {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            int r = i + _random.Next(array.Length - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }
}

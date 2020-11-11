using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    /// <summary>
    /// リストの中のランダムな要素を１つ返します
    /// </summary>
    public static T GetRandom<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// リストの要素をランダムに並び替えた新しいリストを返します
    /// </summary>
    public static List<T> GetShuffledList<T>(this List<T> list) { 
        return list.OrderBy(a => Guid.NewGuid()).ToList();
    }
}
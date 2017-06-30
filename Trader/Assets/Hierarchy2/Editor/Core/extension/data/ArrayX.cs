using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ArrayX {
    /*public static string xJoin(this IList list, string separator = ",", string prefix = null, string suffix = null) {
        var builder = new StringBuilder();
        var hasSep = !string.IsNullOrEmpty(separator);

        if (prefix != null) builder.Append(prefix);
        for (var i = 0; i < list.Count; i++) {
            if (i > 0 && hasSep) builder.Append(separator);
            builder.Append(list[i]);
        }
        return builder.ToString();
    }*/

    public static T[] xToArray<T>(IList list) {
        var result = new T[list.Count];
        for (var i = 0; i < list.Count; i++) {
            result[i] = (T) list[i];
        }
        return result;
    }

    //public static int xIndexOf(this IList list, object item, int stIndex = 0) {
    //    for (var i = stIndex; i < list.Count; i++) {
    //        if (list[i].Equals(item)) return i;
    //    }
    //    return -1;
    //}

    // Array Operations :: 
    public static T[] xAdd<T>(this T[] list, T item, bool checkUnique = false) {
        var tail = new[] {item};
        var result = checkUnique ? list.Union(tail) : list.Concat(tail);
        return result.ToArray();
    }

    public static T[] xAddRange<T>(this T[] list, IList items, bool checkUnique = false) {
        var arr = new List<T>();
        if (items == null || items.Count == 0) return list;
        if (list != null && list.Length > 0) arr.AddRange(list);

        for (var i = 0; i < items.Count; i++) {
            var item = (T) items[i];
            if (checkUnique && arr.Contains(item)) continue;
            arr.Add(item);
        }

        return arr.ToArray<T>();
    }

    public static T[] xRemoveAt<T>(this T[] source, int index) {
        if (index < 0 || index > source.Length - 1) return source;
        var dest = new T[source.Length - 1];
        Array.Copy(source, 0, dest, 0, index);
        Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
        return dest;
    }

    public static T[] xRemove<T>(this T[] source, T item) {
        if (!source.Contains(item)) return source;
        var list = source.ToList();
        list.Remove(item);
        return list.ToArray();
    }

    public static T[] xCompact<T>(this T[] list, T defaultValue = default(T)) {
        return list.ToList()
            .FindAll(item => !item.Equals(defaultValue))
            .ToArray();
    }

    public static T[] xDuplicateToArray<T>(this T item, int nItems) {
        var result = new T[nItems];
        for (var i = 0; i < nItems; i++) {
            result[i] = item;
        }
        return result;
    }

    public static T[] xSwap<T>(this T[] list, int idx1, int idx2) {
        T tmp = list[idx1];
        list[idx1] = list[idx2];
        list[idx2] = tmp;
        return list;
    }
     
    /*public static string xJoin<T>(this T[] source, string delimiter = ",") {
        string result = "";
        for (int i = 0; i < source.Length; i++) {
            result += (i == 0 ? "" : delimiter) + source[i];
        }
        return result;
    }*/

    public static Array xToArrayT(this IList content, Type itemType) {
        var result = Array.CreateInstance(itemType, content.Count);
        for (var i = 0; i < content.Count; i++) {
            result.SetValue(content[i], i);
        }
        return result;
    }


    // List extensions
    public static List<T> xClone<T>(this List<T> source) {
        var list = new List<T>();
        list.AddRange(source);
        return list;
    }

    public static T xShift<T>(this List<T> source) {
        T value = source[0];
        source.RemoveAt(0);
        return value;
    }

    public static Array xNewArrayT(this Type elmType, int count) { return Array.CreateInstance(elmType, count); }

    public static Array xToArrayT(this Array arr, Type elmType) {
        if (elmType == null) {
            elmType = arr.GetValue(0)
                .GetType();
        }
        var result = xNewArrayT(elmType, arr.Length);
        for (var i = 0; i < arr.Length; i++) {
            result.SetValue(arr.GetValue(i), i);
        }
        return result;
    }

    public static IList xNewListT(this Type elmType) {
        var listType = typeof (List<>);
        var combinedType = listType.MakeGenericType(elmType);
        return (IList) Activator.CreateInstance(combinedType);
    }

    public static IList xToListT(this IList list, Type elmType = null) {
        if (elmType == null) elmType = list[0].GetType();
        var result = xNewListT(elmType);
        foreach (object item in list) {
            result.Add(item);
        }
        return result;
    }
}
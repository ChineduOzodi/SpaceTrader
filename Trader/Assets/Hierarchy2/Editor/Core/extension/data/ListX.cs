using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class ListX {
    /*
		NOTES

		- Dictionary<TKey, TValue> :
			IDictionary<TKey, TValue>, 
			ICollection<KeyValuePair<TKey, TValue>>,
			IDictionary,
			ICollection, 
			IReadOnlyDictionary<TKey, TValue>,
			IReadOnlyCollection<KeyValuePair<TKey, TValue>>, 
			IEnumerable<KeyValuePair<TKey, TValue>>,
			IEnumerable,
			ISerializable, 
			IDeserializationCallback


		- List<T> :
			IList<T>,
			ICollection<T>, 
			IList,
			ICollection,
			IReadOnlyList<T>,
			IReadOnlyCollection<T>,
			IEnumerable<T>, 
			IEnumerable


		- T[]	: 
			ICloneable
			IList
			ICollection
			IEnumerable
			IStructuralComparable
			IStructuralEquatable
			------------------------
			IList<T>
			ICollection<T>
			IEnumerable<T>
			IReadOnlyList<T>
			IReadOnlyCollection<T>

	*/

    public static string xJoin(this ICollection list, string separator = ",", string prefix = null, string suffix = null) {
        if (list == null || list.Count == 0) return string.Empty;

        var builder = new StringBuilder();
        var hasSep = !string.IsNullOrEmpty(separator);
        if (prefix != null) builder.Append(prefix);

        var isFirst = true;
        foreach (var item in list) {
            if (hasSep) {
                if (isFirst) isFirst = false;
                else builder.Append(separator);
            }

            builder.Append(item);
        }

        return builder.ToString();
    }

    /*public static int xIndexOf<T>(this IList<T> list, object item, int stIndex = 0) {
        for (var i = stIndex; i < list.Count; i++) {
            if (list[i].Equals(item)) return i;
        }
        return -1;
    }*/
}
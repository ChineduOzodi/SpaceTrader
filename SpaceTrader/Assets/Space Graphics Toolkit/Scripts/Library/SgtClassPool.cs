using UnityEngine;
using System.Collections.Generic;

public static class SgtClassPool<T>
	where T : class
{
	private static List<T> pool = new List<T>();
	
	public static int Count
	{
		get
		{
			return pool.Count;
		}
	}
	
	static SgtClassPool()
	{
		if (typeof(T).IsSubclassOf(typeof(Object)))
		{
			Debug.LogError("Attempting to use " + typeof(T).Name + " with SgtClassPool. Use SgtObjectPool instead.");
		}
	}
	
	public static void Add(List<T> list, bool clearList = true)
	{
		Add(list, null, clearList);
	}
	
	public static void Add(List<T> list, System.Action<T> onAdd, bool clearList = true)
	{
		if (list != null)
		{
			for (var i = list.Count - 1; i >= 0; i--)
			{
				Add(list[i], onAdd);
			}
			
			if (clearList == true)
			{
				list.Clear();
			}
		}
	}
	
	public static T Add(T entry)
	{
		return Add(entry, null);
	}
	
	public static T Add(T element, System.Action<T> onAdd)
	{
		if (element != null)
		{
			if (onAdd != null)
			{
				onAdd(element);
			}
			
			pool.Add(element);
		}
		
		return null;
	}
	
	public static T Pop()
	{
		return Pop(null);
	}
	
	public static T Pop(System.Predicate<T> match)
	{
		if (pool.Count > 0)
		{
			if (match != null)
			{
				var index = pool.FindIndex(match);
				
				if (index >= 0)
				{
					return QuickPop(index);
				}
			}
			else
			{
				return QuickPop(pool.Count - 1);
			}
		}
		
		return null;
	}
	
	public static void Clear()
	{
		Clear(null);
	}
	
	public static void Clear(System.Action<T> onClear)
	{
		if (onClear != null)
		{
			for (var i = 0; i < pool.Count; i++)
			{
				onClear(pool[i]);
			}
		}
		
		pool.Clear();
	}
	
	public static void ForEach(System.Action<T> onForEach)
	{
		if (onForEach != null)
		{
			for (var i = pool.Count - 1; i >= 0; i--)
			{
				onForEach(pool[i]);
			}
		}
	}
	
	private static T QuickPop(int index)
	{
		var element = pool[index];
		
		pool.RemoveAt(index);
		
		return element;
	}
}
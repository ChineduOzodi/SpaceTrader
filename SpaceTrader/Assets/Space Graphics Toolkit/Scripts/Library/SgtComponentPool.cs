#define DO_NOT_POOL_IN_EDITOR

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtComponentPool : MonoBehaviour
{
	public int Count;

	[HideInInspector]
	public List<Component> Pool = new List<Component>();

	protected virtual void Update()
	{
		Count = Pool.Count;
#if DO_NOT_POOL_IN_EDITOR
		if (Application.isPlaying == false)
		{
			SgtHelper.Destroy(gameObject);
		}
#endif
	}

	protected virtual void OnDestroy()
	{
		for (var i = Pool.Count - 1; i >= 0; i--)
		{
			var element = Pool[i];

			if (element != null)
			{
				Object.DestroyImmediate(element.gameObject);
			}
		}
	}

#if DO_NOT_POOL_IN_EDITOR
	protected virtual void OnValidate()
	{
		if (Application.isPlaying == false)
		{
			SgtHelper.Destroy(gameObject);
		}
	}
#endif

#if DO_NOT_POOL_IN_EDITOR
	protected virtual void OnDrawGizmos()
	{
		if (Application.isPlaying == false)
		{
			SgtHelper.Destroy(gameObject);
		}
	}
#endif
}

public static class SgtComponentPool<T>
	where T : Component
{
	private static SgtComponentPool component;

	/*
	public static int Count
	{
		get
		{
			if (component != null)
			{
				return component.Pool.Count;
			}

			return 0;
		}
	}*/

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
			UpdateComponent();

			if (onAdd != null)
			{
				onAdd(element);
			}

			element.gameObject.hideFlags = HideFlags.DontSave;

			element.transform.SetParent(component.transform, false);

			element.gameObject.SetActive(false);

			component.Pool.Add(element);
		}

		return null;
	}

	public static T Pop(string name, int layer, Transform parent)
	{
		return Pop(name, layer, parent, null);
	}

	public static T Pop(string name, int layer, Transform parent, System.Predicate<T> match)
	{
		UpdateComponent(false);

		if (component != null)
		{
			var pool = component.Pool;

			if (pool.Count > 0)
			{
				if (match != null)
				{
					var index = pool.FindIndex(o => match((T)o));

					if (index >= 0)
					{
						return QuickPop(name, layer, parent, index);
					}
				}
				else
				{
					var element = QuickPop(name, layer, parent, pool.Count - 1);

					// If the top is null then clean the whole pool
					if (element == null)
					{
						Debug.LogWarning("Popped a null element");

						Clean();

						if (pool.Count > 0)
						{
							return QuickPop(name, layer, parent, pool.Count - 1);
						}
					}
					else
					{
						return element;
					}
				}
			}
		}

		return SgtHelper.CreateGameObject(name, layer, parent).AddComponent<T>();
	}

	public static void Clear()
	{
		Clear(t => Object.DestroyImmediate(t.gameObject));
	}

	public static void Clear(System.Action<T> onClear)
	{
		UpdateComponent(false);

		if (onClear != null && component != null)
		{
			var pool = component.Pool;

			for (var i = 0; i < pool.Count; i++)
			{
				var element = pool[i];

				if (element != null)
				{
					onClear((T)element);
				}
			}

			pool.Clear();
		}
	}

	public static void ForEach(System.Action<T> onForEach)
	{
		UpdateComponent(false);

		if (onForEach != null && component != null)
		{
			var pool = component.Pool;

			for (var i = pool.Count - 1; i >= 0; i--)
			{
				var element = pool[i];

				if (element == null)
				{
					onForEach((T)element);
				}
				else
				{
					pool.RemoveAt(i);
				}
			}
		}
	}

	public static void Clean()
	{
		UpdateComponent(false);

		if (component != null)
		{
			var count = component.Pool.RemoveAll(e => e == null);

			if (count > 0)
			{
				Debug.LogWarning(typeof(T).Name + " pool contained " + count + " null elements");
			}
		}
	}

	private static T QuickPop(string name, int layer, Transform parent, int index)
	{
		var pool    = component.Pool;
		var element = pool[index];

		pool.RemoveAt(index);

		var tElement = (T)element;

		if (tElement != null)
		{
			tElement.transform.SetParent(parent, false);
			tElement.transform.localPosition = Vector3.zero;
			tElement.transform.localRotation = Quaternion.identity;
			tElement.transform.localScale    = Vector3.one;

			tElement.gameObject.name      = name;
			tElement.gameObject.layer     = layer;
			tElement.gameObject.hideFlags = HideFlags.None;
			tElement.gameObject.SetActive(true);
		}

		return tElement;
	}

	private static void UpdateComponent(bool allowCreation = true)
	{
		if (component == null)
		{
			var name = "SgtComponentPool<" + typeof(T).Name + ">";
			var root = GameObject.Find(name);

			if (root == null && allowCreation == true)
			{
				root = new GameObject(name);
			}

			if (root != null)
			{
				root.hideFlags = HideFlags.DontSave;

				Object.DontDestroyOnLoad(root);

				component = root.GetComponent<SgtComponentPool>();

				if (component == null && allowCreation == true)
				{
					SgtHelper.BeginStealthSet(root);
					{
						component = root.AddComponent<SgtComponentPool>();
					}
					SgtHelper.EndStealthSet();
				}
			}
		}
	}
}

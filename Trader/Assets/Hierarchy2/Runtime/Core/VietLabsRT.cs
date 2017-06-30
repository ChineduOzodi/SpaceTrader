#if UNITY_EDITOR

using UnityEngine;

public class VietLabsRT : MonoBehaviour {
	private static bool _searched;
    private static VietLabsRT _inst;
    static public T Get<T>(bool autoCreate = true) where T : vlbRTBase {

	    if (_inst == null) {
			if (_searched && !autoCreate) return null;

            var all = Resources.FindObjectsOfTypeAll<VietLabsRT>();
			_searched = true;

            if (all.Length > 0) {
                _inst = all[0];
            } else {
				if (!autoCreate) return null;
                var go = new GameObject { name = "~vietlabs", hideFlags = HideFlags.NotEditable, tag = "EditorOnly" };
                _inst = go.AddComponent<VietLabsRT>();
            }
	    }
	    
		_inst.gameObject.hideFlags &= ~HideFlags.NotEditable;

        var result = _inst.GetComponent<T>();
        if (autoCreate && result == null) {
            result = _inst.gameObject.AddComponent<T>();
            result.Init();
        }
        return result;
    }
}

public class vlbRTBase : MonoBehaviour {
    virtual public vlbRTBase Init() {
        return this;
    }
}

#endif
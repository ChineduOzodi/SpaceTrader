using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class EditorResource {
    //public static string resourcePath = "Assets/Hierarchy2/Editor/res/";
    private static Dictionary<string, Texture2D> map;
    private static string[] _resourceNames;
    private static string _resourcePath;

	public static string resourcePath {
		get {
            if (!string.IsNullOrEmpty(_resourcePath)) return _resourcePath;

            //find the EditorResource one
            var identities = new[] { "Hierarchy2", "Editor" };
            var pathList    = Directory.GetFiles(Application.dataPath, "*eye.png", SearchOption.AllDirectories);

            /*if (pathList.Length == 1) {
                _resourcePath = pathList[0].Replace("eye.png", "");
                return _resourcePath;
            }*/

			for (var i = 0; i < pathList.Length; i++) {
				var path    = pathList[i].Replace("\\", "/");
			    var isValid = true;

			    foreach (var id in identities) {
                    if (path.Contains(id)) continue;

			        isValid = false; 
			        break;
			    }

			    if (!isValid) continue;
			    _resourcePath = path.Replace("eye.png", "");
			    break;
			}
			
            if (string.IsNullOrEmpty(_resourcePath)) Debug.LogWarning("eye.png not found, make sure you have imported correctly !\n" + pathList.xJoin("\n"));
			return _resourcePath;
		}
	}

    public static string[] ResourceNames {
        get {
            if (_resourceNames != null) return _resourceNames;
            var files = Directory.GetFiles(resourcePath).ToList();
            files.RemoveAll(item=>item.LastIndexOf(".meta")!=-1);
            _resourceNames = files.Select(item=>item.Replace(resourcePath, "")).ToArray();
            //Debug.Log(ArrayX.Join(_resourceNames, " , "));
            return _resourceNames;
        }
    }

    private static Texture2D BlankTex;

    public static Texture2D GetTexture2D(string id) {
        if (map == null) map = new Dictionary<string, Texture2D>();
        Texture2D result;

        if (map != null && map.ContainsKey(id)) {
            result = map[id];
            if (result != null) return result;
            map.Remove(id);
        }
    
        var path = resourcePath + id + ".png";
        if (!File.Exists(path)) {
            Debug.LogWarning("EditorResource <" + id + "> not found at path=" + path + " you may have just move Vietlabs folder around, trying to detect the new path ...");
            
            //try to get the patch again next time ?
            //_resourcePath = null;

            return BlankTex ?? (BlankTex = new Texture2D(4,4, TextureFormat.ARGB32, false));
        }

        var ba = File.ReadAllBytes(path);
        result = new Texture2D(4, 4, TextureFormat.ARGB32, false) {hideFlags = HideFlags.HideAndDontSave};
        result.LoadImage(ba);
        map.Add(id, result);

        return result;
    }
}

/*public class ERToggleIcon {
    public string[] resNames;
    public Color[] colorList; // dark-on, dark-off, light-on, light-off
}


public static class EditorResourceGUI {
    static public void DrawRes(this Rect r, ERToggleIcon info) {
        
    }
}*/
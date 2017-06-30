using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class FileX {
    
	
	static public void xDelete(this string path) {
		if (File.Exists(path)) {
			File.Delete(path);
		} else if (Directory.Exists(path)) {
			Directory.Delete(path);
		}
	}

    

    public static string xToAbsolutePath(this string path) { return new FileInfo(@path).FullName; }

    public static string xToRelativePath(this string path) {
        var fullPath = (new FileInfo(path)).FullName;
        var basePath = (new FileInfo("Assets")).FullName;
        return "Assets" + (fullPath.Replace(basePath, "")).Replace(@"\", "/");
    }

    public static string[] xGetPaths(this FileInfo[] fileList) {
        return fileList.ToList()
            .Select(file => file.FullName)
            .ToArray();
    }

    public static bool xIsFolder(this string path) {
        if (!string.IsNullOrEmpty(path)) return (File.GetAttributes(@path) & FileAttributes.Directory) == FileAttributes.Directory;
        Debug.LogWarning("vlbFile.IsFolder() Error - path should not be null or empty");
        return false;
    }

    public static bool xIsFile(this string path) {
        if (!string.IsNullOrEmpty(path)) return (File.GetAttributes(@path) & FileAttributes.Directory) != FileAttributes.Directory;
        Debug.LogWarning("vlbFile.IsFile() Error - path should not be null or empty");
        return false;
    }

    public static string xGetName(this string path) {
        return path.xIsFolder() ? new DirectoryInfo(@path).Name : new FileInfo(@path).Name;
    }

    public static string xGetExtension(this string path) {
        return path.xIsFolder() ? new DirectoryInfo(@path).Extension : new FileInfo(@path).Extension;
    }

    public static string xGetNameWithoutExtension(this string path) {
        //replace is not safe, what if the path contains a folder with exactly the same extension with the current file/folder ?
        //should replace the last one instead
        return path.xGetName()
            .Replace(path.xGetExtension(), "");
    }

    public static string xCreatePath(this string path) {
        var info = Directory.CreateDirectory(@path);
        AssetDatabase.Refresh();
        return info.FullName;
    }

    public static string xParentFolder(this string path) {
        var directoryInfo = new DirectoryInfo(@path).Parent;
        return directoryInfo != null ? (path.xIsFolder() ? directoryInfo.FullName : new FileInfo(@path).DirectoryName) : null;
    }
	
	static public string xCurrentFolder(this string path) {
		return path.xIsFile() ? path.xParentFolder() : path;
	}
	

    public static string[] xGetFolders(this string path, string pattern = null, bool recursive = false) {
        return Directory.GetDirectories(
            path, pattern ?? "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }

    public static string[] xGetFiles(this string path, string pattern = null, bool recursive = false) {
        return Directory.GetFiles(
            path, pattern ?? "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }
	
	


/********************************  BYTE-64 ******************************/
    
    static internal string xReadBase64(this string filePath) {
		return Convert.ToBase64String(filePath.xReadBytes());
	}
	
	static internal string xReadBase64(this string[] filePaths) {
		var result = "";
		if (filePaths.Length == 1) {
			foreach (var path in filePaths) {
				result += path + "\n" + path.xReadBase64() + "\n";
			}
		} else {
			result = filePaths[0].xReadBase64();
		}
		
		return result;
	}


/*********************************  BYTES *******************************/

    internal static byte[] xReadBytes(this string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var nBytes = (int)(new FileInfo(filePath).Length);
        return new BinaryReader(fs).ReadBytes(nBytes);
    }

    internal static void xWriteBytes(this byte[] bytes, string filePath)
    {
        var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
        var bw = new BinaryWriter(fs);
        bw.Write(bytes);
        bw.Flush();
    }

/*********************************  TEXT *******************************/

    internal static void xWriteText(this string text, string filePath, bool overwrite = false) {
        File.WriteAllText(filePath, text, Encoding.Unicode);

        //var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        //var bw = new StreamWriter(fs);
        //bw.Write(text);
        //bw.Flush();
    }

    internal static string xReadText(this string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var br = new StreamReader(fs);
        return br.ReadToEnd();
    }

/****************************  TEXTURE 2D *******************************/
    
    static internal Texture2D xReadTexture2D(this string filePath) {
		var tex = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		tex.LoadImage(filePath.xReadBytes());
		tex.Apply();
		return tex;
	}

    static public void xWritePNG(this Texture2D tex, string path) {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        File.WriteAllBytes(path + tex.name + ".png", tex.EncodeToPNG());
    }

/****************************  MESH *******************************/
    
    public static string xWriteMesh(Mesh _mesh, string path, string labels = "mesh") {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string fullPath = path + "/" + _mesh.name + "_mesh.asset";

        //mesh or path already existed
        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(_mesh)) ||
            string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(fullPath)))
        {
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(_mesh))) AssetDatabase.CreateAsset(_mesh, fullPath);
            if (!string.IsNullOrEmpty(labels)) AssetDatabase.SetLabels(_mesh, labels.Split(';'));
            AssetDatabase.SaveAssets();
        }

        return AssetDatabase.AssetPathToGUID(fullPath);
    }
    public static string xWriteMaterial(Material _mat, string path, string labels = "material", bool appendMat = true)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string fullPath = path + "/" + _mat.name + (appendMat ? "_mat.asset" : ".asset");

        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(_mat)) ||
            string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(fullPath)))
        {
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(_mat))) AssetDatabase.CreateAsset(_mat, fullPath);
            if (!string.IsNullOrEmpty(labels)) AssetDatabase.SetLabels(_mat, labels.Split(';'));
            AssetDatabase.SaveAssets();
        }

        return AssetDatabase.AssetPathToGUID(fullPath);
    }







}
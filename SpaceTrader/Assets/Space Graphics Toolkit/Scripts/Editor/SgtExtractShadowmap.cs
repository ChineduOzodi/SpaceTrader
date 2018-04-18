using UnityEngine;
using UnityEditor;

public class SgtExtractShadowmap : MonoBehaviour
{
	[MenuItem("CONTEXT/TextureImporter/Extract Shadowmap", true)]
	public static bool ExtractShadowmapValidate(MenuCommand mc)
	{
		var path = AssetDatabase.GetAssetPath(mc.context);
		
		if (string.IsNullOrEmpty(path) == false)
		{
			var assets = AssetDatabase.LoadAllAssetsAtPath(path);
			
			foreach (var asset in assets)
			{
				if (asset is Texture2D)
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
	[MenuItem("CONTEXT/TextureImporter/Extract Shadowmap")]
	public static void ExtractShadowmap(MenuCommand mc)
	{
		var path = AssetDatabase.GetAssetPath(mc.context);
		
		if (string.IsNullOrEmpty(path) == false)
		{
			var assets = AssetDatabase.LoadAllAssetsAtPath(path);
			
			if (RemoveExtension(ref path) == true)
			{
				foreach (var asset in assets)
				{
					var texture = asset as Texture2D;
					
					if (texture != null)
					{
						SaveShadowmap(texture, path);
					}
				}
			}
		}
	}
	
	private static bool RemoveExtension(ref string path)
	{
		var lastDot = path.LastIndexOf(".");
		
		if (lastDot >= 0)
		{
			path = path.Substring(0, lastDot);
			
			return true;
		}
		
		return false;
	}
	
	private static void SaveShadowmap(Texture2D source, string path)
	{
		SgtHelper.MakeTextureReadable(source);
		
		var clone  = (Texture2D)Instantiate(source);
		var fPath  = path + "_Shadow.asset"; //fPath = AssetDatabase.GenerateUniqueAssetPath(fPath);
		var jPath  = path + "_Shadow.tempasset"; //jPath = AssetDatabase.GenerateUniqueAssetPath(jPath);
		var width  = source.width;
		var height = source.height;
		
		for (var i = 0; i < source.mipmapCount; i++)
		{
			var pixels = ConvertPixels(source.GetPixels32(i), width, height);
			
			clone.SetPixels32(pixels, i);
			
			width  = Mathf.Max(1, width  / 2);
			height = Mathf.Max(1, height / 2);
		}
		
		clone.Apply(false);
		
		SaveTexture(clone, fPath, jPath);
		
		SgtHelper.Destroy(clone);
		
		Debug.Log("Saved shadowmap to: " + fPath);
	}
	
	private static Color32[] ConvertPixels(Color32[] pixels, int width, int height)
	{
		for (var i = pixels.Length - 1; i >= 0; i--)
		{
			var pixel = pixels[i];
			
			pixel.r = pixel.g = pixel.b = pixel.a = (byte)(255 - (int)pixel.a);
			
			pixels[i] = pixel;
		}
		
		var clear = new Color32(255, 255, 255, 255);
		
		for (var y = height - 1; y >= 0; y--)
		{
			var o = y * width;
			
			pixels[o + 0] = clear;
			pixels[o + width - 1] = clear;
		}
		
		return pixels;
	}
	
	// Major hacks to get the saving working
	private static void SaveTexture(Texture2D texture, string path, string tempPath)
	{
		texture.wrapMode   = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Trilinear;
		texture.anisoLevel = 8;
		
		AssetDatabase.CreateAsset(texture, tempPath);
		
		System.IO.File.Copy(tempPath, path, true);
		
		AssetDatabase.DeleteAsset(tempPath);
		
		SgtHelper.SetDirty(AssetImporter.GetAtPath(path));
		
		AssetDatabase.Refresh();
		
		AssetDatabase.ImportAsset(path);
		
		SgtHelper.SelectAndPing(AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)));
	}
}
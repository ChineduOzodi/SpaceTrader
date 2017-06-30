using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using vietlabs;

public static class TextureX {
    private static Dictionary<Color, Texture2D> ColorMap;
    private static Dictionary<string, Texture2D> Map;

    public static Texture2D xToTexture2D(this string base64, string id = null) {
        var tex = new Texture2D(16, 16);
        tex.xSetFlag(HideFlags.HideAndDontSave, true);
        tex.LoadImage(Convert.FromBase64String(base64));

        if (string.IsNullOrEmpty(id)) return tex;
        if (Map == null) Map = new Dictionary<string, Texture2D>();
        if (!Map.ContainsKey(id) || Map[id] == null) Map.Add(id, tex);
        else {
            Debug.Log("vlbTexture.ToTexture2D() Error :: id <" + id + "> already exist and will be replaced");
            Map[id] = tex;
        }

        return tex;
    }

    public static bool HasTextureId(string id) { return Map != null && Map.ContainsKey(id) && Map[id] != null; }

    public static Texture2D xGetTextureFromId(this string id) {
        if (string.IsNullOrEmpty(id)) {
            Debug.LogWarning("vlbTexture.GetTextureFromId() Error :: id should not be null or empty");
            return null;
        }
        if (Map == null || !Map.ContainsKey(id)) {
            Debug.LogWarning(
                "vlbTexture.GetTextureFromId() Error :: id <" + id
                + "> not found, consider adding it first by calling base64Source.ToTexture2D(" + id + ")");
            return null;
        }

        if (Map[id] != null) return Map[id];

        Debug.LogWarning(
            "vlbTexture.GetTextureFromId() Error : texture with id <" + id + "> is destroyed, consider adding it again");
        return null;
    }

    public static Texture2D xGetTexture2D(this Color c) {
        if (ColorMap == null) ColorMap = new Dictionary<Color, Texture2D>();
        if (ColorMap.ContainsKey(c) && ColorMap[c] != null) return ColorMap[c];
        var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        tex.xSetFlag(HideFlags.HideAndDontSave, true);
        tex.SetPixel(0, 0, c);
        tex.Apply();
        ColorMap.Add(c, tex);
        return tex;
    }

    public static Rect xGetSizeRect(this Texture2D tex) { return new Rect(0, 0, tex.width, tex.height); }

    static internal Texture2D xImport(this Texture2D tex,
        bool?                       readable    = null,
        int?                        aniso       = null,
        bool?                       mipMap      = null,
        bool                        reload      = true,

        int?                        maxSize     = null,
        TextureWrapMode?            wrap        = null,
        TextureImporterFormat?      format      = null,
        TextureImporterNPOTScale?   npotScale   = null)
    {
        var path = AssetDatabase.GetAssetPath(tex);
        var changed = false;
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);

        if (importer.textureType != TextureImporterType.Default) {
            importer.textureType = TextureImporterType.Default;
            changed = true;
        }

        if (wrap != null && importer.wrapMode != wrap) {
            importer.wrapMode = wrap.Value;
            changed = true;
        }

        if (readable != null && importer.isReadable != readable) {
            importer.isReadable = readable.Value;
            changed = true;
        }

        if (format != null && importer.textureFormat != format) {
            importer.textureFormat = format.Value;
            changed = true;
        }

        if (maxSize != null && importer.maxTextureSize != maxSize) {
            importer.maxTextureSize = maxSize.Value;
            changed = true;
        }

        if (mipMap != null && importer.mipmapEnabled != mipMap) {
            importer.mipmapEnabled = mipMap.Value;
            changed = true;
        }

        if (!importer.alphaIsTransparency) {
            importer.alphaIsTransparency = true;
            changed = true;
        }

        if (aniso != null && importer.anisoLevel != aniso) { 
            importer.anisoLevel = aniso.Value;
            changed = true;
        }

        if (npotScale != null && importer.npotScale != npotScale) {
            importer.npotScale = npotScale.Value;
            changed = true;
        }

        if (!changed) return tex;

        AssetDatabase.ImportAsset(path, reload ? ImportAssetOptions.ForceSynchronousImport : ImportAssetOptions.Default);
        return reload ? (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) : null;
    }


}
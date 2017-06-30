using System;
using System.Collections;
using System.Text;
using UnityEngine;

public static class StringX {
    public static string xReplaceEvery(this string src, IList oldStrArr, string newStr) {
        for (var i = 0; i < oldStrArr.Count; i++) {
            src = src.Replace((string) oldStrArr[i], newStr);
        }
        return src;
    }

    public static string[] xSplit(this string src, string spliter, bool removeEmpty = false) {
        return src.Split(new[] {spliter}, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }

    public static string xSetCharAt(this string src, char c, int idx) {
        var sb = new StringBuilder(src);
        if (idx < 0 || idx >= sb.Length) {
            Debug.LogWarning("xSetCharAt error :: Invalid idx <"+idx+"> within source <" + src + ">");
            return src;
        }
        sb[idx] = c;
        return sb.ToString();
    }

    public static string xDuplicate(this string str, int nTimes) {
        if (string.IsNullOrEmpty(str)) return str;

        var builder = new StringBuilder();
        for (var i = 0; i < nTimes; i++) {
            builder.Append(str);
        }
        return builder.ToString();
    }

    public static string xEnum2String(this Enum e) {
        if (e == null) return null;
        var names = Enum.GetNames(e.GetType());
        var values = Enum.GetValues(e.GetType());
        return names[Array.IndexOf(values, e)];
    }

    public static string xInQuote(this object str) { return "\"" + str + "\""; }
    public static string xInBraces(this object str) { return "{" + str + "}"; }
    public static string xInParentheses(this object str) { return "(" + str + ")"; }
    public static string InTags(this object str) { return "<" + str + ">"; }

    private static string xGetTypePrefix(Type typeT) {
        if (typeT == typeof(bool))      return "bool";
        if (typeT == typeof(Color32))   return "RGBA";
        if (typeT == typeof(Color))     return "RGBA";
        if (typeT == typeof(int))       return "int";
        if (typeT == typeof(float))     return "float";
        if (typeT == typeof(string))    return "string";

        Debug.LogWarning("Unsupported type <" + typeT + ">");
        return string.Empty;
    }

    public static Type xGetPrefixType(string prefix) {
        switch (prefix) {
            case "bool"     : return typeof(bool);
            case "RGBA"     : return typeof(Color);
            case "int"      : return typeof(int);
            case "float"    : return typeof(float);
            case "string"   : return typeof(string);
        }

        Debug.Log("Unsupported type prefix <" + prefix+">");
        return null;
    }


    public static string xStringEncodeT<T>(this T t, bool withPrefix = true) {
        return t.xStringEncode(typeof(T), withPrefix);
    }

    //stupid & slow first, optimize later
    static string xEscape(string str) {
        return str  .Replace(",", @"\comma/")
                    .Replace(":", @"\colon/")
                    .Replace(";", @"\semicolon/")
                    .Replace("|", @"\split/")
                    .Replace("\r", @"\return/")
                    .Replace("\n", @"\newline/");
    }

    //stupid & slow first, optimize later
    static string xUnescape(string str) {
        return str  .Replace(@"\comma/",         ",")
                    .Replace(@"\colon/",         ":")
                    .Replace(@"\semicolon/",     ";")
                    .Replace(@"\split/",         "|")
                    .Replace(@"\return/",        "\r")
                    .Replace(@"\newline/",       "\n");
    }

    public static string xStringEncode(this object t, Type typeT = null, bool withPrefix = true) {
        if (typeT == null) typeT = t.GetType();

        if (typeT == typeof(byte[])) {
            return (withPrefix ? "byte[]:" : string.Empty) + Convert.ToBase64String((byte[]) t);
        }

        //Debug.Log(typeT + ":" + typeT.IsArray);

        if (typeT.IsArray) {
            var elmType = typeT.GetElementType();    
            var arr = (Array)t;
            var result = new StringBuilder();
            result.Append(xGetTypePrefix(elmType));
            result.Append("[]:");

            for (var i = 0; i<arr.Length; i++) {
                if (i > 0) result.Append(",");
                result.Append(arr.GetValue(i).xStringEncode(elmType, false));
            }
            return result.ToString();
        }

        var prefix = withPrefix ? xGetTypePrefix(typeT) + ":" : string.Empty;
        if (typeT == typeof(bool))  return prefix + ((bool)t ? "1" : "0");
        if (typeT == typeof(int))   return prefix + ((int)t);
        if (typeT == typeof(float)) return prefix + ((float)t);
        if (typeT == typeof(string)) return prefix + xEscape((string) t);

        if (typeT == typeof(Color)) {
            typeT = typeof(Color32);
            t = (Color32)(Color) t;
        }
        if (typeT == typeof(Color32)) {
            var c = (Color32) t;
            return string.Format("{0}{1}:{2}:{3}:{4}", withPrefix ? "RGBA:" : string.Empty, c.r, c.g, c.b, c.a);
        }

        Debug.LogWarning("String EncodeError - Unsupported type <" + typeT + ">");
        return null;
    }


    //public static string StringEncode(this bool b) {
    //    return "bool:" + (b ? "1" : "0");
    //}
    /*public static string StringEncode(this Color32 c) {
        return string.Format("RGBA:{0},{1},{2},{3}", c.r, c.g, c.b, c.a);
    }
    public static string StringEncode(this Color c) { return ((Color32) c).StringEncode();}
    public static string StringEncode(this Color[] arr) {
        var result = "RGBA[]";
        for (var i = 0; i < arr.Length; i++) {
            Color32 c = arr[i];
            result += string.Format(":{0},{1},{2},{3}", c.r, c.g, c.b, c.a);
        }
        return result;
    }


    public static string StringEncode(this string[] arr) {
        var result = "string[]";
        for (var i = 0; i < arr.Length; i++) {
            result += ":"+ arr[i].Replace(",", "~se{comma}");
        }
        return result;
    }*/
    public static string xStringEncode(this int[] intArr) {
        var str = "int[]";
        for (var i = 0; i < intArr.Length; i++) {
            str += ":"+ intArr[i];
        }
        return str;
    }

    public static Color DecodeColor(this string str) {
        if (string.IsNullOrEmpty(str)) return Color.white;
        var arr = str.xSplit(":");
        return new Color32(byte.Parse(arr[0]), byte.Parse(arr[1]), byte.Parse(arr[2]), byte.Parse(arr[3]));
    }
    /*public static Color[] DecodeColorArray(this string str) {
        if (string.IsNullOrEmpty(str)) return new Color[0];
        var arr     = str.Split(":");
        var result  = new Color[arr.Length];

        for (var i = 0; i < arr.Length; i++) {
            var crr = arr[i].Split(",");
            result[i] = new Color32(
                byte.Parse(crr[0]), byte.Parse(crr[1]), byte.Parse(crr[2]), byte.Parse(crr[3])
            );
        }
        return result;
    }
    public static string[] DecodeStringArray(this string str) {
        if (string.IsNullOrEmpty(str)) return new string[0];

        var arr = str.Split(",");
        for (var i = 0; i < arr.Length; i++) {
            arr[i] = arr[i].Replace("~se{comma}", ",");
        }
        return arr;
    }
    public static int[] DecodeIntArray(this string str) {
        if (string.IsNullOrEmpty(str)) return new int[0];

        var arr = str.Split(",");
        var result = new int[arr.Length];

        for (var i = 0; i < arr.Length; i++) {
            result[i] = int.Parse(arr[i]);
        }
        return result;
    }*/

    public static object xStringDecode(this string str) {
        var idx = str.IndexOf(":");
        if (idx == -1) {
            Debug.LogWarning("Invalid string to decode, identify token not found, must contains a ':' " + str);
            return null;
        }

        var token = str.Substring(0, idx);
        var value = str.Substring(idx + 1);

        if (token.Contains("[]")) {
            var arr     = value.xSplit(",");
            var prefix  = token.Replace("[]", string.Empty);
            var typeT   = xGetPrefixType(prefix);

            var result  = Array.CreateInstance(typeT, arr.Length);
            for (var i = 0; i < arr.Length; i++) {
                var decoded = (prefix + ":" + arr[i]).xStringDecode();
                //Debug.Log(decoded + "::" + typeT + ":" + decoded.GetType() +":"+ result.GetType());
                result.SetValue(decoded, i);
            }
            return result;
        }

        switch (token) {
            case "bool"     : return value == "1";
            case "RGBA"     : return DecodeColor(value);
            case "string"   : return xUnescape(value);
            case "int"      : return int.Parse(value);
            case "float"    : return float.Parse(value);
        }

        Debug.Log("Invalid Token - unsupported <" + token + "> with value <" + value + ">");
        return null;
    }

    public static T xStringDecode<T>(this string str) { return (T)str.xStringDecode(); }
}
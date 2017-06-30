using System;
using UnityEditor;
using UnityEngine;

public static class MenuX {
    public static GenericMenu xAdd(this GenericMenu menu, string text, Action func, bool selected = false) {
        if (func == null) menu.AddDisabledItem(new GUIContent(text));
        else menu.AddItem(new GUIContent(text), selected, () => func());

        return menu;
    }

    public static GenericMenu xAddSep(this GenericMenu menu, string text) {
#if UNITY_EDITOR_WIN
        menu.AddSeparator(text);
#else
        if (string.IsNullOrEmpty(text) || (text.IndexOf("/") == -1)){
			menu.AddSeparator(text);
		} else {
            menu.AddSeparator((text ?? "") + "--------------------");
		}
#endif
        return menu;
    }

    public static GenericMenu xAddIf(this GenericMenu menu, bool expression, string text, Action func) {
        if (expression) menu.xAdd(text, func);
        return menu;
    }

    public static GenericMenu xAdd(this GenericMenu menu, bool has, string text1, string text2, Action<bool> func) {
        menu.AddItem(new GUIContent(has ? text1 : text2), false, () => func(has));
        return menu;
    }

    public static GenericMenu xAddIf(this GenericMenu menu, bool expression, bool has, string text1, string text2,
        Action<bool> func) {
        if (expression) xAdd(menu, has, text1, text2, func);
        return menu;
    }
}
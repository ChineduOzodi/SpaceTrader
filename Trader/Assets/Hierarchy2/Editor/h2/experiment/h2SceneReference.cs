using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Text;

public class h2SceneReference {
	static int gameObjectCount;
	static int componentCount;
	
	static int skipArrayCount;
	static int skipPropertyCount;
	static int nullCount;
	static int monoCount;
	static int materialCount;
	static int textureCount;
	static int spriteCount;
	static int shaderCount;
	static int textCount;
	static int audioCount;
	static int animCount;
	static int meshCount;
	
	static Dictionary<int, ReferenceInfo> dict;
	
	// Do this only once for every scene opened
	static public void ScanHierarchy(){
		dict = new Dictionary<int, ReferenceInfo>();
		
		
		gameObjectCount = 0;
		componentCount = 0;
		
		skipArrayCount = 0;
		skipPropertyCount = 0;
		nullCount = 0;
		monoCount = 0;
		materialCount = 0;
		textureCount = 0;
		spriteCount = 0;
		shaderCount = 0;
		textCount = 0;
		audioCount = 0;
		animCount = 0;
		meshCount = 0;
		
		GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>() ;
		
		for (var i = 0;i < allObjects.Length;i ++){
			var go = allObjects[i];
			ScanGameObject(go); //maybe we can do it gradually
		}
		
		//Debug.Log(string.Format("GameObject={0}\n Components={1}\n skipArray={2}\n skipProperty={3}\n nullCount={4}\n monoCount={5}\n materialCount={6}\n textureCount={7}\n spriteCount={8}\n shaderCount={9}\n textCount={10}\n audioCount={11}\n animCount={12}\n meshCount={13}\n",
		//	gameObjectCount,
		//	componentCount,
		//	skipArrayCount,
		//	skipPropertyCount,
		//	nullCount,
		//	monoCount,
		//	materialCount,
		//	textureCount,
		//	spriteCount,
		//	shaderCount,
		//	textCount,
        //    audioCount,
        //    animCount,
        //    meshCount
		//));
		
		//Debug.Log("Total Object inspected :: " + dict.Count);
		
		//foreach(object go in allObjects)
		//	if (go.activeInHierarchy)
		//		print(thisObject+" is an active object") ;
	}
	
	static void ScanGameObject(GameObject go, bool force = false){
		var instId = go.GetInstanceID();
		if (!force && dict.ContainsKey(instId)) return;
		
		var fullName = GetHierarchyName(go.transform);
		
		if (!dict.ContainsKey(instId)) {
			gameObjectCount++;
			dict.Add(instId, new ReferenceInfo(ReferenceType.GameObject, go.name, fullName));
		}
		
		var compList = go.GetComponents<Component>();
		for (var j= 0; j < compList.Length; j++){
			ScanComponent(compList[j]);
		}
	}

	static string GetHierarchyName(Transform t){
		if (t == null)
			return string.Empty;

		var p = t.parent;
		var result = t.name;

		while (p != null) {
			result = p.name + "/" + result;
			p = p.parent; 
		}

		return result;
	}
	
	static void ScanComponent(Component c, bool force = false){
		if (c == null) {
			nullCount++;
			return;
		}
		
		var instId = c.GetInstanceID();
		var hasKey = dict.ContainsKey(instId);
		
		if (!force && hasKey) return;
		
		ReferenceInfo info = hasKey ? dict[instId] : null;
		if (info == null){ // may not hasKey or has key with a null object
			info = new ReferenceInfo(ReferenceType.Component, c.GetType().FullName, c.GetType().FullName + " (" + GetHierarchyName(c.transform) + ")");
			if (hasKey){
				dict[instId] = info;
			} else {
				componentCount++;
				dict.Add(instId, info);
			}
		}
		
		var props = SerializeX.xGetSerializedProperties(c);
		for (var k = 0; k < props.Length; k++){
			if (props[k].isArray){
				skipArrayCount++;
				//TODO : Process Array elements (stop at N level ? )
				continue;
			}
			
			if (props[k].propertyType == SerializedPropertyType.ObjectReference) {
				var refObj	= props[k].objectReferenceValue;
				if (refObj == null) continue;

				//TODO : check for references
				if (refObj is Component){
					ScanComponent((Component)refObj);
					declareRef(instId, refObj.GetInstanceID());
				} else if (refObj is GameObject){
					ScanGameObject((GameObject)refObj);	
					declareRef(instId, refObj.GetInstanceID());
				} else if (refObj is MonoScript) {
					monoCount++;
				} else if (refObj is Texture2D){
					textureCount++;
				} else if (refObj is Material){
					materialCount++;
				} else if (refObj is Sprite){
					spriteCount++;
				} else if (refObj is Shader){
					shaderCount++;
				} else if (refObj is TextAsset){
					textCount ++;
				} else if (refObj is AudioClip){
					audioCount ++;
				} else if (refObj is AnimationClip){
					animCount++;
				} else if (refObj is Mesh){
					meshCount++;
				}
				else {
					//skipCount++;
					//if (skipPropertyCount % 10 == 0) 
					//Debug.Log("Skipping object ... " + refObj.GetType());
					skipPropertyCount++;
				}
			}
		}
	}

	static void declareRef(int sourceInst, int destInst){
		var destInfo = dict [destInst];

		if (!destInfo.list.Contains(destInst)) {
			destInfo.list.Add (sourceInst);
		}
	}

	static public void LogReferences(GameObject go){
		var cList = go.GetComponents<Component>();
		var sb = new StringBuilder();
		
		Append(sb, dict[go.GetInstanceID()]);
		
		for (var i = 0;i < cList.Length; i++){
			var info = dict[cList[i].GetInstanceID()];
			sb.Append("\n\t");
			Append(sb, info, false);
		}
		
		Debug.Log(sb.ToString());
	}
	
	static void Append(StringBuilder sb, ReferenceInfo info, bool useFullName = false){
		if (info.type == ReferenceType.GameObject){
			sb.Append("[GameObject] ");
			sb.Append(info.name);
		} else if (info.type == ReferenceType.Component){
			//sb.Append("[Component] ");
			sb.Append(useFullName ? info.path : info.name);
		}
		for (var j= 0 ; j < info.list.Count; j++){
			var id = info.list[j];
			if (!dict.ContainsKey(id)) continue;
			
			sb.Append("\n\t\t");
			sb.Append(dict[id].path);
		}
	}
}

enum ReferenceType {
	GameObject,
	Component,
	Prefab,
	Material
}

class ReferenceInfo {
	public string name;
	public string path;
	public ReferenceType type;
	public List<int> list; //list of objects reference to this one
	
	public ReferenceInfo(ReferenceType t, string name, string path){
		type = t;
		this.name = name;
		this.path = path;
		list = new List<int>();
	}
}



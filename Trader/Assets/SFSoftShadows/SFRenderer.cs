// Super Fast Soft Lighting. Copyright 2015 Howling Moon Software, LLP

#if !UNITY_EDITOR_OSX && ( UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_XBOX360 || UNITY_XBOXONE || UNITY_WP8 || UNITY_WSA || UNITY_WINRT )
#define DIRECTX_WORKAROUND
#endif

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public interface _SFCullable {
	void _CacheWorldBounds();
	Rect _GetWorldBounds();
}

[ExecuteInEditMode]
public class SFRenderer : MonoBehaviour {
	private RenderTexture _lightMap;
	private RenderTexture _ShadowMap;
	
	public bool _linearLightBlending = false;
	public bool _shadows = true;
	public Color _ambientLight = Color.black;
	public float _globalDynamicRange = 1.0f;

	public float _lightMapScale = 8;
	public float _shadowMapScale = 2;

//	public float _globalSoftening = 0.2f;

	public Color _fogColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	public Color _scatterColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	public float _softHardMix = 0.0f;

	public bool linearLightBlending {get {return _linearLightBlending;} set {_linearLightBlending = value;}}
	public bool shadows {get {return _shadows;} set {_shadows = value;}}
	public Color ambientLight {get {return _ambientLight;} set {_ambientLight = value;}}
	public float globalDynamicRange {get {return _globalDynamicRange;} set {_globalDynamicRange = value;}}
	public float lightMapScale {get {return _lightMapScale;} set {_lightMapScale = value;}}
	public float shadowMapScale {get {return _shadowMapScale;} set {_shadowMapScale = value;}}
	public Color fogColor {get {return _fogColor;} set {_fogColor = value;}}
	public Color scatterColor {get {return _scatterColor;} set {_scatterColor = value;}}
	public float softHardMix {get {return _softHardMix;} set {_softHardMix = value;}}

	private Material _shadowMaskMaterial;
	private Material shadowMaskMaterial {
		get {
			if(_shadowMaskMaterial == null){
				_shadowMaskMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/ShadowMask"));
				_shadowMaskMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return _shadowMaskMaterial;
		}
	}
	
	private Material _linearLightMaterial;
	private Material _softLightMaterial;
	private Material lightMaterial {
		get {
			if(_linearLightMaterial == null){
				_linearLightMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/LightBlendLinear"));
				_linearLightMaterial.hideFlags = HideFlags.HideAndDontSave;

				_softLightMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/LightBlendSoft"));
				_softLightMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return (_linearLightBlending ? _linearLightMaterial : _softLightMaterial);
		}
	}
	
	private Material _fogMaterial;
	private Material fogMaterial {
		get {
			if(_fogMaterial == null){
				_fogMaterial = new Material(Shader.Find("Hidden/SFSoftShadows/FogLayer"));
				_fogMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			return _fogMaterial;
		}
	}

	private static RenderTexture GetTexture(float downscale){
		downscale = Mathf.Max(1.0f, downscale);
		
		var camera = Camera.current;
		var width = (int)(camera.pixelWidth/downscale);
		var height = (int)(camera.pixelHeight/downscale);
		return RenderTexture.GetTemporary(width, height);
	}
	
	public static Rect _TransformRect(Matrix4x4 m, Rect r){
		Vector4 c = m.MultiplyPoint3x4(new Vector4(r.x + 0.5f*r.width, r.y + 0.5f*r.height, 0.0f, 1.0f));
		float hw = 0.5f*Mathf.Max(Mathf.Abs(r.width*m[0] + r.height*m[4]), Mathf.Abs(r.width*m[0] - r.height*m[4]));
		float hh = 0.5f*Mathf.Max(Mathf.Abs(r.width*m[1] + r.height*m[5]), Mathf.Abs(r.width*m[1] - r.height*m[5]));
		return new Rect(c.x - hw, c.y - hh, 2.0f*hw, 2.0f*hh);
	}
	
	private static Rect Intersection(Rect r1, Rect r2){
		return Rect.MinMaxRect(
			Mathf.Max(r1.xMin, r2.xMin),
			Mathf.Max(r1.yMin, r2.yMin),
			Mathf.Min(r1.xMax, r2.xMax),
			Mathf.Min(r1.yMax, r2.yMax)
		);
	}
	
	private static Rect Union(Rect r1, Rect r2){
		return Rect.MinMaxRect(
			Mathf.Min(r1.xMin, r2.xMin),
			Mathf.Min(r1.yMin, r2.yMin),
			Mathf.Max(r1.xMax, r2.xMax),
			Mathf.Max(r1.yMax, r2.yMax)
		);
	}
	
	private static Rect ConvertToPixelRect(Rect rect, float w, float h){
		var l = Mathf.Max(0.0f, Mathf.Floor((0.5f*rect.xMin + 0.5f)*w));
		var b = Mathf.Max(0.0f, Mathf.Floor((0.5f*rect.yMin + 0.5f)*h));
		var r = Mathf.Min(w, Mathf.Ceil((0.5f*rect.xMax + 0.5f)*w));
		var t = Mathf.Min(h, Mathf.Ceil((0.5f*rect.yMax + 0.5f)*h));
		return new Rect(l, b, r - l, t - b);
	}
	
	// Calculate a clip matrix given the viewport pixel rect and 2/width, 2/height
	private static Matrix4x4 ClipMatrix(Rect r, float dw, float dh){
		float x = r.x*dw - 1.0f;
		float y = r.y*dh - 1.0f;
		return Matrix4x4.Ortho(x, x + r.width*dw, y, y + r.height*dh, -1.0f, 1.0f);
	}
	
	private List<SFLight> _culledLights = new List<SFLight>();
	private List<SFPolygon> _culledPolygons = new List<SFPolygon>();
	private List<SFPolygon> _perLightCulledPolygons = new List<SFPolygon>();


	private delegate bool CullDelegate<T>(T obj);

	private List<T> CullObjects<T>(List<T> objs, List<T> cache, Rect cullBounds, bool useEditorFallback = true, bool cacheBounds = true, CullDelegate<T> del = null) where T : MonoBehaviour, _SFCullable {
		if(!Application.isPlaying && useEditorFallback){
			// This only runs in the editor since the active light and polygon lists don't exist.
			objs = new List<T>(Component.FindObjectsOfType<T>().Where((o) => o.isActiveAndEnabled));
		}
		
		for(var i = 0; i < objs.Count; i++){
			var obj = objs[i];

			if(cacheBounds) obj._CacheWorldBounds();
			if(cullBounds.Overlaps(obj._GetWorldBounds())){
				if(del == null || del(obj)) cache.Add(obj);
			}
		}
		
		return cache;
	}

	private Rect PolyCullingBounds(List<SFLight> lights, Rect viewBounds){
		var l = viewBounds.xMin;
		var b = viewBounds.yMin;
		var r = viewBounds.xMax;
		var t = viewBounds.yMax;

		for(int i = 0; i < lights.Count; i++){
			var p = lights[i]._position;
			l = Mathf.Min(l, p.x);
			b = Mathf.Min(b, p.y);
			r = Mathf.Max(r, p.x);
			t = Mathf.Max(t, p.y);
        }
        
        return Rect.MinMaxRect(l, b, r, t);
    }

	private void RenderLightMap(RenderTexture target, bool shadows, List<SFLight> lights, Rect viewBounds){
		var cam = Camera.current;
		var projection = cam.projectionMatrix;
		
		var LIGHT_RECT = new Rect(-1, -1, 2, 2);
		var SRC_RECT = new Rect(0, 0, 1, 1);
		
		Graphics.SetRenderTarget(target);
		GL.Clear(false, true, _ambientLight);
		
		List<SFPolygon> polys = (shadows ? CullObjects(SFPolygon._polygons, _culledPolygons, PolyCullingBounds(lights, viewBounds)) : null);

		foreach(var light in lights){
			if(!light.enabled) continue;
			
			if(light._cookieTexture == null){
				Debug.LogWarning("SFLight has no cookie texture set.", light);
				continue;
			}
			
			var color = light.color;
			var transf = light.transform.localToWorldMatrix;
			var boundsTransform = transf*light._lightMatrix;
			var modelView = cam.worldToCameraMatrix*boundsTransform;
			
			float w = target.width;
			float h = target.height;
			var clipBounds = _TransformRect(projection*modelView, LIGHT_RECT);
			var viewport = ConvertToPixelRect(clipBounds, w, h);
			
			GL.Viewport(viewport);
			var clippedProjection = ClipMatrix(viewport, 2.0f/w, 2.0f/h)*projection;
			GL.LoadProjectionMatrix(clippedProjection);
			
			// Draw shadow mask
			if(shadows && light._shadowLayers != 0){
				var radius = light.radius;
				var areaBounds = _TransformRect(transf, new Rect(-radius, -radius, 2.0f*radius, 2.0f*radius));
				var rangeBounds = _TransformRect(boundsTransform, LIGHT_RECT);

				var lightBounds = Union(areaBounds, Intersection(rangeBounds, viewBounds));
				var lightPolys = CullObjects(polys, _perLightCulledPolygons, lightBounds, false, false, delegate(SFPolygon poly){
					return (poly._shadowLayers & light._shadowLayers) != 0;
				});

				var mesh = light._GetShadowMesh(lightPolys);
				if(mesh != null){
					// Note: DrawMesh apparently not affected by the "GL" transform.
					this.shadowMaskMaterial.SetPass(0);
					Graphics.DrawMeshNow(mesh, transf);
				}

				lightPolys.Clear();
			}

			var textureMatrix = (clippedProjection*modelView).inverse;

#if DIRECTX_WORKAROUND
			// Viewport and texture coordinats are flipped on DirectX.
			// Need to flip the projection going in, then flip the texture coordinates coming out.
			var flip = Matrix4x4.Scale(new Vector3(1.0f, -1.0f, 1.0f));
			textureMatrix = flip*textureMatrix*flip;
#endif

			// Composite the light.
			// Draw a fullscreen quad with the light's texture on it.
			// The vertex shader doesn't use any transforms at all.
			// Abuse the projection matrix since there isn't really a better way to pass the texture's transform.
			GL.LoadProjectionMatrix(textureMatrix);
			Graphics.DrawTexture(LIGHT_RECT, light._cookieTexture, SRC_RECT, 0, 0, 0, 0, color, this.lightMaterial);
		}

		if(shadows) _culledPolygons.Clear();
	}
	
	private void OnPreRender(){

		RenderBuffer savedColorBuffer = Graphics.activeColorBuffer;
		RenderBuffer savedDepthBuffer = Graphics.activeDepthBuffer;
		var currentCam = Camera.current;
		var vp = currentCam.projectionMatrix*currentCam.worldToCameraMatrix;
		var viewBounds = _TransformRect(vp.inverse, new Rect(-1.0f, -1.0f, 2.0f, 2.0f));

		var lights = CullObjects(SFLight._lights, _culledLights, viewBounds);
		
		GL.PushMatrix();
		_lightMap = GetTexture(_lightMapScale);
		RenderLightMap(_lightMap, false, lights, viewBounds);
		
		if(_shadows){
			_ShadowMap = GetTexture(_shadowMapScale);

//			this.shadowMaskMaterial.SetFloat("_GlobalSoftening", Mathf.Max(1e-5f, _globalSoftening));
			RenderLightMap(_ShadowMap, true, lights, viewBounds);
		}
		GL.PopMatrix();
		
		// Lights is a cached list. Clear it now since we are done with the contents.
		lights.Clear();
		
		Graphics.SetRenderTarget(null);
		GL.Viewport(currentCam.pixelRect);

		Shader.SetGlobalFloat("_SFGlobalDynamicRange", _globalDynamicRange);
		Shader.SetGlobalTexture("_SFLightMap", _lightMap);
		Shader.SetGlobalTexture("_SFLightMapWithShadows", _shadows ? _ShadowMap : _lightMap);

		Graphics.SetRenderTarget (savedColorBuffer, savedDepthBuffer);

#if DIRECTX_WORKAROUND
		Shader.SetGlobalMatrix("_SFProjection", Camera.current.projectionMatrix);
#endif
	}
	
	private void OnPostRender(){
		var fogCheck = _fogColor.a + _scatterColor.r + _scatterColor.g + _scatterColor.b;
		if(fogCheck > 0.0f){
			GL.PushMatrix(); {
				GL.LoadProjectionMatrix(Matrix4x4.identity);
				GL.LoadIdentity();
				
				var scatter = _scatterColor;
				scatter.a = _softHardMix;
				
				var mat = this.fogMaterial;
				mat.SetColor("_FogColor", _fogColor);
				mat.SetColor("_Scatter", scatter);
				mat.SetPass(0);
				
				GL.Begin(GL.QUADS); {
					GL.MultiTexCoord2(0, 0.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 0.0f);
					GL.MultiTexCoord2(0, 0.0f, 1.0f); GL.Vertex3(-1.0f,  1.0f, 0.0f);
					GL.MultiTexCoord2(0, 1.0f, 1.0f); GL.Vertex3( 1.0f,  1.0f, 0.0f);
					GL.MultiTexCoord2(0, 1.0f, 0.0f); GL.Vertex3( 1.0f, -1.0f, 0.0f);
				} GL.End();
			} GL.PopMatrix();
		}

		Shader.SetGlobalTexture("_SFLightMap", null);
		Shader.SetGlobalTexture("_SFLightMapWithShadows", null);
		RenderTexture.ReleaseTemporary(_lightMap); _lightMap = null;
		RenderTexture.ReleaseTemporary(_ShadowMap); _ShadowMap = null;
	}
}

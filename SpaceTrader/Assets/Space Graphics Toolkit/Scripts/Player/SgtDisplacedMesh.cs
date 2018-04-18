using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(SgtDisplacedMesh))]
public class SgtDisplacedMesh_Editor : SgtEditor<SgtDisplacedMesh>
{
	protected override void OnInspector()
	{
		DrawDefault("OriginalMesh");

		DrawDefault("HeightTex");

		DrawDefault("InnerRadius");

		DrawDefault("OuterRadius");
	}
}
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Displaced Mesh")]
public class SgtDisplacedMesh : MonoBehaviour
{
	[Tooltip("The original mesh we want to displace")]
	public Mesh OriginalMesh;

	[Tooltip("The height map texture used to displace the mesh (Height must be stored in alpha channel)")]
	public Texture2D HeightTex;

	[Tooltip("The mesh radius represented by a 0 alpha value")]
	public float InnerRadius = 0.9f;

	[Tooltip("The mesh radius represented by a 255 alpha value")]
	public float OuterRadius = 1.1f;

	[System.NonSerialized]
	private Mesh displacedMesh;

	[System.NonSerialized]
	private MeshFilter meshFilter;

	// This will return the local terrain height at the given local position
	public float GetSurfaceHeightLocal(Vector3 localPosition)
	{
		if (HeightTex != null)
		{
			var uv       = SgtHelper.CartesianToPolarUV(localPosition);
			var height01 = GetHeightTexAlphaBilinear(uv);

			return Mathf.Lerp(InnerRadius, OuterRadius, height01);
		}

		return 1.0f;
	}

	// Call this if you've made any changes from code and need the mesh to get rebuilt
	[ContextMenu("Rebuild Mesh")]
	public void RebuildMesh()
	{
		if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

		displacedMesh = SgtHelper.Destroy(displacedMesh);

		if (OriginalMesh != null)
		{
#if UNITY_EDITOR
			SgtHelper.MakeTextureReadable(HeightTex);
#endif
			// Duplicate original
			displacedMesh = Instantiate(OriginalMesh);

			displacedMesh.hideFlags = HideFlags.DontSave;

			displacedMesh.name = OriginalMesh.name + " (Displaced)";

			// Displace vertices
			var positions = OriginalMesh.vertices;

			for (var i = 0; i < positions.Length; i++)
			{
				var direction = positions[i].normalized;

				positions[i] = direction * GetSurfaceHeightLocal(direction);
			}

			displacedMesh.vertices = positions;
		}

		meshFilter.sharedMesh = displacedMesh;
	}

	protected virtual void Awake()
	{
		if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

		if (OriginalMesh == null) OriginalMesh = meshFilter.sharedMesh;

		RebuildMesh();
    }

#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		RebuildMesh();
    }
#endif

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireSphere(Vector3.zero, InnerRadius);
		Gizmos.DrawWireSphere(Vector3.zero, OuterRadius);
	}
#endif

	private float GetHeightTexAlphaBilinear(Vector2 uv)
	{
		// Unity biliear
		return HeightTex.GetPixelBilinear(uv.x, uv.y).a;
	}

	private float GetHeightTexAlphaCubic(Vector2 uv)
	{
		int px, py; float fx, fy;

		FracInt(uv.x * HeightTex.width , out px, out fx);
		FracInt(uv.y * HeightTex.height, out py, out fy);

		var w0 = GetPixel(px - 1, py - 1);
		var w1 = GetPixel(px    , py - 1);
		var w2 = GetPixel(px + 1, py - 1);
		var w3 = GetPixel(px + 2, py - 1);
		var x0 = GetPixel(px - 1, py    );
		var x1 = GetPixel(px    , py    );
		var x2 = GetPixel(px + 1, py    );
		var x3 = GetPixel(px + 2, py    );
		var y0 = GetPixel(px - 1, py + 1);
		var y1 = GetPixel(px    , py + 1);
		var y2 = GetPixel(px + 1, py + 1);
		var y3 = GetPixel(px + 2, py + 1);
		var z0 = GetPixel(px - 1, py + 2);
		var z1 = GetPixel(px    , py + 2);
		var z2 = GetPixel(px + 1, py + 2);
		var z3 = GetPixel(px + 2, py + 2);

		var a = Cubic(w0, w1, w2, w3, fx);
		var b = Cubic(x0, x1, x2, x3, fx);
		var c = Cubic(y0, y1, y2, y3, fx);
		var d = Cubic(z0, z1, z2, z3, fx);

		return Cubic(a, b, c, d, fy);
	}

	private float GetHeightTexAlphaCos(Vector2 uv)
	{
		int px, py; float fx, fy;

		FracInt(uv.x * HeightTex.width , out px, out fx);
		FracInt(uv.y * HeightTex.height, out py, out fy);

		var x1 = RepeatX(px);
		var x2 = RepeatX(px + 1);
		var y1 = ClampY(py);
		var y2 = ClampY(py + 1);
		var bl = HeightTex.GetPixel(x1, y1).a;
		var br = HeightTex.GetPixel(x2, y1).a;
		var tl = HeightTex.GetPixel(x1, y2).a;
		var tr = HeightTex.GetPixel(x2, y2).a;

		var b = Coserp(bl, br, fx);
		var t = Coserp(tl, tr, fx);

		return Coserp(b, t, fy);
	}

	private int RepeatX(int x)
	{
		if (x < 0) return HeightTex.width + x;
		if (x >= HeightTex.width) return x - HeightTex.width;

		return x;
	}

	private int ClampY(int y)
	{
		if (y < 0) return 0;
		if (y >= HeightTex.height) return HeightTex.height - 1;

		return y;
	}

	private float Coserp(float a, float b, float t)
	{
		t = (1.0f - Mathf.Cos(t * Mathf.PI)) * 0.5f;

		return a + (b - a) * t;
	}

	public static float Cubic(float a, float b, float c, float d, float t)
	{
		float aSq = t * t;
		d = (d - c) - (a - b);
		return d * (aSq * t) + ((a - b) - d) * aSq + (c - a) * t + b;
	}

	private void FracInt(float v, out int i, out float f)
	{
		i = (int)v;
		f = v - i;
	}

	private float GetPixel(int x, int y)
	{
		return HeightTex.GetPixel(RepeatX(x), ClampY(y)).a;
	}
}

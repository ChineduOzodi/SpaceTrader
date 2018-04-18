using UnityEngine;

// This component draws debug mesh info in the scene window
[RequireComponent(typeof(MeshFilter))]
public class SgtDebugMesh : MonoBehaviour
{
	public float DrawScale = 1.0f;
	
	public Color NormalsColor = Color.red;
	
	public Color TangentsColor = Color.green;
	
	private MeshFilter meshFilter;
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmos()
	{
		if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
		
		var mesh = meshFilter.sharedMesh;
		
		if (mesh != null)
		{
			var positions = mesh.vertices;
			
			if (positions.Length > 0)
			{
				var normals   = mesh.normals;
				var tangents  = mesh.tangents;
				
				Gizmos.matrix = transform.localToWorldMatrix;
				
				if (normals.Length > 0 && normals.Length == positions.Length)
				{
					Gizmos.color = NormalsColor;
					
					for (var i = 0; i < positions.Length; i++)
					{
						var position = positions[i];
						
						Gizmos.DrawLine(position, position + normals[i] * DrawScale);
					}
				}
				
				if (tangents.Length > 0 && tangents.Length == positions.Length)
				{
					Gizmos.color = TangentsColor;
					
					for (var i = 0; i < positions.Length; i++)
					{
						var position = positions[i];
						
						Gizmos.DrawLine(position, position + (Vector3)tangents[i] * DrawScale);
					}
				}
			}
		}
	}
#endif
}
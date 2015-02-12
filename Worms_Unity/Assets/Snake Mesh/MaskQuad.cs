using UnityEngine;
using System.Collections;

public class MaskQuad : MonoBehaviour {
	public float tileSize = 1;
	public float snakeWidth = 0.5f;

	private float val_ = 0;
	public float val {
		get {return val_;}
		set {
			val_ = Mathf.Clamp01(value);
			UpdateMask();
		}
	}

	private BoardDirection direction_ = BoardDirection.Right;
	public BoardDirection direction {
		get {return direction_;}
		set {
			direction_ = value;
			UpdateMask();
		}
	}

	private Vector3 tileOrigin_ = Vector3.zero;
	public Vector3 tileOrigin {
		get {return tileOrigin_;}
		set {
			tileOrigin_ = value;
			UpdateMask();
		}
	}
	
	private bool isDirty = false;
	private MeshFilter meshFilter;

	void Awake() {
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.RecalculateBounds();
		meshFilter.mesh.RecalculateNormals();
	}

	void Start () {
		val = 1;
	}
	
	void Update () {
		if (isDirty) UpdateMask();
	}

	public void UpdateMask() {
		float minVal = ((tileSize - snakeWidth) / 2f) / tileSize;
		float value = Mathf.Clamp(val, 0, 1 - minVal);
	
		Mesh mesh = meshFilter.mesh;

		Vector3 originPoint = tileOrigin;
		if (direction == BoardDirection.Left) originPoint.x -= tileSize;
		if (direction == BoardDirection.Down) originPoint.y -= tileSize;

		Vector3 tileCenter = originPoint + new Vector3(tileSize / 2f, tileSize / 2f, 0);
		Vector3[] verts = new Vector3[4];
		Vector2[] uvs = new Vector2[4];
		int[] tris = new int[6];
		Rect rect = new Rect();

		if (direction == BoardDirection.Right) rect = new Rect(tileCenter.x, originPoint.y, tileSize * value, tileSize);
		else if (direction == BoardDirection.Left) rect = new Rect(tileCenter.x + tileSize - tileSize * value, originPoint.y, tileSize * value, tileSize);
		else if (direction == BoardDirection.Up) rect = new Rect(originPoint.x, tileCenter.y, tileSize, tileSize * value);
		else if (direction == BoardDirection.Down) rect = new Rect(originPoint.x, tileCenter.y + tileSize - tileSize * value, tileSize, tileSize * value);

		verts[0] = new Vector3(rect.x, rect.y, transform.position.z);
		verts[1] = new Vector3(rect.xMax, rect.yMax, transform.position.z);
		verts[2] = new Vector3(rect.xMax, rect.y, transform.position.z);
		verts[3] = new Vector3(rect.x, rect.yMax, transform.position.z);
			
		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(1, 1);
		uvs[2] = new Vector2(1, 0);
		uvs[3] = new Vector2(0, 1);

		tris[0] = 0;
		tris[1] = 1;
		tris[2] = 2;
		tris[3] = 1;
		tris[4] = 0;
		tris[5] = 3;

		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		isDirty = false;
	}
}

using UnityEngine;
using System.Collections;
using System;

public class SnakeMesh : MonoBehaviour {
	public float snakeWidth = 1;
	public float tileSize = 1.6f;
	public int curveResolution = 10;
	public int lineResolution = 1;

	MeshFilter meshFilter;

	void Awake() {
		meshFilter = GetComponent<MeshFilter>();
	}

	void Start () {
		Mesh mesh = meshFilter.mesh;

		Vector3[] verts = new Vector3[0];
		Vector2[] uvs = new Vector2[0];
		int[] tris = new int[0];
		int triIndexBase = 0;
		int vertIndexBase = 0;

//		AddCurve(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, 0, 90, Vector3.zero);
//		AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(-tileSize, 0, 0), BoardDirection.Left);

		AddSegment(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(0, 0, 0), BoardDirection.Up, BoardDirection.Left);
		AddSegment(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(-tileSize, 0, 0), BoardDirection.Left, BoardDirection.Up);
		AddSegment(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(-tileSize, tileSize, 0), BoardDirection.Up, BoardDirection.Up);
		AddSegment(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(-tileSize, tileSize * 2, 0), BoardDirection.Up, BoardDirection.Left);
		AddSegment(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(-tileSize * 2, tileSize * 2, 0), BoardDirection.Left, BoardDirection.Down);
		AddSegment(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(-tileSize * 2, tileSize, 0), BoardDirection.Down, BoardDirection.Down);

		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	void AddSegment(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, Vector3 tileOrigin, BoardDirection direction1, BoardDirection direction2) {
		if (direction1 == direction2.GetOpposite()) Debug.LogError("can't turn around and go back in the same way");
		
		if (direction1 == direction2) {
			AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, tileOrigin, direction1);
		}

		else {
			float baseAngle = 0;
			float curveAngle = 0;
			Vector3 curveOrigin = Vector3.zero;

			if (direction1 == BoardDirection.Down && direction2 == BoardDirection.Right) {
				baseAngle = 180;
				curveAngle = 90;
				curveOrigin = tileOrigin + new Vector3(tileSize, tileSize, 0);
			}

			else if (direction1 == BoardDirection.Left && direction2 == BoardDirection.Up) {
				baseAngle = 270;
				curveAngle = -90;
				curveOrigin = tileOrigin + new Vector3(tileSize, tileSize, 0);
			}

			else if (direction1 == BoardDirection.Left && direction2 == BoardDirection.Down) {
				baseAngle = 90;
				curveAngle = 90;
				curveOrigin = tileOrigin + new Vector3(tileSize, 0, 0);
			}

			else if (direction1 == BoardDirection.Up && direction2 == BoardDirection.Right) {
				baseAngle = 180;
				curveAngle = -90;
				curveOrigin = tileOrigin + new Vector3(tileSize, 0, 0);
			}

			else if (direction1 == BoardDirection.Up && direction2 == BoardDirection.Left) {
				baseAngle = 0;
				curveAngle = 90;
				curveOrigin = tileOrigin;
			}

			else if (direction1 == BoardDirection.Right && direction2 == BoardDirection.Down) {
				baseAngle = 90;
				curveAngle = -90;
				curveOrigin = tileOrigin;
			}

			else if (direction1 == BoardDirection.Right && direction2 == BoardDirection.Up) {
				baseAngle = 270;
				curveAngle = 90;
				curveOrigin = tileOrigin + new Vector3(0, tileSize, 0);
			}

			else if (direction1 == BoardDirection.Down && direction2 == BoardDirection.Left) {
				baseAngle = 0;
				curveAngle = -90;
				curveOrigin = tileOrigin + new Vector3(0, tileSize, 0);
			}

			AddCurve(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, baseAngle, curveAngle, curveOrigin);
		}
	}

	void AddCurve(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, float baseAngle, float curveAngle, Vector2 curveOrigin) {
		float curveAngleInRad = curveAngle * Mathf.Deg2Rad;
		float baseAngleInRad = baseAngle * Mathf.Deg2Rad;

		float r1 = (tileSize - snakeWidth) / 2f;
		float r2 = tileSize - r1;

		Vector3 v1 = Vector3.zero;
		Vector3 v2 = Vector3.zero;
		Vector3 v3 = Vector3.zero;
		Vector3 v4 = Vector3.zero;
		
		for (int i = 0; i < curveResolution; i++) {
			Array.Resize<Vector3>(ref verts, verts.Length + 4);
			Array.Resize<Vector2>(ref uvs, uvs.Length + 4);
			Array.Resize<int>(ref tris, tris.Length + 6);

			float a1 = curveAngleInRad / curveResolution * i;
			float a2 = curveAngleInRad / curveResolution * (i + 1);

			v1.x = Mathf.Cos(a2 + baseAngleInRad) * r1 + curveOrigin.x;
			v1.y = Mathf.Sin(a2 + baseAngleInRad) * r1 + curveOrigin.y;
			
			v2.x = Mathf.Cos(a1 + baseAngleInRad) * r2 + curveOrigin.x;
			v2.y = Mathf.Sin(a1 + baseAngleInRad) * r2 + curveOrigin.y;
			
			v3.x = Mathf.Cos(a2 + baseAngleInRad) * r2 + curveOrigin.x;
			v3.y = Mathf.Sin(a2 + baseAngleInRad) * r2 + curveOrigin.y;

			v4.x = Mathf.Cos(a1 + baseAngleInRad) * r1 + curveOrigin.x;
			v4.y = Mathf.Sin(a1 + baseAngleInRad) * r1 + curveOrigin.y;

			verts[vertIndexBase + 0] = v1;
			verts[vertIndexBase + 1] = v2;
			verts[vertIndexBase + 2] = v3;
			verts[vertIndexBase + 3] = v4;
			
			uvs[vertIndexBase + 0] = new Vector2(0, 0);
			uvs[vertIndexBase + 1] = new Vector2(1, 1);
			uvs[vertIndexBase + 2] = new Vector2(1, 0);
			uvs[vertIndexBase + 3] = new Vector2(0, 1);

			if (curveAngle < 0) {
				tris[triIndexBase + 0] = vertIndexBase + 0;
				tris[triIndexBase + 1] = vertIndexBase + 1;
				tris[triIndexBase + 2] = vertIndexBase + 2;
				tris[triIndexBase + 3] = vertIndexBase + 1;
				tris[triIndexBase + 4] = vertIndexBase + 0;
				tris[triIndexBase + 5] = vertIndexBase + 3;
			}
			else {
				tris[triIndexBase + 0] = vertIndexBase + 3;
				tris[triIndexBase + 1] = vertIndexBase + 0;
				tris[triIndexBase + 2] = vertIndexBase + 1;
				tris[triIndexBase + 3] = vertIndexBase + 2;
				tris[triIndexBase + 4] = vertIndexBase + 1;
				tris[triIndexBase + 5] = vertIndexBase + 0;
			}
			
			triIndexBase += 6;
			vertIndexBase += 4;
		}
	}

	void AddLine(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, Vector3 tileOrigin, BoardDirection direction) {
		Vector3 v1 = Vector3.zero;
		Vector3 v2 = Vector3.zero;
		Vector3 v3 = Vector3.zero;
		Vector3 v4 = Vector3.zero;

		float margin = (tileSize - snakeWidth) / 2f;
		float x1, x2, y1, y2;

		for (int i = 0; i < lineResolution; i++) {
			Array.Resize<Vector3>(ref verts, verts.Length + 4);
			Array.Resize<Vector2>(ref uvs, uvs.Length + 4);
			Array.Resize<int>(ref tris, tris.Length + 6);

			if (direction == BoardDirection.Up) {
				x1 = margin;
				x2 = margin + snakeWidth;
				y1 = tileSize / lineResolution * i;
				y2 = tileSize / lineResolution * (i + 1);

				v1 = new Vector3(x1, y1);
				v2 = new Vector3(x2, y2);
				v3 = new Vector3(x2, y1);
				v4 = new Vector3(x1, y2);
			}

			else if (direction == BoardDirection.Right) {
				x1 = tileSize / lineResolution * i;
				x2 = tileSize / lineResolution * (i + 1);
				y1 = margin;
				y2 = margin + snakeWidth;

				v1 = new Vector3(x1, y2);
				v2 = new Vector3(x2, y1);
				v3 = new Vector3(x1, y1);
				v4 = new Vector3(x2, y2);
			}

			else if (direction == BoardDirection.Down) {
				x1 = margin;
				x2 = margin + snakeWidth;
				y1 = tileSize - (tileSize / lineResolution * i);
				y2 = tileSize - (tileSize / lineResolution * (i + 1));
				
				v1 = new Vector3(x1, y1);
				v2 = new Vector3(x2, y2);
				v3 = new Vector3(x2, y1);
				v4 = new Vector3(x1, y2);
			}

			else if (direction == BoardDirection.Left) {
				x1 = tileSize - (tileSize / lineResolution * i);
				x2 = tileSize - (tileSize / lineResolution * (i + 1));
				y1 = margin;
				y2 = margin + snakeWidth;
				
				v1 = new Vector3(x1, y2);
				v2 = new Vector3(x2, y1);
				v3 = new Vector3(x1, y1);
				v4 = new Vector3(x2, y2);
			}
			
			verts[vertIndexBase + 0] = v1 + tileOrigin;
			verts[vertIndexBase + 1] = v2 + tileOrigin;
			verts[vertIndexBase + 2] = v3 + tileOrigin;
			verts[vertIndexBase + 3] = v4 + tileOrigin;

			uvs[vertIndexBase + 0] = new Vector2(0, 0);
			uvs[vertIndexBase + 1] = new Vector2(1, 1);
			uvs[vertIndexBase + 2] = new Vector2(1, 0);
			uvs[vertIndexBase + 3] = new Vector2(0, 1);
			
			if (direction == BoardDirection.Up || direction == BoardDirection.Right) {
				tris[triIndexBase + 0] = vertIndexBase + 0;
				tris[triIndexBase + 1] = vertIndexBase + 1;
				tris[triIndexBase + 2] = vertIndexBase + 2;
				tris[triIndexBase + 3] = vertIndexBase + 1;
				tris[triIndexBase + 4] = vertIndexBase + 0;
				tris[triIndexBase + 5] = vertIndexBase + 3;
			}
			else {
				tris[triIndexBase + 0] = vertIndexBase + 3;
				tris[triIndexBase + 1] = vertIndexBase + 0;
				tris[triIndexBase + 2] = vertIndexBase + 1;
				tris[triIndexBase + 3] = vertIndexBase + 2;
				tris[triIndexBase + 4] = vertIndexBase + 1;
				tris[triIndexBase + 5] = vertIndexBase + 0;
			}
			
			triIndexBase += 6;
			vertIndexBase += 4;
		}
	}
	
	void Update () {
	
	}
}

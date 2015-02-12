using UnityEngine;
using System.Collections;
using System;

public class Snake : MonoBehaviour {
	public float snakeWidth = 1;
	public float tileSize = 1.6f;
	public int curveResolution = 10;
	public int lineResolution = 1;
	public float curveRadius = 0.2f;
	public Transform snakeFrontPrefab;
	public Transform snakeBackPrefab;
	public MaskQuad maskQuadPrefab;

	private MaskQuad maskQuad;
	private Transform snakeFront;
	private Transform snakeBack;
	private int triIndexBase = 0;
	private int vertIndexBase = 0;
	private Vector3 tileOrigin = Vector3.zero;
	private float spokeSize;
	private BoardDirection previousDirection = BoardDirection.NONE;
	private MeshFilter meshFilter;
	private bool hasMoved = false;

	void Awake() {
		meshFilter = GetComponent<MeshFilter>();
	}

	void Start () {
		spokeSize = (tileSize - snakeWidth) / 2f;

		Mesh mesh = new Mesh();
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		snakeFront = Instantiate(snakeFrontPrefab) as Transform;
		snakeBack = Instantiate(snakeBackPrefab) as Transform;

		snakeFront.parent = transform;
		snakeBack.parent = transform;

		snakeFront.position = new Vector3(tileSize / 2f, tileSize / 2f, 0);
		snakeBack.position = new Vector3(tileSize / 2f, tileSize / 2f, 0);

		maskQuad = Instantiate(maskQuadPrefab) as MaskQuad;
		maskQuad.transform.parent = transform;
	}

	void Update () {
		BoardDirection direction = BoardDirection.NONE;
		
		if (Input.GetKeyDown(KeyCode.UpArrow)) direction = BoardDirection.Up;
		else if (Input.GetKeyDown(KeyCode.RightArrow)) direction = BoardDirection.Right;
		else if (Input.GetKeyDown(KeyCode.DownArrow)) direction = BoardDirection.Down;
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = BoardDirection.Left;
		
		if (direction != BoardDirection.NONE && direction != previousDirection.GetOpposite()) Move(direction);
	}

	void Move(BoardDirection direction) {
		if (!hasMoved) {
			snakeFront.rotation = direction.ToRotation();
			snakeBack.rotation = direction.ToRotation();

			hasMoved = true;
		}

		BoardDirection direction1 = previousDirection;
		BoardDirection direction2 = direction;

		if (direction1 == direction2.GetOpposite()) Debug.LogError("can't turn around and go back in the same way");
		if (direction2 == BoardDirection.NONE) Debug.LogError("no direction to move towards!");

		Mesh mesh = meshFilter.mesh;
		
		Vector3[] verts = mesh.vertices;
		Vector2[] uvs = mesh.uv;
		int[] tris = mesh.triangles;
		float normalizedSpokeSize = spokeSize / tileSize;
		
		if (direction1 == BoardDirection.NONE) {
			direction1 = direction2;
			AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, 0.5f, 1, tileOrigin, direction2);
		}
		
		else if (direction1 == direction2) {
			AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, normalizedSpokeSize, 1, tileOrigin, direction2);
		}
		
		else {
			AddTurn(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, tileOrigin, direction1, direction2);
			AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, 1 - normalizedSpokeSize, 1, tileOrigin, direction2);
		}

		UpdateTileOrigin(ref tileOrigin, direction2);
		AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, 0, 0.5f, tileOrigin, direction2);
	
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		float x = 0;
		float y = 0;

		x = tileSize / 2f;
		y = tileSize / 2f;
		snakeFront.rotation = direction.ToRotation();

		maskQuad.val = 1;
		maskQuad.direction = direction2.GetOpposite();
		maskQuad.tileOrigin = tileOrigin;

		Go.to(snakeFront, 0.15f, new GoTweenConfig().setEaseType(GoEaseType.SineInOut).position(new Vector3(tileOrigin.x + x, tileOrigin.y + y, 0)));
		Go.to(maskQuad, 0.15f, new GoTweenConfig().setEaseType(GoEaseType.SineInOut).floatProp("val", 0));
	}

	void UpdateTileOrigin(ref Vector3 tileOrigin, BoardDirection newPreviousDirection) {
		previousDirection = newPreviousDirection;
		IntVector2 dirIntVect = previousDirection.ToIntVector2();
		Vector3 dirVect = new Vector3(dirIntVect.x, dirIntVect.y, 0);
		tileOrigin += new Vector3(tileSize * dirVect.x, tileSize * dirVect.y, 0);
	}

	void AddTurn(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, Vector2 tileOrigin, BoardDirection direction1, BoardDirection direction2) {
		float margin = (tileSize - snakeWidth) / 2f;
		float baseAngle = 0;
		float curveAngle = 0;
		Vector3 curveOrigin = Vector3.zero;
		Rect rect1 = new Rect();
		Rect rect2 = new Rect();
		Rect rect3 = new Rect();
		
		if (direction1 == BoardDirection.Down && direction2 == BoardDirection.Right) {
			baseAngle = 180;
			curveAngle = 90;
			curveOrigin = tileOrigin + new Vector2(tileSize, tileSize);
			rect1 = new Rect(margin + curveRadius, margin + curveRadius, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(margin, margin + curveRadius, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, margin, snakeWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Left && direction2 == BoardDirection.Up) {
			baseAngle = 270;
			curveAngle = -90;
			curveOrigin = tileOrigin + new Vector2(tileSize, tileSize);
			rect1 = new Rect(margin + curveRadius, margin + curveRadius, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(margin, margin + curveRadius, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, margin, snakeWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Left && direction2 == BoardDirection.Down) {
			baseAngle = 90;
			curveAngle = 90;
			curveOrigin = tileOrigin + new Vector2(tileSize, 0);
			rect1 = new Rect(margin + curveRadius, spokeSize, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(margin, spokeSize, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, spokeSize + snakeWidth - curveRadius, snakeWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Up && direction2 == BoardDirection.Right) {
			baseAngle = 180;
			curveAngle = -90;
			curveOrigin = tileOrigin + new Vector2(tileSize, 0);
			rect1 = new Rect(margin + curveRadius, spokeSize, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(margin, spokeSize, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, spokeSize + snakeWidth - curveRadius, snakeWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Up && direction2 == BoardDirection.Left) {
			baseAngle = 0;
			curveAngle = 90;
			curveOrigin = tileOrigin;
			rect1 = new Rect(spokeSize, spokeSize, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(spokeSize + snakeWidth - curveRadius, spokeSize, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(spokeSize, spokeSize + snakeWidth - curveRadius, snakeWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Right && direction2 == BoardDirection.Down) {
			baseAngle = 90;
			curveAngle = -90;
			curveOrigin = tileOrigin;
			rect1 = new Rect(spokeSize, spokeSize, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(spokeSize + snakeWidth - curveRadius, spokeSize, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(spokeSize, spokeSize + snakeWidth - curveRadius, snakeWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Right && direction2 == BoardDirection.Up) {
			baseAngle = 270;
			curveAngle = 90;
			curveOrigin = tileOrigin + new Vector2(0, tileSize);
			rect1 = new Rect(spokeSize, margin + curveRadius, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(spokeSize + snakeWidth - curveRadius, margin + curveRadius, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(spokeSize, margin, snakeWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Down && direction2 == BoardDirection.Left) {
			baseAngle = 0;
			curveAngle = -90;
			curveOrigin = tileOrigin + new Vector2(0, tileSize);
			rect1 = new Rect(spokeSize, margin + curveRadius, snakeWidth - curveRadius, snakeWidth - curveRadius);
			rect2 = new Rect(spokeSize + snakeWidth - curveRadius, margin + curveRadius, curveRadius, snakeWidth - curveRadius);
			rect3 = new Rect(spokeSize, margin, snakeWidth - curveRadius, curveRadius);
		}

		AddRect(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, rect1, tileOrigin);
		AddRect(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, rect2, tileOrigin);
		AddRect(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, rect3, tileOrigin);
		
		Vector2 dirVect1 = new Vector2(direction1.ToIntVector2().x, direction1.ToIntVector2().y);
		Vector2 dirVect2 = new Vector2(direction2.GetOpposite().ToIntVector2().x, direction2.GetOpposite().ToIntVector2().y);
		curveOrigin += new Vector3((spokeSize + (snakeWidth - curveRadius)) * (dirVect1.x + dirVect2.x), (spokeSize + (snakeWidth - curveRadius)) * (dirVect1.y + dirVect2.y), 0);

		AddCurve(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, curveOrigin, curveRadius, curveAngle, baseAngle);
	}

	void AddCurve(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, Vector3 curveOrigin, float radius, float curveAngle, float baseAngle) {
		float curveAngleInRad = curveAngle * Mathf.Deg2Rad;
		float baseAngleInRad = baseAngle * Mathf.Deg2Rad;
		
		float r1 = 0;
		float r2 = radius;
		
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

	void AddRect(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, Rect rect, Vector3 tileOrigin) {
		Array.Resize<Vector3>(ref verts, verts.Length + 4);
		Array.Resize<Vector2>(ref uvs, uvs.Length + 4);
		Array.Resize<int>(ref tris, tris.Length + 6);
		
		Vector3 v1 = new Vector3(rect.x, rect.y);
		Vector3 v2 = new Vector3(rect.xMax, rect.yMax);
		Vector3 v3 = new Vector3(rect.xMax, rect.y);
		Vector3 v4 = new Vector3(rect.x, rect.yMax);
	
		verts[vertIndexBase + 0] = v1 + tileOrigin;
		verts[vertIndexBase + 1] = v2 + tileOrigin;
		verts[vertIndexBase + 2] = v3 + tileOrigin;
		verts[vertIndexBase + 3] = v4 + tileOrigin;

		float minUV = 0.3f;
		float maxUV = 1 - minUV;
		uvs[vertIndexBase + 0] = new Vector2(minUV, minUV);
		uvs[vertIndexBase + 1] = new Vector2(maxUV, maxUV);
		uvs[vertIndexBase + 2] = new Vector2(maxUV, minUV);
		uvs[vertIndexBase + 3] = new Vector2(minUV, maxUV);
		
		tris[triIndexBase + 0] = vertIndexBase + 0;
		tris[triIndexBase + 1] = vertIndexBase + 1;
		tris[triIndexBase + 2] = vertIndexBase + 2;
		tris[triIndexBase + 3] = vertIndexBase + 1;
		tris[triIndexBase + 4] = vertIndexBase + 0;
		tris[triIndexBase + 5] = vertIndexBase + 3;

		triIndexBase += 6;
		vertIndexBase += 4;
	}

	void AddLine(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, float startPoint, float endPoint, Vector3 tileOrigin, BoardDirection direction) {
		Vector3 v1 = Vector3.zero;
		Vector3 v2 = Vector3.zero;
		Vector3 v3 = Vector3.zero;
		Vector3 v4 = Vector3.zero;

		float margin = (tileSize - snakeWidth) / 2f;
		float x1, x2, y1, y2;
		int newLineRes = Mathf.Max(1, (int)((float)lineResolution * (endPoint - startPoint)));

		for (int i = 0; i < newLineRes; i++) {
			Array.Resize<Vector3>(ref verts, verts.Length + 4);
			Array.Resize<Vector2>(ref uvs, uvs.Length + 4);
			Array.Resize<int>(ref tris, tris.Length + 6);

			if (direction == BoardDirection.Up) {
				x1 = margin;
				x2 = margin + snakeWidth;
				y1 = tileSize / lineResolution * i + startPoint * tileSize;
				y2 = tileSize / lineResolution * (i + 1) - (1 - endPoint) * tileSize;

				v1 = new Vector3(x1, y1);
				v2 = new Vector3(x2, y2);
				v3 = new Vector3(x2, y1);
				v4 = new Vector3(x1, y2);
			}

			else if (direction == BoardDirection.Right) {
				x1 = tileSize / lineResolution * i + startPoint * tileSize;
				x2 = tileSize / lineResolution * (i + 1) - (1 - endPoint) * tileSize;
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
				y1 = tileSize - (tileSize / lineResolution * i) - startPoint * tileSize;
				y2 = tileSize - (tileSize / lineResolution * (i + 1)) + (1 - endPoint) * tileSize;
				
				v1 = new Vector3(x1, y1);
				v2 = new Vector3(x2, y2);
				v3 = new Vector3(x2, y1);
				v4 = new Vector3(x1, y2);
			}

			else if (direction == BoardDirection.Left) {
				x1 = tileSize - (tileSize / lineResolution * i) - startPoint * tileSize;
				x2 = tileSize - (tileSize / lineResolution * (i + 1)) + (1 - endPoint) * tileSize;
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
}

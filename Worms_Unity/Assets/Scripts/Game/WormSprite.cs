using UnityEngine;
using System.Collections;
using System;

public class WormSprite : MonoBehaviour {
	public int curveResolution = 10;
	public int lineResolution = 1;
	public float curveRadius = 0.2f;
	public tk2dSprite wormHeadPrefab;
	public tk2dSprite wormButtPrefab;
	public MaskQuad maskQuadPrefab;
	public BoardDirection previousDirection = BoardDirection.NONE;

	private MaskQuad maskQuad;
	private tk2dSprite wormHead;
	private tk2dSprite wormButt;
	private int triIndexBase = 0;
	private int vertIndexBase = 0;
	private int triIndexBaseAtMoveStart = 0;
	private int vertIndexBaseAtMoveStart = 0;
	private Vector3 currentTileOrigin = Vector3.zero;
	private Vector3 newTileOrigin = Vector3.zero;
	private float spokeSize;
	private BoardDirection currentMove = BoardDirection.NONE;
	private MeshFilter meshFilter;
	private bool hasMoved = false;

	void Awake() {
		meshFilter = GetComponent<MeshFilter>();

		maskQuad = Instantiate(maskQuadPrefab) as MaskQuad;
	}

	void Start () {
		spokeSize = (Board.tileSize - Worm.wormWidth) / 2f;

		Mesh mesh = new Mesh();
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		maskQuad.transform.parent = transform;
		maskQuad.transform.localPosition = new Vector3(-Board.tileSize / 2f, -Board.tileSize / 2f, -1);

//		Vector3[] verts = mesh.vertices;
//		Vector2[] uvs = mesh.uv;
//		int[] tris = mesh.triangles;
//
//		AddCurve(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, new Vector3(tileSize / 2f, tileSize / 2f, 0), curveRadius, 360, 0);
//
//		mesh.vertices = verts;
//		mesh.uv = uvs;
//		mesh.triangles = tris;
//
//		mesh.RecalculateBounds();
//		mesh.RecalculateNormals();
//
//		UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Snake Mesh/SnakeFrontFull.asset");
	}

	public void SetColor(Color color) {
		renderer.material.color = color;

		if (wormHead == null || wormButt == null) {
			wormHead = Instantiate(wormHeadPrefab) as tk2dSprite;
			wormButt = Instantiate(wormButtPrefab) as tk2dSprite;
			
			wormHead.transform.parent = transform;
			wormButt.transform.parent = transform;
			
			wormHead.transform.localPosition = Vector3.zero;
			wormButt.transform.localPosition = Vector3.zero;
		}

		wormHead.color = color;
		wormButt.color = color;
	}

	public void CancelMove() {
		if (previousDirection != BoardDirection.NONE) Go.to(wormHead.transform, 0.15f, new GoTweenConfig().setEaseType(GoEaseType.SineInOut).rotation(previousDirection.ToRotation()));

		currentMove = BoardDirection.NONE;
		maskQuad.direction = previousDirection.GetOpposite();
		maskQuad.direction = previousDirection.GetOpposite();

		wormHead.transform.localPosition = new Vector3(currentTileOrigin.x, currentTileOrigin.y, 0);
		newTileOrigin = Vector3.zero;
		RemoveUnusedVerts();
	}

	void RemoveUnusedVerts() {
		Mesh mesh = meshFilter.mesh;

		Vector3[] verts = mesh.vertices;
		Vector2[] uvs = mesh.uv;
		int[] tris = mesh.triangles;

		Array.Resize<Vector3>(ref verts, vertIndexBaseAtMoveStart);
		Array.Resize<Vector2>(ref uvs, vertIndexBaseAtMoveStart);
		Array.Resize<int>(ref tris, triIndexBaseAtMoveStart);

		if (verts.Length == 0) {
			Array.Resize<Vector3>(ref verts, 4);
			Array.Resize<Vector2>(ref uvs, 4);
			Array.Resize<int>(ref tris, 6);

			verts[0] = Vector3.zero;
			verts[1] = Vector3.zero;
			verts[2] = Vector3.zero;
			verts[3] = Vector3.zero;

			uvs[0] = Vector2.zero;
			uvs[1] = Vector2.zero;
			uvs[2] = Vector2.zero;
			uvs[3] = Vector2.zero;

			tris[0] = 0;
			tris[1] = 1;
			tris[2] = 2;
			tris[3] = 1;
			tris[4] = 0;
			tris[5] = 3;

			vertIndexBase = 4;
			triIndexBase = 6;
		}
		else {
			triIndexBase = triIndexBaseAtMoveStart;
			vertIndexBase = vertIndexBaseAtMoveStart;
		}

		mesh.triangles = tris;
		mesh.vertices = verts;
		mesh.uv = uvs;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		maskQuad.val = 1;
	}

	public void ContinueMove(float normalizedVal) {
		Vector3 snakeFrontFrom = new Vector3(currentTileOrigin.x, currentTileOrigin.y, 0);
		Vector3 snakeFrontTo = new Vector3(newTileOrigin.x, newTileOrigin.y, 0);
		Vector3 snakeFrontPos = Vector3.Lerp(snakeFrontFrom, snakeFrontTo, normalizedVal);
		wormHead.transform.localPosition = snakeFrontPos;
		maskQuad.val = normalizedVal;
	}

	public void CommitMove() {
		hasMoved = true;
		previousDirection = currentMove;
		currentMove = BoardDirection.NONE;
		UpdateTileOrigin(ref currentTileOrigin, previousDirection);
		wormHead.transform.localPosition = new Vector3(currentTileOrigin.x, currentTileOrigin.y, 0);
		maskQuad.val = 1;
	}
	
	public void StartMove(BoardDirection direction) {
		if (!hasMoved) {
			Go.to(wormHead.transform, 0.15f, new GoTweenConfig().setEaseType(GoEaseType.SineInOut).rotation(direction.ToRotation()));
			wormButt.transform.rotation = direction.ToRotation();
		}

		newTileOrigin = currentTileOrigin;
		triIndexBaseAtMoveStart = triIndexBase;
		vertIndexBaseAtMoveStart = vertIndexBase;

		currentMove = direction;

		BoardDirection direction1 = previousDirection;
		BoardDirection direction2 = currentMove;

		if (direction1 == direction2.GetOpposite()) Debug.LogError("can't turn around and go back in the same way");
		if (direction2 == BoardDirection.NONE) Debug.LogError("no direction to move towards!");

		Mesh mesh = meshFilter.mesh;
		
		Vector3[] verts = mesh.vertices;
		Vector2[] uvs = mesh.uv;
		int[] tris = mesh.triangles;
		float normalizedSpokeSize = spokeSize / Board.tileSize;

		if (direction1 == BoardDirection.NONE) {
			direction1 = direction2;
			AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, 0.5f, 1, newTileOrigin, direction2);
		}
		
		else if (direction1 == direction2) {
			AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, normalizedSpokeSize, 1, newTileOrigin, direction2);
		}
		
		else {
			AddTurn(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, newTileOrigin, direction1, direction2);
			AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, 1 - normalizedSpokeSize, 1, newTileOrigin, direction2);
		}

		UpdateTileOrigin(ref newTileOrigin, direction2);
		AddLine(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, 0, 0.5f, newTileOrigin, direction2);
	
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		Go.to(wormHead.transform, 0.15f, new GoTweenConfig().setEaseType(GoEaseType.SineInOut).rotation(direction.ToRotation()));

		maskQuad.direction = direction2.GetOpposite();
		maskQuad.tileOrigin = newTileOrigin;
		maskQuad.val = 0;
	}

	void UpdateTileOrigin(ref Vector3 tileOrigin, BoardDirection direction) {
		IntVector2 dirIntVect = direction.ToIntVector2();
		Vector3 dirVect = new Vector3(dirIntVect.x, dirIntVect.y, 0);
		tileOrigin += new Vector3(Board.tileSize * dirVect.x, Board.tileSize * dirVect.y, 0);
	}

	void AddTurn(ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int triIndexBase, ref int vertIndexBase, Vector2 tileOrigin, BoardDirection direction1, BoardDirection direction2) {
		float margin = (Board.tileSize - Worm.wormWidth) / 2f;
		float baseAngle = 0;
		float curveAngle = 0;
		Vector3 curveOrigin = Vector3.zero;
		Rect rect1 = new Rect();
		Rect rect2 = new Rect();
		Rect rect3 = new Rect();
		
		if (direction1 == BoardDirection.Down && direction2 == BoardDirection.Right) {
			baseAngle = 180;
			curveAngle = 90;
			curveOrigin = tileOrigin + new Vector2(Board.tileSize, Board.tileSize);
			rect1 = new Rect(margin + curveRadius, margin + curveRadius, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(margin, margin + curveRadius, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, margin, Worm.wormWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Left && direction2 == BoardDirection.Up) {
			baseAngle = 270;
			curveAngle = -90;
			curveOrigin = tileOrigin + new Vector2(Board.tileSize, Board.tileSize);
			rect1 = new Rect(margin + curveRadius, margin + curveRadius, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(margin, margin + curveRadius, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, margin, Worm.wormWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Left && direction2 == BoardDirection.Down) {
			baseAngle = 90;
			curveAngle = 90;
			curveOrigin = tileOrigin + new Vector2(Board.tileSize, 0);
			rect1 = new Rect(margin + curveRadius, spokeSize, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(margin, spokeSize, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, spokeSize + Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Up && direction2 == BoardDirection.Right) {
			baseAngle = 180;
			curveAngle = -90;
			curveOrigin = tileOrigin + new Vector2(Board.tileSize, 0);
			rect1 = new Rect(margin + curveRadius, spokeSize, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(margin, spokeSize, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(margin + curveRadius, spokeSize + Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Up && direction2 == BoardDirection.Left) {
			baseAngle = 0;
			curveAngle = 90;
			curveOrigin = tileOrigin;
			rect1 = new Rect(spokeSize, spokeSize, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(spokeSize + Worm.wormWidth - curveRadius, spokeSize, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(spokeSize, spokeSize + Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Right && direction2 == BoardDirection.Down) {
			baseAngle = 90;
			curveAngle = -90;
			curveOrigin = tileOrigin;
			rect1 = new Rect(spokeSize, spokeSize, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(spokeSize + Worm.wormWidth - curveRadius, spokeSize, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(spokeSize, spokeSize + Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Right && direction2 == BoardDirection.Up) {
			baseAngle = 270;
			curveAngle = 90;
			curveOrigin = tileOrigin + new Vector2(0, Board.tileSize);
			rect1 = new Rect(spokeSize, margin + curveRadius, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(spokeSize + Worm.wormWidth - curveRadius, margin + curveRadius, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(spokeSize, margin, Worm.wormWidth - curveRadius, curveRadius);
		}
		
		else if (direction1 == BoardDirection.Down && direction2 == BoardDirection.Left) {
			baseAngle = 0;
			curveAngle = -90;
			curveOrigin = tileOrigin + new Vector2(0, Board.tileSize);
			rect1 = new Rect(spokeSize, margin + curveRadius, Worm.wormWidth - curveRadius, Worm.wormWidth - curveRadius);
			rect2 = new Rect(spokeSize + Worm.wormWidth - curveRadius, margin + curveRadius, curveRadius, Worm.wormWidth - curveRadius);
			rect3 = new Rect(spokeSize, margin, Worm.wormWidth - curveRadius, curveRadius);
		}

		AddRect(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, rect1, tileOrigin);
		AddRect(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, rect2, tileOrigin);
		AddRect(ref verts, ref uvs, ref tris, ref triIndexBase, ref vertIndexBase, rect3, tileOrigin);
		
		Vector2 dirVect1 = new Vector2(direction1.ToIntVector2().x, direction1.ToIntVector2().y);
		Vector2 dirVect2 = new Vector2(direction2.GetOpposite().ToIntVector2().x, direction2.GetOpposite().ToIntVector2().y);
		curveOrigin += new Vector3((spokeSize + (Worm.wormWidth - curveRadius)) * (dirVect1.x + dirVect2.x), (spokeSize + (Worm.wormWidth - curveRadius)) * (dirVect1.y + dirVect2.y), 0);

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
			
			v1.x = Mathf.Cos(a2 + baseAngleInRad) * r1 + curveOrigin.x - Board.tileSize / 2f;
			v1.y = Mathf.Sin(a2 + baseAngleInRad) * r1 + curveOrigin.y - Board.tileSize / 2f;
			
			v2.x = Mathf.Cos(a1 + baseAngleInRad) * r2 + curveOrigin.x - Board.tileSize / 2f;
			v2.y = Mathf.Sin(a1 + baseAngleInRad) * r2 + curveOrigin.y - Board.tileSize / 2f;
			
			v3.x = Mathf.Cos(a2 + baseAngleInRad) * r2 + curveOrigin.x - Board.tileSize / 2f;
			v3.y = Mathf.Sin(a2 + baseAngleInRad) * r2 + curveOrigin.y - Board.tileSize / 2f;
			
			v4.x = Mathf.Cos(a1 + baseAngleInRad) * r1 + curveOrigin.x - Board.tileSize / 2f;
			v4.y = Mathf.Sin(a1 + baseAngleInRad) * r1 + curveOrigin.y - Board.tileSize / 2f;
			
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
		
		Vector3 v1 = new Vector3(rect.x - Board.tileSize / 2f, rect.y - Board.tileSize / 2f);
		Vector3 v2 = new Vector3(rect.xMax - Board.tileSize / 2f, rect.yMax - Board.tileSize / 2f);
		Vector3 v3 = new Vector3(rect.xMax - Board.tileSize / 2f, rect.y - Board.tileSize / 2f);
		Vector3 v4 = new Vector3(rect.x - Board.tileSize / 2f, rect.yMax - Board.tileSize / 2f);
	
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

		float margin = (Board.tileSize - Worm.wormWidth) / 2f;
		float x1, x2, y1, y2;
		int newLineRes = Mathf.Max(1, (int)((float)lineResolution * (endPoint - startPoint)));

		for (int i = 0; i < newLineRes; i++) {
			Array.Resize<Vector3>(ref verts, verts.Length + 4);
			Array.Resize<Vector2>(ref uvs, uvs.Length + 4);
			Array.Resize<int>(ref tris, tris.Length + 6);

			if (direction == BoardDirection.Up) {
				x1 = margin;
				x2 = margin + Worm.wormWidth;
				y1 = Board.tileSize / lineResolution * i + startPoint * Board.tileSize;
				y2 = Board.tileSize / lineResolution * (i + 1) - (1 - endPoint) * Board.tileSize;

				v1 = new Vector3(x1, y1);
				v2 = new Vector3(x2, y2);
				v3 = new Vector3(x2, y1);
				v4 = new Vector3(x1, y2);
			}

			else if (direction == BoardDirection.Right) {
				x1 = Board.tileSize / lineResolution * i + startPoint * Board.tileSize;
				x2 = Board.tileSize / lineResolution * (i + 1) - (1 - endPoint) * Board.tileSize;
				y1 = margin;
				y2 = margin + Worm.wormWidth;

				v1 = new Vector3(x1, y2);
				v2 = new Vector3(x2, y1);
				v3 = new Vector3(x1, y1);
				v4 = new Vector3(x2, y2);
			}

			else if (direction == BoardDirection.Down) {
				x1 = margin;
				x2 = margin + Worm.wormWidth;
				y1 = Board.tileSize - (Board.tileSize / lineResolution * i) - startPoint * Board.tileSize;
				y2 = Board.tileSize - (Board.tileSize / lineResolution * (i + 1)) + (1 - endPoint) * Board.tileSize;
				
				v1 = new Vector3(x1, y1);
				v2 = new Vector3(x2, y2);
				v3 = new Vector3(x2, y1);
				v4 = new Vector3(x1, y2);
			}

			else if (direction == BoardDirection.Left) {
				x1 = Board.tileSize - (Board.tileSize / lineResolution * i) - startPoint * Board.tileSize;
				x2 = Board.tileSize - (Board.tileSize / lineResolution * (i + 1)) + (1 - endPoint) * Board.tileSize;
				y1 = margin;
				y2 = margin + Worm.wormWidth;
				
				v1 = new Vector3(x1, y2);
				v2 = new Vector3(x2, y1);
				v3 = new Vector3(x1, y1);
				v4 = new Vector3(x2, y2);
			}

			v1.x -= Board.tileSize / 2f;
			v1.y -= Board.tileSize / 2f;
			v2.x -= Board.tileSize / 2f;
			v2.y -= Board.tileSize / 2f;
			v3.x -= Board.tileSize / 2f;
			v3.y -= Board.tileSize / 2f;
			v4.x -= Board.tileSize / 2f;
			v4.y -= Board.tileSize / 2f;
			
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

using UnityEngine;
using System.Collections;

public class MaskQuad : MonoBehaviour {
	private float val_ = 1;
	public float val {
		get {return val_;}
		set {
			val_ = Mathf.Clamp01(value);
			UpdateMask();
		}
	}

	private BoardDirection direction_ = BoardDirection.NONE;
	public BoardDirection direction {
		get {return direction_;}
		set {
			previousDirection = direction_;
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
	public BoardDirection previousDirection = BoardDirection.NONE;

	void Awake() {
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.RecalculateBounds();
		meshFilter.mesh.RecalculateNormals();
	}

	void Start () {
		val = 0;
	}
	
	void Update () {
		if (isDirty) UpdateMask();
	}

	public void UpdateMask() {
		Mesh mesh = meshFilter.mesh;
		Vector3 originPoint = tileOrigin;
		int vertIndex = 0;
		int triIndex = 0;
		BoardDirection dir = direction.GetOpposite();

		Vector3 tileCenter = originPoint + new Vector3(Board.tileSize / 2f, Board.tileSize / 2f, 0);
		Vector3[] verts = new Vector3[8];
		Vector2[] uvs = new Vector2[8];
		int[] tris = new int[12];
		Rect mainRect = new Rect();
		Rect smallRect = new Rect();

		float margin = (Board.tileSize - Worm.wormWidth) / 2f;
		float invertedVal = 1 - val;
		float maxVal = 1 - ((Board.tileSize - Worm.wormWidth) / 2f) / Board.tileSize;
		if (previousDirection == direction || previousDirection == BoardDirection.NONE) maxVal = 1;

		float mainRectVal = Mathf.Clamp(invertedVal, 0, maxVal);
		float smallRectVal = 1;
		if (mainRectVal >= maxVal) smallRectVal = 1 - (((1 - maxVal) - Mathf.Min(invertedVal, 1 - invertedVal, 1 - maxVal)) / (1 - maxVal));

		// main rect
		if (dir == BoardDirection.Left) mainRect = new Rect(tileCenter.x, originPoint.y + margin, Board.tileSize * mainRectVal, Worm.wormWidth);
		else if (dir == BoardDirection.Right) mainRect = new Rect(tileCenter.x - Board.tileSize * mainRectVal, originPoint.y + margin, Board.tileSize * mainRectVal, Worm.wormWidth);
		else if (dir == BoardDirection.Down) mainRect = new Rect(originPoint.x + margin, tileCenter.y, Worm.wormWidth, Board.tileSize * mainRectVal);
		else if (dir == BoardDirection.Up) mainRect = new Rect(originPoint.x + margin, tileCenter.y - Board.tileSize * mainRectVal, Worm.wormWidth, Board.tileSize * mainRectVal);

		verts[vertIndex + 0] = new Vector3(mainRect.x, mainRect.y, transform.position.z);
		verts[vertIndex + 1] = new Vector3(mainRect.xMax, mainRect.yMax, transform.position.z);
		verts[vertIndex + 2] = new Vector3(mainRect.xMax, mainRect.y, transform.position.z);
		verts[vertIndex + 3] = new Vector3(mainRect.x, mainRect.yMax, transform.position.z);
			
		uvs[vertIndex + 0] = new Vector2(0, 0);
		uvs[vertIndex + 1] = new Vector2(1, 1);
		uvs[vertIndex + 2] = new Vector2(1, 0);
		uvs[vertIndex + 3] = new Vector2(0, 1);

		tris[triIndex + 0] = vertIndex + 0;
		tris[triIndex + 1] = vertIndex + 1;
		tris[triIndex + 2] = vertIndex + 2;
		tris[triIndex + 3] = vertIndex + 1;
		tris[triIndex + 4] = vertIndex + 0;
		tris[triIndex + 5] = vertIndex + 3;

		vertIndex += 4;
		triIndex += 6;

		// small rect
		Vector3 adjustedCenter = tileCenter;
		if (previousDirection == BoardDirection.Right) adjustedCenter.x -= Worm.wormWidth / 2f;
		else if (previousDirection == BoardDirection.Up) adjustedCenter.y -= Worm.wormWidth / 2f;

		if (dir == BoardDirection.Left) smallRect = new Rect(adjustedCenter.x + Board.tileSize - Worm.wormWidth / 2f, adjustedCenter.y, Worm.wormWidth / 2f * (1 - smallRectVal), Worm.wormWidth / 2f);
		else if (dir == BoardDirection.Right) smallRect = new Rect(adjustedCenter.x - Board.tileSize + smallRectVal * Worm.wormWidth / 2f, adjustedCenter.y, Worm.wormWidth / 2f * (1 - smallRectVal), Worm.wormWidth / 2f);
		else if (dir == BoardDirection.Down) smallRect = new Rect(adjustedCenter.x, adjustedCenter.y + Board.tileSize - Worm.wormWidth / 2f, Worm.wormWidth / 2f, Worm.wormWidth / 2f * (1 - smallRectVal));
		else if (dir == BoardDirection.Up) smallRect = new Rect(adjustedCenter.x, adjustedCenter.y - Board.tileSize + smallRectVal * Worm.wormWidth / 2f, Worm.wormWidth / 2f, Worm.wormWidth / 2f * (1 - smallRectVal));

		verts[vertIndex + 0] = new Vector3(smallRect.x, smallRect.y, transform.position.z);
		verts[vertIndex + 1] = new Vector3(smallRect.xMax, smallRect.yMax, transform.position.z);
		verts[vertIndex + 2] = new Vector3(smallRect.xMax, smallRect.y, transform.position.z);
		verts[vertIndex + 3] = new Vector3(smallRect.x, smallRect.yMax, transform.position.z);
		
		uvs[vertIndex + 0] = new Vector2(0, 0);
		uvs[vertIndex + 1] = new Vector2(1, 1);
		uvs[vertIndex + 2] = new Vector2(1, 0);
		uvs[vertIndex + 3] = new Vector2(0, 1);
		
		tris[triIndex + 0] = vertIndex + 0;
		tris[triIndex + 1] = vertIndex + 1;
		tris[triIndex + 2] = vertIndex + 2;
		tris[triIndex + 3] = vertIndex + 1;
		tris[triIndex + 4] = vertIndex + 0;
		tris[triIndex + 5] = vertIndex + 3;

		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		isDirty = false;
	}
}

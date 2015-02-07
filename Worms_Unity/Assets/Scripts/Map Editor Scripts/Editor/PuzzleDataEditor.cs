using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PuzzleData))]
public class PuzzleDataEditor : Editor {
	private const int tileSize = 40;
	private const int wallThickness = 4;
	private const int wormSize = 20;

	private Texture2D blockedTileSprite;
	private Texture2D tileSprite;
	private Texture2D wallSprite;
	private Texture2D wallRotatedSprite;
	private Texture2D wormHead1Sprite;
	private Texture2D wormHead2Sprite;
	private Texture2D wormHead3Sprite;
	private Texture2D wormHead4Sprite;
	private IntVector2 size;

	void OnEnable() {
		wormHead1Sprite = Resources.Load("Inspector Sprites/wormHead1") as Texture2D;
		wormHead2Sprite = Resources.Load("Inspector Sprites/wormHead2") as Texture2D;
		wormHead3Sprite = Resources.Load("Inspector Sprites/wormHead3") as Texture2D;
		wormHead4Sprite = Resources.Load("Inspector Sprites/wormHead4") as Texture2D;
		wallSprite = Resources.Load("Inspector Sprites/wall") as Texture2D;
		wallRotatedSprite = Resources.Load("Inspector Sprites/wallRotated") as Texture2D;
		blockedTileSprite = Resources.Load("Inspector Sprites/blockedTile") as Texture2D;
		tileSprite = Resources.Load("Inspector Sprites/tile") as Texture2D;
	}

	public override void OnInspectorGUI() {
		DrawMap();
	}

	void DrawMap() {
		PuzzleData puzzleToLoad = target as PuzzleData;

		size = puzzleToLoad.size;
		
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				MapEditorTileData tileData = puzzleToLoad.tiles[y * size.x + x];
				Vector2 pos = GetPosition(new IntVector2(x, y));
				Rect textureRect = new Rect(pos.x, pos.y, tileSize, tileSize);
				if (tileData.tileType == TileType.Tile) EditorGUI.DrawPreviewTexture(textureRect, tileSprite);
				else if (tileData.tileType == TileType.Blocked) EditorGUI.DrawPreviewTexture(textureRect, blockedTileSprite);

				foreach (MapEditorWallData wall in tileData.walls) {
					Vector2 wallSize = new Vector2();
					Vector2 wallPos = new Vector2(pos.x, pos.y);
					Texture2D texture = null;

					if (wall.direction == BoardDirection.Up) {
						wallPos.y = wallPos.y - wallThickness / 2;
						wallSize = new Vector2(tileSize, wallThickness);
						texture = wallSprite;
					}
					if (wall.direction == BoardDirection.Right) {
						wallPos.x = wallPos.x + tileSize - wallThickness / 2;
						wallSize = new Vector2(wallThickness, tileSize);
						texture = wallRotatedSprite;
					}
					if (wall.direction == BoardDirection.Down) {
						wallPos.y = wallPos.y + tileSize - wallThickness / 2;
						wallSize = new Vector2(tileSize, wallThickness);
						texture = wallSprite;
					}
					if (wall.direction == BoardDirection.Left) {
						wallPos.x = wallPos.x - wallThickness / 2;
						wallSize = new Vector2(wallThickness, tileSize);
						texture = wallRotatedSprite;
					}

					if (texture != null) {
						Rect wallTextureRect = new Rect(wallPos.x, wallPos.y, wallSize.x, wallSize.y);
						EditorGUI.DrawPreviewTexture(wallTextureRect, texture);
					}
				}

				if (tileData.worm != null && tileData.worm.wormType != WormType.NONE) {
					textureRect = new Rect(pos.x + wormSize / 2, pos.y + wormSize / 2, wormSize, wormSize);
					Texture2D texture = null;

					if (tileData.worm.wormType == WormType.Worm1) texture = wormHead1Sprite;
					else if (tileData.worm.wormType == WormType.Worm2) texture = wormHead2Sprite;
					else if (tileData.worm.wormType == WormType.Worm3) texture = wormHead3Sprite;
					else if (tileData.worm.wormType == WormType.Worm4) texture = wormHead4Sprite;

					if (texture != null) EditorGUI.DrawPreviewTexture(textureRect, texture);
				}
			}
		}
	}

	Vector2 GetPosition(IntVector2 coordinates) {
		Vector2 pos = new Vector2();

		pos.x = coordinates.x * tileSize - size.x * tileSize / 2f + Screen.width / 2f;
		pos.y = (size.y - coordinates.y - 1) * tileSize - size.y * tileSize / 2f + Screen.height / 2f;

		return pos;
	}
}

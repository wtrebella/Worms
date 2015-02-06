using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MapEditorTileType {
	Tile,
	BlockedTile
}

public class MapEditorTile : MonoBehaviour {
	public IntVector2 coordinates;
	public List<MapEditorWall> walls = new List<MapEditorWall>();
	public MapEditorWorm worm;
	public MapEditorTileType tileType {get; private set;}

	public void SetTileType(MapEditorTileType tileType) {
		this.tileType = tileType;

		tk2dSprite sprite = GetComponent<tk2dSprite>();

		if (tileType == MapEditorTileType.Tile) sprite.SetSprite("tile");
		else sprite.SetSprite("blockedTile");
	}

	public MapEditorWall GetWall(MapEditorDirection rotation) {
		foreach (MapEditorWall wall in walls) {
			if (wall.direction == rotation) return wall;
		}

		return null;
	}

	public MapEditorTileData GetDataVersion() {
		MapEditorTileData data = new MapEditorTileData();
		data.tileType = tileType;
		data.coordinates = coordinates;
		if (data.worm != null) data.worm = worm.GetDataVersion();
		List<MapEditorWallData> wallData = new List<MapEditorWallData>();
		foreach (MapEditorWall wall in walls) wallData.Add(wall.GetDataVersion());
		return data;
	}
}

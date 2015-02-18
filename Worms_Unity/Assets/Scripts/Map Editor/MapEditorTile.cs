using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapEditorTile : MonoBehaviour {
	public IntVector2 coordinates;
	public List<MapEditorWall> walls = new List<MapEditorWall>();
	public MapEditorWorm worm;
	public MapEditorPeg peg;
	public TileType tileType {get; private set;}

	public void SetTileType(TileType tileType) {
		this.tileType = tileType;

		tk2dSprite sprite = GetComponent<tk2dSprite>();

		if (tileType == TileType.Tile) sprite.SetSprite("tile");
		else sprite.SetSprite("blockedTile");
	}

	public MapEditorWall GetWall(BoardDirection rotation) {
		foreach (MapEditorWall wall in walls) {
			if (wall.direction == rotation) return wall;
		}

		return null;
	}

	public MapEditorTileData GetDataVersion() {
		MapEditorTileData data = new MapEditorTileData();
		data.tileType = tileType;
		data.coordinates = coordinates;
		if (worm != null) data.worm = worm.GetDataVersion();
		if (peg != null) data.peg = peg.GetDataVersion();

		List<MapEditorWallData> wallData = new List<MapEditorWallData>();
		foreach (MapEditorWall wall in walls) wallData.Add(wall.GetDataVersion());
		data.walls = wallData;
		return data;
	}
}

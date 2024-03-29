using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapEditorTile : MonoBehaviour {
	[HideInInspector]
	public IntVector2 coordinates;

	[HideInInspector]
	public List<MapEditorWall> walls = new List<MapEditorWall>();

	[HideInInspector]
	public MapEditorWorm worm;

	[HideInInspector]
	public MapEditorGenericTileEntity tileEntity;

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
		if (tileEntity != null) data.tileEntity = tileEntity.GetDataVersion();

		List<MapEditorWallData> wallData = new List<MapEditorWallData>();
		foreach (MapEditorWall wall in walls) wallData.Add(wall.GetDataVersion());
		data.walls = wallData;
		return data;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class MapEditorTileData {
	public IntVector2 coordinates;
	public List<MapEditorWallData> walls;
	public MapEditorWormData worm;
	public MapEditorGenericTileEntityData tileEntity;
	public TileType tileType;

	public MapEditorTileData() {

	}
}

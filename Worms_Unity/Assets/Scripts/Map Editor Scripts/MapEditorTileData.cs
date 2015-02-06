using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapEditorTileData {
	public IntVector2 coordinates;
	public List<MapEditorWallData> walls = new List<MapEditorWallData>();
	public MapEditorWormData worm;
	public MapEditorTileType tileType;

	public MapEditorTileData() {

	}
}

using UnityEngine;
using System.Collections;

public class MapEditorGenericTileEntity : MonoBehaviour {
	public TileEntityType tileEntityType {get; private set;}

	public void SetTileEntityType(TileEntityType tileEntityType) {
		this.tileEntityType = tileEntityType;
	}

	public MapEditorGenericTileEntityData GetDataVersion() {
		MapEditorGenericTileEntityData data = new MapEditorGenericTileEntityData();
		data.tileEntityType = tileEntityType;
		return data;
	}
}

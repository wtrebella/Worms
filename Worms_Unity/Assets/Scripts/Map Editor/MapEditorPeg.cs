using UnityEngine;
using System.Collections;

public class MapEditorPeg : MonoBehaviour {
	public PegType pegType {get; private set;}

	public void SetPegType(PegType pegType) {
		this.pegType = pegType;
	}

	public MapEditorPegData GetDataVersion() {
		MapEditorPegData data = new MapEditorPegData();
		data.pegType = pegType;
		return data;
	}
}

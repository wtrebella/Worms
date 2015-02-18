using UnityEngine;
using System.Collections;
using System;

public enum PegType {
	Peg,
	NONE
}

[Serializable]
public class MapEditorPegData {
	public PegType pegType = PegType.NONE;

	public MapEditorPegData() {

	}
}

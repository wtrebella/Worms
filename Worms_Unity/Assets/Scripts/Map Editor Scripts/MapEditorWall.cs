using UnityEngine;
using System.Collections;

public class MapEditorWall : MonoBehaviour {
	public BoardDirection direction {get; private set;}

	public void SetDirection(BoardDirection direction) {
		this.direction = direction;
		
		transform.eulerAngles = new Vector3(0, 0, (int)direction * -90);
	}

	public MapEditorWallData GetDataVersion() {
		MapEditorWallData data = new MapEditorWallData();
		data.direction = direction;
		return data;
	}
}

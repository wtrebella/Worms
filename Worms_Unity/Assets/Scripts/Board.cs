using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {
	public tk2dSprite tileSpritePrefab;
	public int tileSize = 100;
	public int boardWidth = 5;
	public int boardHeight = 8;

	void Start () {
		CreateBoard();
	}
	
	void Update () {
	
	}

	void CreateBoard() {
		for (int y = 0; y < boardHeight; y++) {
			for (int x = 0; x < boardWidth; x++) {
				tk2dSprite tile = (tk2dSprite)Instantiate(tileSpritePrefab);
				Vector3 pos = new Vector3();
				pos.x = (x + 0.5f - boardWidth / 2f) * tileSize;
				pos.y = (y + 0.5f - boardHeight / 2f) * tileSize;
				pos.z = transform.position.z;
				tile.transform.parent = transform;
				tile.transform.position = pos;
			}
		}
	}
}

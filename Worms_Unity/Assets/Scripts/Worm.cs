using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormHead wormHeadPartPrefab;

	private Board board;
	private WormHead head;
	private List<WormBodyPart> bodyParts;

	public void Initialize(Board board, IntVector2 startCoordinates) {
		this.board = board;

		Tile tile = this.board.GetTile(startCoordinates);

		head = Instantiate(wormHeadPartPrefab) as WormHead;
		head.Initialize(this, tile, BoardDirection.Up); 
		head.transform.parent = transform;
		head.transform.position = this.board.GetTilePosition(startCoordinates);
	}

	public void Move(BoardDirection direction) {
		head.SetDirection(direction);

		TileEdge edge = head.currentTile.GetEdge(direction);
		if (edge is TilePassage) {
			head.SetCurrentTile(edge.otherTile);
		}
	}

	void Start () {
	
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow)) Move(BoardDirection.Up);
		else if (Input.GetKeyDown(KeyCode.RightArrow)) Move(BoardDirection.Right);
		else if (Input.GetKeyDown(KeyCode.DownArrow)) Move(BoardDirection.Down);
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(BoardDirection.Left);
	}
}
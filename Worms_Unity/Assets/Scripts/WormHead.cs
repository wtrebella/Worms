using UnityEngine;
using System.Collections;

public class WormHead : MonoBehaviour {
	public IntVector2 coordinates {get; private set;}
	public IntVector2 tempCoordinates {get; private set;}
	public BoardDirection direction {get; private set;}
	public Tile currentTile {
		get {
			return Board.instance.GetTile(coordinates);
		}
	}

	public void Initialize(Worm worm, IntVector2 startingCoordinates, BoardDirection startingDirection) {
		this.coordinates = new IntVector2(-1, -1);
		this.tempCoordinates = new IntVector2(-1, -1);
		transform.parent = worm.transform;
		SetDirection(startingDirection);
		ProposePosition(startingCoordinates);
		CommitPosition();
	}

	public void SetDirection(BoardDirection direction) {
		this.direction = direction;
		transform.localRotation = direction.ToRotation();
	}

	public void ProposePosition(IntVector2 newCoordinates) {
		Board.instance.RemoveObject(Board.instance.tempTileBitmasks, this.coordinates, ObjectType.WormHead);
		this.tempCoordinates = newCoordinates;
		Board.instance.AddObject(Board.instance.tempTileBitmasks, this.tempCoordinates, ObjectType.WormHead);
	}

	public bool CommitPosition() {
		if (Board.instance.GetTileContains(Board.instance.tempTileBitmasks, tempCoordinates, ObjectType.WormBodyPart) ||
		    Board.instance.GetTileContains(Board.instance.tempTileBitmasks, tempCoordinates, ObjectType.Enemy)) return false;

		Board.instance.RemoveObject(Board.instance.tileBitmasks, this.coordinates, ObjectType.WormHead);
		this.coordinates = this.tempCoordinates;
		Board.instance.AddObject(Board.instance.tileBitmasks, this.coordinates, ObjectType.WormHead);
		transform.position = Board.instance.GetTilePosition(this.coordinates);
		return true;
	}
}

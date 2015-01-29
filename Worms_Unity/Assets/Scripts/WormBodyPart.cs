using UnityEngine;
using System.Collections;

public class WormBodyPart : MonoBehaviour {
	public BoardDirection direction {get; private set;}
	public IntVector2 coordinates {get; private set;}
	public Tile tile {
		get {
			return Board.instance.GetTile(coordinates);
		}
	}

	public void Initialize(Worm worm, IntVector2 newCoordinates, BoardDirection direction) {
		coordinates = new IntVector2(-1, -1);
		this.direction = direction;
		transform.parent = worm.transform;
		SetPosition(newCoordinates);
		SetDirection(direction);
	}

	public void SetPosition(IntVector2 newCoordinates) {
		Board.instance.RemoveObject(Board.instance.tileBitmasks, this.coordinates, ObjectType.WormBodyPart);
		this.coordinates = newCoordinates;
		transform.position = Board.instance.GetTilePosition(this.coordinates);
		Board.instance.AddObject(Board.instance.tileBitmasks, this.coordinates, ObjectType.WormBodyPart);
	}
	
	public void SetDirection(BoardDirection direction) {
		transform.localRotation = direction.ToRotation();
	}
}

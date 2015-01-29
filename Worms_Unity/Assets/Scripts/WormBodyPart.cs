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

	public void Initialize(Worm worm, IntVector2 coordinates, BoardDirection direction) {
		coordinates = new IntVector2(-1, -1);
		SetPosition(coordinates);
		this.direction = direction;
		transform.parent = worm.transform;
		transform.localPosition = Vector3.zero;
		SetDirection(direction);
	}

	public void SetPosition(IntVector2 coordinates) {
		Board.instance.RemoveObject(coordinates, ObjectType.WormBodyPart);
		this.coordinates = coordinates;
		transform.position = Board.instance.GetTilePosition(coordinates);
		Board.instance.AddObject(coordinates, ObjectType.WormBodyPart);
	}
	
	public void SetDirection(BoardDirection direction) {
		transform.localRotation = direction.ToRotation();
	}
}

using UnityEngine;
using System.Collections;

public class WormHead : MonoBehaviour {
	public IntVector2 coordinates {get; private set;}
	public Tile currentTile {
		get {
			return Board.instance.GetTile(coordinates);
		}
	}

	public void Initialize(Worm worm, IntVector2 startingCoordinates, BoardDirection startingDirection) {
		coordinates = new IntVector2(-1, -1);
		SetPosition(startingCoordinates);
		transform.parent = worm.transform;
		transform.localPosition = Vector3.zero;
		SetDirection(startingDirection);
	}

	public void SetPosition(IntVector2 coordinates) {
		Board.instance.RemoveObject(coordinates, ObjectType.WormHead);
		this.coordinates = coordinates;
		transform.position = Board.instance.GetTilePosition(coordinates);
		Board.instance.AddObject(coordinates, ObjectType.WormHead);
	}

	public void SetDirection(BoardDirection direction) {
		transform.localRotation = direction.ToRotation();
	}
}

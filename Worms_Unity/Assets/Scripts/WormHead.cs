using UnityEngine;
using System.Collections;

public class WormHead : MonoBehaviour {
	public Tile currentTile;

	public void Initialize(Worm worm, Tile startingTile, BoardDirection startingDirection) {
		this.currentTile = startingTile;
		transform.parent = worm.transform;
		transform.localPosition = Vector3.zero;
		SetDirection(startingDirection);
	}

	public void SetDirection(BoardDirection direction) {
		transform.localRotation = direction.ToRotation();
	}

	public void SetCurrentTile(Tile newTile) {
		currentTile = newTile;
		transform.position = currentTile.transform.position;
	}
}

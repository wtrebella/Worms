using UnityEngine;
using System.Collections;

public class WormBodyPart : TileEntity {
	public BoardDirection direction {get; private set;}

	private Worm worm;

	public void Initialize(Worm worm, Tile tile, BoardDirection direction) {
		this.worm = worm;
		tileEntityType = TileEntityType.WormBodyPart;
		transform.parent = worm.transform;
		GoToTile(tile, direction);
	}

	private void SetDirection(BoardDirection newDirection) {
		direction = newDirection;
		transform.localRotation = direction.ToRotation();
	}

	public override void GoToTile(Tile tile, BoardDirection newDirection) {
		if (currentTile != null) {
			Debug.LogWarning("this shouldn't happen! don't try to move a worm body part to another tile.");
			currentTile.RemoveObject(this);
		}
		SetDirection(newDirection);
		currentTile = tile;
		currentTile.AddObject(this);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}
	
	public override bool CanMoveToTile(Tile tile) {
		return false;
	}
}

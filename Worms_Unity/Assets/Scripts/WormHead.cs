using UnityEngine;
using System.Collections;

public class WormHead : TileEntity {
	public BoardDirection direction {get; private set;}

	private Worm worm;

	public void Initialize(Worm worm, Tile tile, BoardDirection direction) {
		this.worm = worm;
		tileEntityType = TileEntityType.WormHead;
		transform.parent = worm.transform;
		GoToTile(tile, direction);
	}

	private void SetDirection(BoardDirection newDirection) {
		direction = newDirection;
		transform.localRotation = direction.ToRotation();
	}

	public override void GoToTile(Tile tile, BoardDirection newDirection) {
		Tile previousTile = currentTile;
		RemoveFromTile();
		currentTile = tile;
		SetDirection(newDirection);
		currentTile.AddObject(this);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
		worm.HandleHeadMoved(previousTile, currentTile, newDirection);
	}
	
	public override bool CanMoveToTile(Tile tile) {
		return !(tile.Contains(TileEntityType.WormBodyPart));
	}

	public override void RemoveFromTile() {
		if (currentTile == null) return;

		currentTile.RemoveObject(this);
		currentTile = null;
	}
}
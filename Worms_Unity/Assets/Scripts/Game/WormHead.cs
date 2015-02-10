using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormHead : TileEntity {
	public BoardDirection direction {get; private set;}

	private Worm worm;

	public void Initialize(Worm worm, Tile tile, BoardDirection newDirection) {
		this.worm = worm;
		tileEntityType = TileEntityType.WormHead;
		transform.parent = worm.transform;
		direction = BoardDirection.NONE;
		SetTile(tile);
		if (newDirection != BoardDirection.NONE) transform.localRotation = newDirection.ToRotation();
		worm.HandleHeadMoved(null, tile, BoardDirection.NONE, BoardDirection.NONE);
	}

	private void SetDirection(BoardDirection newDirection) {
		direction = newDirection;
		transform.localRotation = direction.ToRotation();
	}

	public override void SetTile(Tile tile) {
		RemoveFromTile();
		currentTile = tile;
		if (!tile.tileEntities.Contains(this)) tile.tileEntities.Add(this);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}
	
	public override void Move(BoardDirection newDirection) {
		if (currentTile == null) Debug.LogError("can't move an entity before it has a tile");

		Tile previousTile = currentTile;
		BoardDirection previousDirection = direction;
		SetDirection(newDirection);
		SetTile(Board.instance.GetTile(currentTile.coordinates + newDirection.ToIntVector2()));
		worm.HandleHeadMoved(previousTile, currentTile, previousDirection, newDirection);
	}

	public override void RemoveFromTile() {
		if (currentTile == null) return;

		if (currentTile.tileEntities.Contains(this)) currentTile.tileEntities.Remove(this);
		currentTile = null;
	}
}
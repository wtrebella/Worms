using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormBodyPart : TileEntity {
	public Worm worm;
	public BoardDirection direction {get; private set;}
	
	public void Initialize(Worm worm, Tile tile) {
		this.worm = worm;
		tileEntityType = TileEntityType.WormBodyPart;
		transform.parent = worm.transform;
		SetTile(tile);
	}

	public override void SetTile(Tile tile) {
		base.SetTile(tile);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}
	
	public override void AutoMove(BoardDirection newDirection) {
		if (currentTile == null) Debug.LogError("can't move an entity before it has a tile");
		
		SetTile(Board.instance.GetTile(currentTile.coordinates + newDirection.ToIntVector2()));
	}
}

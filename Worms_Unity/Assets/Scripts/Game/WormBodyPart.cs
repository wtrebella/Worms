using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormBodyPart : TileEntity {
	public Worm worm;

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
}

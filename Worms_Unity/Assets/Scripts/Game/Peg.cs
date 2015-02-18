using UnityEngine;
using System.Collections;

public class Peg : TileEntity {

	public void Initialize(Tile tile) {
		tileEntityType = TileEntityType.Peg;
//		transform.parent = worm.transform;
		SetTile(tile);
	}
	
	public override void SetTile(Tile tile) {
		base.SetTile(tile);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}
}

using UnityEngine;
using System.Collections;

public class DeathTrap : TileEntity {
	public void Initialize(Tile tile) {
		tileEntityType = TileEntityType.DeathTrap;
		transform.parent = Board.instance.transform;
		SetTile(tile);
	}
	
	public override void SetTile(Tile tile) {
		base.SetTile(tile);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TileEntity : MonoBehaviour {
	public Tile currentTile;
	public TileEntityType tileEntityType = TileEntityType.NONE;

	public abstract void SetTile(Tile tile);
	public abstract void Move(BoardDirection newDirection);
	public abstract void RemoveFromTile();
	public abstract bool CanEnterTileWithTileEntities(List<TileEntity> tileEntities);
}

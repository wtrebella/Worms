using UnityEngine;
using System.Collections;

public abstract class TileEntity : MonoBehaviour {
	public Tile currentTile;
	public TileEntityType tileEntityType = TileEntityType.NONE;

	public abstract bool CanMoveToTile(Tile tile);
	public abstract void GoToTile(Tile tile, BoardDirection newDirection);
	public abstract void RemoveFromTile();
}

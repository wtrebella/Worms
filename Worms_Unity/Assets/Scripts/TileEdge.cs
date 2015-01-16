using UnityEngine;

public abstract class TileEdge : MonoBehaviour {
	
	public Tile tile, otherTile;
	
	public BoardDirection direction;

	public void Initialize (Tile tile, Tile otherTile, BoardDirection direction) {
		this.tile = tile;
		this.otherTile = otherTile;
		this.direction = direction;
		tile.SetEdge(direction, this);
		transform.parent = tile.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = direction.ToRotation();
	}
}
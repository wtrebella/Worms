using UnityEngine;
using System.Collections;

public class WormBodyPart : MonoBehaviour {
	public Tile tile;
	public TileEdge tileEdge;
	public BoardDirection direction;
	
	public void Initialize(Worm worm, Tile tile, TileEdge tileEdge, BoardDirection direction) {
		this.tile = tile;
		this.tileEdge = tileEdge;
		this.direction = direction;
		transform.parent = worm.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = direction.ToRotation();
	}

}

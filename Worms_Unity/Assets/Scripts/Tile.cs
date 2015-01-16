using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public IntVector2 coordinates;

	private TileEdge[] edges = new TileEdge[BoardDirections.Count];
	
	public TileEdge GetEdge (BoardDirection direction) {
		return edges[(int)direction];
	}
	
	public void SetEdge (BoardDirection direction, TileEdge edge) {
		edges[(int)direction] = edge;
	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public IntVector2 coordinates;

	private int initializedEdgeCount = 0;
	
	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == BoardDirections.Count;
		}
	}

	private TileEdge[] edges = new TileEdge[BoardDirections.Count];
	
	public TileEdge GetEdge (BoardDirection direction) {
		return edges[(int)direction];
	}
	
	public void SetEdge (BoardDirection direction, TileEdge edge) {
		edges[(int)direction] = edge;
		initializedEdgeCount++;
	}

	public bool GetDirectionIsInitialized(BoardDirection direction) {
		return edges[(int)direction] != null;
	}

	void Start () {

	}
	
	void Update () {
	
	}
}

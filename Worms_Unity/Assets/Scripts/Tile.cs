using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	public IntVector2 coordinates;
	public List<TileEntity> tileEntities;

	private int initializedEdgeCount = 0;
	
	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == BoardDirections.Count;
		}
	}

	private TileEdge[] edges = new TileEdge[BoardDirections.Count];

	void Awake() {
		tileEntities = new List<TileEntity>();
	}

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

	public bool Contains(TileEntityType tileEntityType) {
		foreach (TileEntity t in tileEntities) {
			if (t.tileEntityType == tileEntityType) return true;
		}
		
		return false;
	}

	public bool IsEmpty() {
		return tileEntities.Count == 0;
	}
	
	public void AddObject(TileEntity tileEntity) {
		tileEntities.Add(tileEntity);
	}
	
	public void RemoveObject(TileEntity tileEntity) {
		for (int i = tileEntities.Count - 1; i >= 0; i--) {
			TileEntity t = tileEntities[i];
			if (tileEntity == t) {
				tileEntities.RemoveAt(i);
				return;
			}
		}
	}
}

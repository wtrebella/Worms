using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType {
	Tile,
	Blocked
}

public class Tile : MonoBehaviour {
	public TileType tileType;
	public tk2dSprite tileSpritePrefab;
	public IntVector2 coordinates;
	public List<TileEntity> tileEntities;

	private int initializedEdgeCount = 0;

	public void Initialize() {
		tileEntities = new List<TileEntity>();
	}

	public void InitializeSprite() {
		if (tileSpritePrefab == null) return;

		tk2dSprite s = Instantiate(tileSpritePrefab) as tk2dSprite;
		s.transform.parent = transform;
		s.transform.localPosition = Vector3.zero;
	}

	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == BoardDirections.Count;
		}
	}

	public bool IsCompleted() {
		if (tileType == TileType.Blocked) return true;

		foreach (TileEntity t in tileEntities) {
			if (t.tileEntityType == TileEntityType.WormHead || t.tileEntityType == TileEntityType.WormBodyPart) return true;
		}

		return false;
	}

	private TileEdge[] edges = new TileEdge[BoardDirections.Count];

	public TileEdge GetEdge (BoardDirection direction) {
		return edges[(int)direction];
	}
	
	public void SetEdge(BoardDirection direction, TileEdge edge) {
		edges[(int)direction] = edge;
		initializedEdgeCount++;
	}

	public bool GetDirectionIsInitialized(BoardDirection direction) {
		return edges[(int)direction] != null;
	}

	public TileEntity GetTileEntity(TileEntityType tileEntityType) {
		foreach (TileEntity t in tileEntities) {
			if (t.tileEntityType == tileEntityType) return t;
		}

		return null;
	}

	public List<TileEntity> GetTileEntities(TileEntityType tileEntityType) {
		List<TileEntity> entities = new List<TileEntity>();

		foreach (TileEntity t in tileEntities) {
			if (t.tileEntityType == tileEntityType) entities.Add(t);
		}

		return entities;
	}
}

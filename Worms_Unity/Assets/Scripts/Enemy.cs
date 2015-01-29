using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public IntVector2 coordinates {get; private set;}
	public IntVector2 tempCoordinates {get; private set;}
	public Tile currentTile {
		get {
			return Board.instance.GetTile(coordinates);
		}
	}
	
	public void Initialize(EnemyManager enemyManager, IntVector2 newCoordinates) {
		coordinates = new IntVector2(-1, -1);
		transform.parent = enemyManager.transform;
		ProposePosition(newCoordinates);
		CommitPosition();
	}
	
	public void ProposePosition(IntVector2 newCoordinates) {
		Board.instance.RemoveObject(Board.instance.tempTileBitmasks, this.coordinates, ObjectType.Enemy);
		this.tempCoordinates = newCoordinates;
		Board.instance.AddObject(Board.instance.tempTileBitmasks, this.tempCoordinates, ObjectType.Enemy);
	}

	public bool CommitPosition() {
		if (Board.instance.GetTileContains(Board.instance.tempTileBitmasks, tempCoordinates, ObjectType.WormHead) ||
		    Board.instance.GetTileContains(Board.instance.tempTileBitmasks, tempCoordinates, ObjectType.WormBodyPart)) return false;

		Board.instance.RemoveObject(Board.instance.tileBitmasks, this.coordinates, ObjectType.Enemy);
		this.coordinates = this.tempCoordinates;
		transform.position = Board.instance.GetTilePosition(this.coordinates);
		Board.instance.AddObject(Board.instance.tileBitmasks, this.coordinates, ObjectType.Enemy);

		return true;
	}

	public void CommitMove() {
		CommitPosition();
	}
	
	public void ProposeMove(BoardDirection direction) {
		TileEdge edge = currentTile.GetEdge(direction);
		Tile otherTile = null;
		if (edge is TilePassage) otherTile = edge.otherTile;
		
		if (otherTile != null) {
			ProposePosition(otherTile.coordinates);
		}
	}
}

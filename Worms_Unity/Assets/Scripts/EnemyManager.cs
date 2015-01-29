using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
	public Enemy enemyPrefab;

	private List<Enemy> enemies;

	public void Initialize() {
		enemies = new List<Enemy>();
		transform.parent = Board.instance.transform;
		transform.position = Vector3.zero;
	}

	public void AddEnemy(IntVector2 coordinates) {
		Enemy enemy = Instantiate(enemyPrefab) as Enemy;
		enemy.transform.parent = transform;
		enemy.Initialize(this, coordinates);
		enemies.Add(enemy);
	}

	public void CommitMove() {
		foreach (Enemy enemy in enemies) enemy.CommitMove();
	}

	public void ProposeMove(BoardDirection direction) {
		foreach (Enemy enemy in enemies) enemy.ProposeMove(direction);
	}
}

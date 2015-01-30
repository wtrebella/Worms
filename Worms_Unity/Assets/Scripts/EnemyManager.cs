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

	public void AddEnemy(Tile tile) {
		Enemy enemy = Instantiate(enemyPrefab) as Enemy;
		enemy.transform.parent = transform;
		enemy.Initialize(this, tile);
		enemies.Add(enemy);
	}

	public void RemoveEnemy(Enemy enemy) {
		enemy.RemoveFromTile();
		enemies.Remove(enemy);
		Destroy(enemy.gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Board boardPrefab;
	public Worm wormPrefab;
	
	private Board board;
	private Worm worm;

	private void Start () {
		BeginGame();
	}
	
	private void Update () {

	}
	
	private void BeginGame () {
		board = Instantiate(boardPrefab) as Board;
		board.Generate();

		worm = Instantiate(wormPrefab) as Worm;
		worm.Initialize(board, new IntVector2(3, 0));
	}
	
	private void RestartGame () {
		Destroy(board.gameObject);
		BeginGame();
	}
}
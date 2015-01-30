using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Board boardPrefab;
	public Worm wormPrefab;
	public EnemyManager enemyManagerPrefab;
	
	private Board board;
	private Worm worm;
	private EnemyManager enemyManager;

	private void Start () {
		BeginGame();
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) RestartGame();

		BoardDirection direction = BoardDirection.NONE;

		if (Input.GetKeyDown(KeyCode.UpArrow)) direction = BoardDirection.Up;
		else if (Input.GetKeyDown(KeyCode.RightArrow)) direction = BoardDirection.Right;
		else if (Input.GetKeyDown(KeyCode.DownArrow)) direction = BoardDirection.Down;
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = BoardDirection.Left;

		if (direction != BoardDirection.NONE) {
			board.Move(direction);
			board.AttemptToAddEnemy(enemyManager, direction);
		}
	}
	
	private void BeginGame () {
		board = Instantiate(boardPrefab) as Board;
		board.Generate();

		worm = Instantiate(wormPrefab) as Worm;
		BoardDirection startDirection = BoardDirections.RandomValue;
		IntVector2 startCoordinates = IntVector2.zero;
		if (startDirection == BoardDirection.Up) startCoordinates = new IntVector2(Random.Range(0, board.size.x), 0);
		else if (startDirection == BoardDirection.Down) startCoordinates = new IntVector2(Random.Range(0, board.size.x), board.size.y - 1);
		else if (startDirection == BoardDirection.Right) startCoordinates = new IntVector2(0, Random.Range(0, board.size.y));
		else if (startDirection == BoardDirection.Left) startCoordinates = new IntVector2(board.size.x - 1, Random.Range(0, board.size.y));
		Tile startTile = board.GetTile(startCoordinates);
		worm.Initialize(startTile, startDirection);

		enemyManager = Instantiate(enemyManagerPrefab) as EnemyManager;
		enemyManager.Initialize();
	}
	
	private void RestartGame () {
		Destroy(board.gameObject);
		BeginGame();
	}
}
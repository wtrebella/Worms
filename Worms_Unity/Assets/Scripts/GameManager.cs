using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	public PuzzleData puzzleToLoad;

	public Board boardPrefab;
	public Worm wormPrefab;
	public EnemyManager enemyManagerPrefab;
	
	public Board board;
	public EnemyManager enemyManager;

	private void Awake() {
		instance = this;
	}

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
		}
	}
	
	private void BeginGame () {
		board = Instantiate(boardPrefab) as Board;
		board.Generate(puzzleToLoad);

//		Worm worm = Instantiate(wormPrefab) as Worm;
//		BoardDirection startDirection = BoardDirections.RandomValue;
//		IntVector2 startCoordinates = board.RandomCoordinates;
//		Tile startTile = board.GetTile(startCoordinates);
//		worm.Initialize(startTile, startDirection, new Color(0.1f, 0.3f, 0.9f));
//
//		worm = Instantiate(wormPrefab) as Worm;
//		startDirection = BoardDirections.RandomValue;
//		startCoordinates = board.RandomCoordinates;
//		startTile = board.GetTile(startCoordinates);
//		worm.Initialize(startTile, startDirection, new Color(0.9f, 0.1f, 0.2f));
//
//		worm = Instantiate(wormPrefab) as Worm;
//		startDirection = BoardDirections.RandomValue;
//		startCoordinates = board.RandomCoordinates;
//		startTile = board.GetTile(startCoordinates);
//		worm.Initialize(startTile, startDirection, new Color(0.1f, 0.8f, 0.4f));
//
//		worm = Instantiate(wormPrefab) as Worm;
//		startDirection = BoardDirections.RandomValue;
//		startCoordinates = board.RandomCoordinates;
//		startTile = board.GetTile(startCoordinates);
//		worm.Initialize(startTile, startDirection, new Color(0.6f, 0.1f, 0.6f));
	}
	
	private void RestartGame () {
		Destroy(board.gameObject);
		BeginGame();
	}
}
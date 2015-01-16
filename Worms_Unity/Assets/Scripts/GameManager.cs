using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Board boardPrefab;
	
	private Board boardInstance;

	private void Start () {
		BeginGame();
	}
	
	private void Update () {

	}
	
	private void BeginGame () {
		boardInstance = Instantiate(boardPrefab) as Board;
		boardInstance.Generate();
	}
	
	private void RestartGame () {
		Destroy(boardInstance.gameObject);
		BeginGame();
	}
}
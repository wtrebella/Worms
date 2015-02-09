using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
	public PuzzleManager puzzleManager;
	public GameObject winLayer;
	public Button previousButton;
	public Button nextButton;

	public void HandleWin() {
		winLayer.SetActive(true);
	}

	public void HandlePuzzleLoaded(int puzzleIndex, PuzzleData puzzleData) {
		winLayer.SetActive(false);

		if (puzzleIndex <= 0) previousButton.interactable = false;
		else previousButton.interactable = true;

		if (puzzleIndex >= puzzleManager.puzzles.Length - 1) nextButton.interactable = false;
		else nextButton.interactable = true;
	}
}

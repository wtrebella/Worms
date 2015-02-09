using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {
	public GameObject winLayer;

	public void HandleWin() {
		winLayer.SetActive(true);
	}

	public void HandleBeginGame() {
		winLayer.SetActive(false);
	}
}

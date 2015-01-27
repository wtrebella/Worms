using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormHeadPart wormHeadPartPrefab;

	private Board board;
	private WormHeadPart headPart;
	private List<WormBodyPart> bodyParts;

	public void Initialize(Board board, IntVector2 startCoordinates) {
		this.board = board;

		headPart = Instantiate(wormHeadPartPrefab) as WormHeadPart;
		headPart.transform.parent = transform;
		headPart.transform.position = board.GetTilePosition(startCoordinates);
	}

	public void Move(BoardDirection boardDirection) {

	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SwipeEventSystem : MonoBehaviour {
	[HideInInspector]
	public List<EventDelegate> onSwipeUpBegan;
	[HideInInspector]
	public List<EventDelegate> onSwipeRightBegan;
	[HideInInspector]
	public List<EventDelegate> onSwipeDownBegan;
	[HideInInspector]
	public List<EventDelegate> onSwipeLeftBegan;
	
	[HideInInspector]
	public List<EventDelegate> onSwipeUpContinue;
	[HideInInspector]
	public List<EventDelegate> onSwipeRightContinue;
	[HideInInspector]
	public List<EventDelegate> onSwipeDownContinue;
	[HideInInspector]
	public List<EventDelegate> onSwipeLeftContinue;

	[HideInInspector]
	public List<EventDelegate> onSwipeUpEnded;
	[HideInInspector]
	public List<EventDelegate> onSwipeRightEnded;
	[HideInInspector]
	public List<EventDelegate> onSwipeDownEnded;
	[HideInInspector]
	public List<EventDelegate> onSwipeLeftEnded;

	[HideInInspector]
	public List<EventDelegate> onSwipeCanceled;

	[HideInInspector]
	public BoardDirection currentSwipeDirection = BoardDirection.NONE;

	[HideInInspector]
	public float currentSwipeDistance = 0;

	[HideInInspector]
	public float totalSwipeTime = 0;

	public bool simulateTouch = true;
	public float swipeDistance = 0.5f;
	public float swipeDelay = 0.5f;
	public float quickSwipeMaxLength = 0.1f;

	private bool isTouching = false;
	private bool isSwiping = false;
	private int swipeFingerID = 0;
	private Vector2 initialTouchPos = Vector2.zero;
	private BoardDirection allowedSwipeDirection = BoardDirection.NONE;

	void Start () {
	
	}

	void Update () {
		UpdateSwipeDetection();
	}

	void OnSwipeUpBegan() {
		EventDelegate.Execute(onSwipeUpBegan);
	}
	
	void OnSwipeRightBegan() {
		EventDelegate.Execute(onSwipeRightBegan);
	}
	
	void OnSwipeDownBegan() {
		EventDelegate.Execute(onSwipeDownBegan);
	}
	
	void OnSwipeLeftBegan() {
		EventDelegate.Execute(onSwipeLeftBegan);
	}

	void OnSwipeUpContinue() {
		EventDelegate.Execute(onSwipeUpContinue);
	}
	
	void OnSwipeRightContinue() {
		EventDelegate.Execute(onSwipeRightContinue);
	}
	
	void OnSwipeDownContinue() {
		EventDelegate.Execute(onSwipeDownContinue);
	}
	
	void OnSwipeLeftContinue() {
		EventDelegate.Execute(onSwipeLeftContinue);
	}

	void OnSwipeCanceled() {
		EventDelegate.Execute(onSwipeCanceled);
	}

	void OnSwipeUpEnded() {
		EventDelegate.Execute(onSwipeUpEnded);
	}

	void OnSwipeRightEnded() {
		EventDelegate.Execute(onSwipeRightEnded);
	}

	void OnSwipeDownEnded() {
		EventDelegate.Execute(onSwipeDownEnded);
	}

	void OnSwipeLeftEnded() {
		EventDelegate.Execute(onSwipeLeftEnded);
	}
	
	void CancelSwipe(bool useCallbacks = true) {
		if (isSwiping && useCallbacks) OnSwipeCanceled();

		currentSwipeDirection = BoardDirection.NONE;
		isSwiping = false;
		totalSwipeTime = 0;
	}

	void EndSwipe() {
		if (currentSwipeDirection == BoardDirection.Up) OnSwipeUpEnded();
		else if (currentSwipeDirection == BoardDirection.Right) OnSwipeRightEnded();
		else if (currentSwipeDirection == BoardDirection.Down) OnSwipeDownEnded();
		else if (currentSwipeDirection == BoardDirection.Left) OnSwipeLeftEnded();

		currentSwipeDirection = BoardDirection.NONE;
		isSwiping = false;
		totalSwipeTime = 0;
	}

	void ContinueSwipe() {
		if (currentSwipeDirection == BoardDirection.Up) OnSwipeUpContinue();
		else if (currentSwipeDirection == BoardDirection.Right) OnSwipeRightContinue();
		else if (currentSwipeDirection == BoardDirection.Down) OnSwipeDownContinue();
		else if (currentSwipeDirection == BoardDirection.Left) OnSwipeLeftContinue();
	}

	public void CancelTouch(bool useCallbacks = true) {
		isTouching = false;
		allowedSwipeDirection = BoardDirection.NONE;

		CancelSwipe(useCallbacks);
	}

	public void EndTouch() {
		isTouching = false;
		allowedSwipeDirection = BoardDirection.NONE;

		EndSwipe();
	}

	void BeginTouch(Vector3 pos) {
		isTouching = true;
		totalSwipeTime = 0;
		currentSwipeDistance = 0;
		initialTouchPos = pos;
	}

	void BeginSwipe(BoardDirection direction) {
		currentSwipeDirection = direction;
		if (allowedSwipeDirection == BoardDirection.NONE) allowedSwipeDirection = direction;

		isSwiping = true;
		if (currentSwipeDirection == BoardDirection.Up) OnSwipeUpBegan();
		else if (currentSwipeDirection == BoardDirection.Right) OnSwipeRightBegan();
		else if (currentSwipeDirection == BoardDirection.Down) OnSwipeDownBegan();
		else if (currentSwipeDirection == BoardDirection.Left) OnSwipeLeftBegan();

//		Debug.Log(currentSwipeDirection.ToString() + " began");
	}

	bool SwipeDirectionAllowed(BoardDirection direction) {
		return direction != BoardDirection.NONE && (allowedSwipeDirection == BoardDirection.NONE || allowedSwipeDirection == direction);
	}

	void UpdateSwipeDetection() {
		if (simulateTouch) {
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			if (isTouching) {
				Vector2 currentSwipeVector = Vector2.zero;
				currentSwipeVector.x = mousePosition.x - initialTouchPos.x;
				currentSwipeVector.y = mousePosition.y - initialTouchPos.y;

				totalSwipeTime += Time.deltaTime;

				if (isSwiping) {
					if (currentSwipeDirection == BoardDirection.Right) {
						if (currentSwipeVector.x < 0) currentSwipeVector.x = 0;
						currentSwipeDistance = currentSwipeVector.x;
						ContinueSwipe();
					}
					else if (currentSwipeDirection == BoardDirection.Left) {
						if (currentSwipeVector.x > 0) currentSwipeVector.x = 0;
						currentSwipeDistance = currentSwipeVector.x;
						ContinueSwipe();
					}
					else if (currentSwipeDirection == BoardDirection.Up) {
						if (currentSwipeVector.y < 0) currentSwipeVector.y = 0;
						currentSwipeDistance = currentSwipeVector.y;
						ContinueSwipe();
					}
					else if (currentSwipeDirection == BoardDirection.Down) {
						if (currentSwipeVector.y > 0) currentSwipeVector.y = 0;
						currentSwipeDistance = currentSwipeVector.y;
						ContinueSwipe();
					}
				}
				else {
					if (totalSwipeTime >= swipeDelay) {
						float horVal = currentSwipeVector.x;
						float vertVal = currentSwipeVector.y;

						bool horizontalAllowed = SwipeDirectionAllowed(BoardDirection.Right) || SwipeDirectionAllowed(BoardDirection.Left);
						bool verticalAllowed = SwipeDirectionAllowed(BoardDirection.Up) || SwipeDirectionAllowed(BoardDirection.Down);

						bool horizontal;

						if (horizontalAllowed && verticalAllowed) horizontal = Mathf.Abs(horVal) > Mathf.Abs(vertVal);
						else horizontal = horizontalAllowed;

						BoardDirection swipeDir = BoardDirection.NONE;

						if (horizontal) {
							if (horVal > swipeDistance) swipeDir = BoardDirection.Right;
							else if (horVal < -swipeDistance) swipeDir = BoardDirection.Left;
						}
						else {
							if (vertVal > swipeDistance) swipeDir = BoardDirection.Up;
							else if (vertVal < -swipeDistance) swipeDir = BoardDirection.Down;
						}

						if (SwipeDirectionAllowed(swipeDir)) BeginSwipe(swipeDir);
					}
				}

				if (Input.GetMouseButtonUp(0)) {
					if (isSwiping) EndTouch();
					else CancelTouch();
				}
			}
			else {
				if (Input.GetMouseButtonDown(0)) BeginTouch(mousePosition);
			}
		}

		if (isTouching) {
			if (Input.touches.Length > 0) {
				Touch t = Input.GetTouch(swipeFingerID);
				Vector2 currentSwipeVector = Vector2.zero;
				currentSwipeVector.x = t.position.x - initialTouchPos.x;
				currentSwipeVector.y = t.position.y - initialTouchPos.y;
				
				totalSwipeTime += Time.deltaTime;
				
				if (isSwiping) {
					if (currentSwipeDirection == BoardDirection.Right) {
						if (currentSwipeVector.x < 0) currentSwipeVector.x = 0;
						currentSwipeDistance = currentSwipeVector.x;
						ContinueSwipe();
					}
					else if (currentSwipeDirection == BoardDirection.Left) {
						if (currentSwipeVector.x > 0) currentSwipeVector.x = 0;
						currentSwipeDistance = currentSwipeVector.x;
						ContinueSwipe();
					}
					else if (currentSwipeDirection == BoardDirection.Up) {
						if (currentSwipeVector.y < 0) currentSwipeVector.y = 0;
						currentSwipeDistance = currentSwipeVector.y;
						ContinueSwipe();
					}
					else if (currentSwipeDirection == BoardDirection.Down) {
						if (currentSwipeVector.y > 0) currentSwipeVector.y = 0;
						currentSwipeDistance = currentSwipeVector.y;
						ContinueSwipe();
					}
				}
				else {
					if (totalSwipeTime >= swipeDelay) {
						float horVal = currentSwipeVector.x;
						float vertVal = currentSwipeVector.y;
						
						bool horizontalAllowed = SwipeDirectionAllowed(BoardDirection.Right) || SwipeDirectionAllowed(BoardDirection.Left);
						bool verticalAllowed = SwipeDirectionAllowed(BoardDirection.Up) || SwipeDirectionAllowed(BoardDirection.Down);
						
						bool horizontal;
						
						if (horizontalAllowed && verticalAllowed) horizontal = Mathf.Abs(horVal) > Mathf.Abs(vertVal);
						else horizontal = horizontalAllowed;
						
						BoardDirection swipeDir = BoardDirection.NONE;
						
						if (horizontal) {
							if (horVal > swipeDistance) swipeDir = BoardDirection.Right;
							else if (horVal < -swipeDistance) swipeDir = BoardDirection.Left;
						}
						else {
							if (vertVal > swipeDistance) swipeDir = BoardDirection.Up;
							else if (vertVal < -swipeDistance) swipeDir = BoardDirection.Down;
						}
						
						if (SwipeDirectionAllowed(swipeDir)) BeginSwipe(swipeDir);
					}
				}
			
				if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) {
					if (isSwiping) EndTouch();
					else CancelTouch();
				}
			}
		}
		else {
			foreach (Touch t in Input.touches) {
				if (t.phase == TouchPhase.Began) {
					BeginTouch(t.position);
					swipeFingerID = t.fingerId;
					break;
				}
			}
		}
	}
}

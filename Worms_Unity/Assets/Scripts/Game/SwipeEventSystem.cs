using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SwipeEventSystem : MonoBehaviour {
	[HideInInspector]
	public List<EventDelegate> onSwipeUp;
	[HideInInspector]
	public List<EventDelegate> onSwipeRight;
	[HideInInspector]
	public List<EventDelegate> onSwipeDown;
	[HideInInspector]
	public List<EventDelegate> onSwipeLeft;

	public bool simulateTouch = true;
	public float swipeDistance = 0.5f;
	public float swipeTime = 0.5f;

	private bool isSwiping = false;
	private float totalSwipeTime = 0;
	private int swipeFingerID = 0;
	private Vector2 swipeVector = Vector2.zero;
	private Vector2 initialTouchPos = Vector2.zero;

	private Vector2 lastMousePos = Vector2.zero;

	void Start () {
	
	}

	public void Up() {
		Debug.Log("Up, " + (Input.mousePosition.y - initialTouchPos.y) + ", " + totalSwipeTime);
	}

	public void Right() {
		Debug.Log("Right, " + (Input.mousePosition.x - initialTouchPos.x) + ", " + totalSwipeTime);
	}

	public void Down() {
		Debug.Log("Down, " + (Input.mousePosition.y - initialTouchPos.y) + ", " + totalSwipeTime);
	}

	public void Left() {
		Debug.Log("Left, " + (Input.mousePosition.x - initialTouchPos.x) + ", " + totalSwipeTime);
	}
	
	void Update () {
		UpdateSwipeDetection();
	}

	void OnSwipeUp() {
		EventDelegate.Execute(onSwipeUp);
	}

	void OnSwipeRight() {
		EventDelegate.Execute(onSwipeRight);
	}

	void OnSwipeDown() {
		EventDelegate.Execute(onSwipeDown);
	}

	void OnSwipeLeft() {
		EventDelegate.Execute(onSwipeLeft);
	}

	void UpdateSwipeDetection() {
		if (simulateTouch) {
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			if (isSwiping) {
				swipeVector += (mousePosition - lastMousePos);
				totalSwipeTime += Time.deltaTime;
				
				if (Input.GetMouseButtonUp(0)) {
					float totalSwipeDistance = swipeVector.magnitude;
					if (totalSwipeTime >= swipeTime && totalSwipeDistance >= swipeDistance) {
						bool horizontal;
						if (Mathf.Abs(mousePosition.x - initialTouchPos.x) > Mathf.Abs(mousePosition.y - initialTouchPos.y)) horizontal = true;
						else horizontal = false;
						
						if (horizontal) {
							if (Input.mousePosition.x > initialTouchPos.x) OnSwipeRight();
							else OnSwipeLeft();
						}
						else {
							if (Input.mousePosition.y > initialTouchPos.y) OnSwipeUp();
							else OnSwipeDown();
						}
					}
					else Debug.Log(totalSwipeDistance + ", " + totalSwipeTime);

					isSwiping = false;
				}
				
				lastMousePos = Input.mousePosition;
			}
			else {
				if (Input.GetMouseButtonDown(0)) {
					isSwiping = true;
					totalSwipeTime = 0;
					initialTouchPos = mousePosition;
					swipeVector = Vector2.zero;
				}
			}
		}
		
		if (isSwiping) {
			if (Input.touches.Length > 0) {
				Touch t = Input.GetTouch(swipeFingerID);
				swipeVector += t.deltaPosition;
				totalSwipeTime += t.deltaTime;
				
				if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Ended) {
					float totalSwipeDistance = swipeVector.magnitude;
					if (totalSwipeTime >= swipeTime && totalSwipeDistance >= swipeDistance) {
						bool horizontal;
						if (Mathf.Abs(t.position.x - initialTouchPos.x) > Mathf.Abs(t.position.y - initialTouchPos.y)) horizontal = true;
						else horizontal = false;
						
						if (horizontal) {
							if (t.position.x > initialTouchPos.x) OnSwipeRight();
							else OnSwipeLeft();
						}
						else {
							if (t.position.y > initialTouchPos.y) OnSwipeUp();
							else OnSwipeDown();
						}
					}

					isSwiping = false;
				}
			}
		}
		else {
			foreach (Touch t in Input.touches) {
				if (t.phase == TouchPhase.Began) {
					isSwiping = true;
					totalSwipeTime = 0;
					swipeFingerID = t.fingerId;
					initialTouchPos = t.position;
					swipeVector = Vector2.zero;
					break;
				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

[CustomEditor(typeof(SwipeEventSystem))]
public class SwipeEventSystemEditor : Editor {
	override public void OnInspectorGUI() {
		base.OnInspectorGUI();

		SwipeEventSystem swipeEventSystem = target as SwipeEventSystem;

		EventDelegateEditorTools.DrawPadding();

		EventDelegateEditorTools.DrawEvents("On Swipe Up Began", swipeEventSystem, swipeEventSystem.onSwipeUpBegan);
		EventDelegateEditorTools.DrawEvents("On Swipe Right Began", swipeEventSystem, swipeEventSystem.onSwipeRightBegan);
		EventDelegateEditorTools.DrawEvents("On Swipe Down Began", swipeEventSystem, swipeEventSystem.onSwipeDownBegan);
		EventDelegateEditorTools.DrawEvents("On Swipe Left Began", swipeEventSystem, swipeEventSystem.onSwipeLeftBegan);

		EventDelegateEditorTools.DrawPadding();
		
		EventDelegateEditorTools.DrawEvents("On Swipe Up Continue", swipeEventSystem, swipeEventSystem.onSwipeUpContinue);
		EventDelegateEditorTools.DrawEvents("On Swipe Right Continue", swipeEventSystem, swipeEventSystem.onSwipeRightContinue);
		EventDelegateEditorTools.DrawEvents("On Swipe Down Continue", swipeEventSystem, swipeEventSystem.onSwipeDownContinue);
		EventDelegateEditorTools.DrawEvents("On Swipe Left Continue", swipeEventSystem, swipeEventSystem.onSwipeLeftContinue);
	
		EventDelegateEditorTools.DrawPadding();

		EventDelegateEditorTools.DrawEvents("On Swipe Up Ended", swipeEventSystem, swipeEventSystem.onSwipeUpEnded);
		EventDelegateEditorTools.DrawEvents("On Swipe Right Ended", swipeEventSystem, swipeEventSystem.onSwipeRightEnded);
		EventDelegateEditorTools.DrawEvents("On Swipe Down Ended", swipeEventSystem, swipeEventSystem.onSwipeDownEnded);
		EventDelegateEditorTools.DrawEvents("On Swipe Left Ended", swipeEventSystem, swipeEventSystem.onSwipeLeftEnded);

		EventDelegateEditorTools.DrawPadding();
		
		EventDelegateEditorTools.DrawEvents("On Swipe Canceled", swipeEventSystem, swipeEventSystem.onSwipeCanceled);
	}
}

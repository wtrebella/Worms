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
		EventDelegateEditorTools.DrawEvents("On Swipe Up", swipeEventSystem, swipeEventSystem.onSwipeUp);
		EventDelegateEditorTools.DrawEvents("On Swipe Right", swipeEventSystem, swipeEventSystem.onSwipeRight);
		EventDelegateEditorTools.DrawEvents("On Swipe Down", swipeEventSystem, swipeEventSystem.onSwipeDown);
		EventDelegateEditorTools.DrawEvents("On Swipe Left", swipeEventSystem, swipeEventSystem.onSwipeLeft);
	}
}

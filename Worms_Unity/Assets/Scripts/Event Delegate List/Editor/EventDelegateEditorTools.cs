using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class EventDelegateEditorTools {
	static public void RegisterUndo (string name, params Object[] objects)
	{
		if (objects != null && objects.Length > 0)
		{
			UnityEditor.Undo.RecordObjects(objects, name);
			
			foreach (Object obj in objects)
			{
				if (obj == null) continue;
				EditorUtility.SetDirty(obj);
			}
		}
	}

	static public void DrawPadding ()
	{
		GUILayout.Space(18f);
	}

	static public void SetLabelWidth (float width)
	{
		EditorGUIUtility.labelWidth = width;
	}

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>
	
	static public bool DrawHeader (string text) { return DrawHeader(text, text, false, false); }
	
	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>
	
	static public bool DrawHeader (string text, string key) { return DrawHeader(text, key, false, false); }
	
	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>
	
	static public bool DrawHeader (string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }
	
	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>
	
	static public bool DrawHeader (string text, string key, bool forceOn, bool minimalistic)
	{
		bool state = EditorPrefs.GetBool(key, true);
		
		if (!minimalistic) GUILayout.Space(3f);
		if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUI.changed = false;
		
		if (minimalistic)
		{
			if (state) text = "\u25BC" + (char)0x200a + text;
			else text = "\u25BA" + (char)0x200a + text;
			
			GUILayout.BeginHorizontal();
			GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
			if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
			GUI.contentColor = Color.white;
			GUILayout.EndHorizontal();
		}
		else
		{
			text = "<b><size=11>" + text + "</size></b>";
			if (state) text = "\u25BC " + text;
			else text = "\u25BA " + text;
			if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
		}
		
		if (GUI.changed) EditorPrefs.SetBool(key, state);
		
		if (!minimalistic) GUILayout.Space(2f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!forceOn && !state) GUILayout.Space(3f);
		return state;
	}
	
	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>
	
	static public SerializedProperty DrawProperty (SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(null, serializedObject, property, false, options);
	}
	
	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>
	
	static public SerializedProperty DrawProperty (string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(label, serializedObject, property, false, options);
	}
	
	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>
	
	static public SerializedProperty DrawPaddedProperty (SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(null, serializedObject, property, true, options);
	}
	
	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>
	
	static public SerializedProperty DrawPaddedProperty (string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(label, serializedObject, property, true, options);
	}
	
	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>
	
	static public SerializedProperty DrawProperty (string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
	{
		SerializedProperty sp = serializedObject.FindProperty(property);
		
		if (sp != null)
		{
			if (padding) EditorGUILayout.BeginHorizontal();
			
			if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
			else EditorGUILayout.PropertyField(sp, options);
			
			if (padding) 
			{
				EventDelegateEditorTools.DrawPadding();
				EditorGUILayout.EndHorizontal();
			}
		}
		return sp;
	}
	
	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>
	
	static public void DrawProperty (string label, SerializedProperty sp, params GUILayoutOption[] options)
	{
		DrawProperty(label, sp, true, options);
	}
	
	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>
	
	static public void DrawProperty (string label, SerializedProperty sp, bool padding, params GUILayoutOption[] options)
	{
		if (sp != null)
		{
			if (padding) EditorGUILayout.BeginHorizontal();
			
			if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
			else EditorGUILayout.PropertyField(sp, options);
			
			if (padding)
			{
				EventDelegateEditorTools.DrawPadding();
				EditorGUILayout.EndHorizontal();
			}
		}
	}

	/// <summary>
	/// Begin drawing the content area.
	/// </summary>
	
	static public void BeginContents () { BeginContents(false); }
	
	static bool mEndHorizontal = false;
	
	/// <summary>
	/// Begin drawing the content area.
	/// </summary>
	
	static public void BeginContents (bool minimalistic)
	{
		if (!minimalistic)
		{
			mEndHorizontal = true;
			GUILayout.BeginHorizontal();
			EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
		}
		else
		{
			mEndHorizontal = false;
			EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
			GUILayout.Space(10f);
		}
		GUILayout.BeginVertical();
		GUILayout.Space(2f);
	}

	/// <summary>
	/// End drawing the content area.
	/// </summary>
	
	static public void EndContents ()
	{
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (mEndHorizontal)
		{
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Space(3f);
	}

	/// <summary>
	/// Draw a list of fields for the specified list of delegates.
	/// </summary>
	
	static public void DrawEvents (string text, Object undoObject, List<EventDelegate> list)
	{
		DrawEvents(text, undoObject, list, null, null, false);
	}
	
	/// <summary>
	/// Draw a list of fields for the specified list of delegates.
	/// </summary>
	
	static public void DrawEvents (string text, Object undoObject, List<EventDelegate> list, bool minimalistic)
	{
		DrawEvents(text, undoObject, list, null, null, minimalistic);
	}
	
	/// <summary>
	/// Draw a list of fields for the specified list of delegates.
	/// </summary>
	
	static public void DrawEvents (string text, Object undoObject, List<EventDelegate> list, string noTarget, string notValid, bool minimalistic)
	{
		if (!EventDelegateEditorTools.DrawHeader(text, text, false, minimalistic)) return;
		
		if (!minimalistic)
		{
			EventDelegateEditorTools.BeginContents(minimalistic);
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			
			EventDelegateEditor.Field(undoObject, list, notValid, notValid, minimalistic);
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			EventDelegateEditorTools.EndContents();
		}
		else EventDelegateEditor.Field(undoObject, list, notValid, notValid, minimalistic);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace maleric.Dialogue
{
	public abstract class DialogueSequenceConfig : ScriptableObject
	{
		[SerializeField] private bool _isOneTimePlay = true;

		public event Action<DialogueSequenceConfig> OnPlayRequest;
		public event Action<DialogueSequenceConfig> OnAddToQueueRequest;

		public abstract int LineCount { get; }
		public bool IsOneTimePlay => _isOneTimePlay;

		public abstract IDialogueLine GetLineAt(int index);

		public abstract void EDITOR_SetDialogueLines(List<IDialogueLine> lines);

		public void PlaySequence()
		{
			if (Application.isPlaying) OnPlayRequest?.Invoke(this);
		}

		public void AddToQueue()
		{
			if (Application.isPlaying) OnAddToQueueRequest?.Invoke(this);
		}
	}


	/// <summary>
	/// Extend this class with using custom dialogue line that can be localized, set or downloaded differently
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DialogueSequenceConfig<T> : DialogueSequenceConfig where T : IDialogueLine
	{
		[SerializeField] private List<T> _lines = new List<T>();

		public override int LineCount => _lines.Count;

		public override void EDITOR_SetDialogueLines(List<IDialogueLine> lines)
		{
			int count = lines.Count;
			_lines = new List<T>(count);
			for (int i = 0; i < count; i++) _lines.Add((T)lines[i]);
		}

		public override IDialogueLine GetLineAt(int index)
		{
			return _lines[index];
		}
	}

#if UNITY_EDITOR

	[CanEditMultipleObjects]
	[CustomEditor(typeof(DialogueSequenceConfig), true)]
	public class DialogueCharacterDefinitionEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			var myScript = (DialogueSequenceConfig)this.target;

			GUI.enabled = Application.isPlaying;
			if (GUILayout.Button("Test Dialogue"))
			{
				Dialogue.Initialize();
				myScript.PlaySequence();
			}

			if (GUILayout.Button("Add Dialoge To Queue"))
			{
				Dialogue.Initialize();
				myScript.AddToQueue();
			}
			GUI.enabled = true;


		}
	}
#endif
}
using System;
using UnityEngine;

namespace maleric.Dialogue
{
	[Serializable]
	public struct DialogueLine : IDialogueLine
	{
		public DialoguePosition CharacterPosition => characterScreenPosition;
		DialogueCharacterExpressionPair IDialogueLine.CharacterData => characterData;
		public bool CanSkip => !disableSkip || Application.isEditor;
		public string LineText => lineText;

		[SerializeField] private DialoguePosition characterScreenPosition;
		[SerializeField] private DialogueCharacterExpressionPair characterData;
		[SerializeField] [Space] private bool disableSkip;
		[SerializeField] private string lineText; // Somewhere this will be localized key
	}
}
using System;
using UnityEngine;

namespace maleric.Dialogue
{
	[Serializable]
	public class DialogueCharacterExpressionPair
	{
		public DialogueCharacterExpressionPair() { }
		public DialogueCharacterExpressionPair(DialogueCharacterConfig config, int expressionIndex = 0) { CharacternName = config.Name; ExpressionIndex = expressionIndex; Character = config; }

		public string CharacternName;
		public int ExpressionIndex;

		public DialogueCharacterConfig Character;

		public string CharacterName { get => Character.Name; }
		public Sprite ExpressionSprite { get => Character.GetExpression(ExpressionIndex); }
	}
}
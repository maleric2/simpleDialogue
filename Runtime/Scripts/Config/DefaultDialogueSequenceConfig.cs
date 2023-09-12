using System.Text.RegularExpressions;
using System;
using UnityEngine;

namespace maleric.Dialogue
{
	[CreateAssetMenu(menuName = "maleric/Dialogue/Default Dialogue Sequence", fileName = "dialogue_")]
	public class DefaultDialogueSequenceConfig : DialogueSequenceConfig<DialogueLine>
	{

		public override bool EDITOR_TryToParseToDialogueLine(string text, out IDialogueLine line)
		{
			Match match = null;
			line = new DialogueLine();
			if (!string.IsNullOrEmpty(text))
			{
				string pattern = @"^([\w\d\s]+)\s\(([^)]+)\):\s(.+)$";
				match = Regex.Match(text, pattern);

				if (match.Success)
				{
					var characterName = match.Groups[1].Value.Trim().ToLower();
					var characterExpression = match.Groups[2].Value.Trim().ToLower();

					var characterDefinitions = Resources.LoadAll<DialogueCharacterConfig>("");
					DialogueCharacterConfig selectedCharacter = null;
					int selectedExpressions = 0;

					foreach (var character in characterDefinitions)
					{
						if (character.Name.Trim().ToLower().StartsWith(characterName))
						{
							selectedCharacter = character;
							break;
						}
					}

					if (selectedCharacter != null && characterExpression.Length > 0)
					{
						for (int i = 0; i < selectedCharacter.Expressions.Length; i++)
						{
							DialogueCharacterExpression expressions = selectedCharacter.Expressions[i];
							if (expressions.Name.Trim().ToLower().StartsWith(characterExpression))
							{
								selectedExpressions = i;
								break;
							}
						}
					}

					DialogueCharacterExpressionPair pair;
					if (selectedCharacter != null) pair = new DialogueCharacterExpressionPair(selectedCharacter, selectedExpressions);
					else pair = new DialogueCharacterExpressionPair();
					line = new DialogueLine(match.Groups[3].Value, pair, DialoguePosition.Left, true);
				}
			}

			return match != null && match.Success;
		}
	}

}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace maleric.Dialogue
{
	[CustomPropertyDrawer(typeof(DialogueCharacterExpressionPair))]
	public class CharacterExpressionAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			DrawProperty(position, property, label);
		}

		public static void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);

			SerializedProperty characterNameProp = property.FindPropertyRelative("CharacternName");
			SerializedProperty expressionProp = property.FindPropertyRelative("ExpressionIndex");
			SerializedProperty definitionRefProp = property.FindPropertyRelative("Character");

			var characterDefinitions = Resources.LoadAll<DialogueCharacterConfig>("");

			List<string> definitions = new List<string>();
			List<string> expressions = new List<string>();
			int selectedDefinitionIndex = 0;
			if (characterDefinitions != null)
			{
				for (int i = 0; i < characterDefinitions.Length; i++)
				{
					definitions.Add(characterDefinitions[i].Name);
					if (characterDefinitions[i].Name.Equals(characterNameProp.stringValue))
					{
						selectedDefinitionIndex = i;
					}
				}
			}

			if (characterDefinitions.Length > selectedDefinitionIndex)
			{
				for (int i = 0; i < characterDefinitions[selectedDefinitionIndex].Expressions.Length; i++)
				{
					expressions.Add(characterDefinitions[selectedDefinitionIndex].Expressions[i].Name);
				}
			}
			else
			{
				expressions.Add("None");
			}

			//EditorGUI.ObjectField(new Rect(position.x, position.y, position.width * 0.62f, position.height), characterDefProp); // Reference
			selectedDefinitionIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width * 0.71f, position.height), label.text, selectedDefinitionIndex, definitions.ToArray());
			characterNameProp.stringValue = definitions[selectedDefinitionIndex];
			definitionRefProp.objectReferenceValue = characterDefinitions[selectedDefinitionIndex];

			expressionProp.intValue = EditorGUI.Popup(new Rect(position.x + position.width * 0.7f, position.y, position.width * 0.3f, position.height),
				expressionProp.intValue, expressions.ToArray()); // Dropdown

			EditorGUI.EndProperty();
		}

		/*public static DialogueCharacterExpressionId DrawEditorProperty(DialogueCharacterExpressionId current, string label)
        {
            var soDbService = Kickstarter.EditTimeServiceLocator.Provide<ScriptableObjectDatabaseService>();

            List<string> expressions = new List<string>();
            if (current.CharacterDefinition != null)
            {
                for (int i = 0; i < current.CharacterDefinition.Expressions.Length; i++)
                {
                    expressions.Add(current.CharacterDefinition.Expressions[i].Name);
                }
            }
            else
            {
                expressions.Add("None");
            }

            DialogueCharacterDefinition definition = (DialogueCharacterDefinition)EditorGUILayout.ObjectField(current.CharacterDefinition, typeof(DialogueCharacterDefinition));
            int index = EditorGUILayout.Popup(label, current.ExpressionIndex, expressions.ToArray());

            return new DialogueCharacterExpressionId() { CharacterDefinition = definition, ExpressionIndex = index };
        }*/
	}
}

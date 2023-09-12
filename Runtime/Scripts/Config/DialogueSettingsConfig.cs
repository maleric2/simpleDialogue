using UnityEngine;

namespace maleric.Dialogue
{
	[CreateAssetMenu(menuName = "maleric/Dialogue/Dialogue Settings Config", fileName = "DialogueSettingsConfig")]
	public class DialogueSettingsConfig : ScriptableObject
	{

		public GameObject DialogueViewPrefab;

		[Tooltip("Toggle Off If you use custom way to play dialogues")]
		public bool UseSingleton = true;

		[Tooltip("Toggle Off if you use custom initialization with custom logic or with custom call order")]
		public bool AutoInitializeSingleton = true;

		[Tooltip("Folder inside Resources. Default value is empty which will get all dialogues")]
		public string DialogueSequencePath = "";

	}
}
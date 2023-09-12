using maleric.Dialogue;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
	[SerializeField]
	private DialogueSequenceConfig startDialogue;

	private void Start()
	{
		if (startDialogue != null)
		{
			Dialogue.Instance.PlayImmediately(startDialogue);
		}
	}
}

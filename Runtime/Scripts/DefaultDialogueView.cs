using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace maleric.Dialogue
{
	public class DefaultDialogueView : ADialogueView
	{

		[SerializeField] private TMPro.TextMeshProUGUI dialogueText;
		[SerializeField] private TMPro.TextMeshProUGUI speakerNameText;

		[SerializeField] private Image leftCharacterImage;
		[SerializeField] private Image rightCharacterImage;

		[SerializeField] private Button[] continueButtons;
		[SerializeField] private Toggle autoPlayToggle;

		protected override bool isAutoPlayOn => autoPlayToggle != null ? autoPlayToggle.isOn : false;

		private async void OnEnable()
		{
			for (int i = 0; i < continueButtons.Length; i++) continueButtons[i].onClick.AddListener(OnContinueClick);
			autoPlayToggle.onValueChanged.AddListener(OnAutoPlayToggled);

			if (dialogueText) dialogueText.text = "";
			if (speakerNameText) speakerNameText.text = "";

			if (leftCharacterImage) leftCharacterImage.enabled = false;
			if (rightCharacterImage) rightCharacterImage.enabled = false;

			if (isAutoPlayOn) await AutoPlay();
		}

		private void OnDisable()
		{
			for (int i = 0; i < continueButtons.Length; i++) continueButtons[i].onClick.RemoveListener(OnContinueClick);
			autoPlayToggle.onValueChanged.RemoveListener(OnAutoPlayToggled);
		}

		private async void OnAutoPlayToggled(bool arg0)
		{
			if (isAutoPlayOn) await AutoPlay();
		}

		public override void SetLine(IDialogueLine dialogueLine)
		{
			speakerNameText.text = dialogueLine.CharacterData.CharacterName;
			switch (dialogueLine.CharacterPosition)
			{
				case DialoguePosition.Left:
					leftCharacterImage.sprite = dialogueLine.CharacterData.ExpressionSprite;
					leftCharacterImage.enabled = true;
					rightCharacterImage.enabled = false;
					break;

				case DialoguePosition.Right:
					rightCharacterImage.sprite = dialogueLine.CharacterData.ExpressionSprite;
					rightCharacterImage.enabled = true;
					leftCharacterImage.enabled = false;
					break;
			}

			base.SetLine(dialogueLine);
		}

		protected override void AnimateSetText(string text, int visibleCharactersCount)
		{
			//Debug.Log(text);
			dialogueText.text = text;
			dialogueText.maxVisibleCharacters = visibleCharactersCount;
		}
	}
}
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace maleric.Dialogue
{
	public interface IDialogueView
	{
		/// <summary>
		/// Event callback which defines when to call next Dialogue
		/// </summary>
		public event Action OnContinueDialogueClick;

		public bool IsActive { get; }

		/// <summary>
		/// Setting a text for this dialogue box. Note View itself should handle text skipping and setting character information
		/// </summary>
		/// <param name="dialogueLine"></param>
		public void SetLine(IDialogueLine dialogueLine);

		public void SetActive(bool active);
	}

	public abstract class ADialogueView : UnityEngine.MonoBehaviour, IDialogueView
	{
		public event Action OnContinueDialogueClick;

		public bool IsActive => isActive;

		[SerializeField] private GameObject Container;

		[Header("Text reveal parameters")]
		[SerializeField] private float timeBetweenCharacters = 0.1f;
		[SerializeField] private float timeBetweenDialogues = 1f;

		protected abstract bool isAutoPlayOn { get; }
		protected IDialogueLine dialogueLine;
		protected bool isActive = false;
		protected Coroutine textAnimationCoroutine;

		protected void OnContinueClick()
		{
			if (textAnimationCoroutine != null)
			{
				if (dialogueLine.CanSkip)
				{
					StopCoroutine(textAnimationCoroutine);
					textAnimationCoroutine = null;
					AnimateSetText(dialogueLine.LineText, dialogueLine.LineText.Length);
				}
			}
			else
			{
				OnContinueDialogueClick?.Invoke();
			}
		}

		public virtual void SetLine(IDialogueLine dialogueLine)
		{
			this.dialogueLine = dialogueLine;

			if (textAnimationCoroutine != null) StopCoroutine(textAnimationCoroutine);
			textAnimationCoroutine = StartCoroutine(AnimateSetText(dialogueLine.LineText));
		}

		protected async Task AutoPlay()
		{
			if (isAutoPlayOn)
			{
				int autoPlayWaitTime = (int)(timeBetweenDialogues * 1000);
				await Task.Delay(autoPlayWaitTime);
				OnContinueDialogueClick?.Invoke();
			}
		}

		protected IEnumerator AnimateSetText(string text)
		{
			var wait = new WaitForSecondsRealtime(timeBetweenCharacters);
			AnimateSetText(text, 0);
			for (int i = 0; i < text.Length; i++)
			{
				AnimateSetText(text, i);
				yield return wait;
			}
			
			AnimateSetText(text, text.Length);

			textAnimationCoroutine = null;

			AutoPlay();
		}

		protected abstract void AnimateSetText(string text, int visibleCharactersCount);

		public virtual void SetActive(bool active)
		{
			this.isActive = active;
			Container.SetActive(active);
		}
	}
}
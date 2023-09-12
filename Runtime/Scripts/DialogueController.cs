using System;
using UnityEngine;

namespace maleric.Dialogue
{
	/// <summary>
	/// Controller that handles the View and callbacks from it
	/// </summary>
	public class DialogueController
	{
		protected IDialogueView view;

		protected DialogueService service;

		public DialogueController(DialogueConfig config, DialogueService dialogueService)
		{
			service = dialogueService;

			var defaultView = GameObject.FindObjectOfType<ADialogueView>(true);

			if (defaultView != null)
			{
				view = defaultView;
			}
			else
			{
				var viewGameObject = GameObject.Instantiate(config.DialogueViewPrefab);
				view = viewGameObject.GetComponent<IDialogueView>();
			}

			view.SetActive(false);

			dialogueService.OnLineChange += DialogueService_OnLineChange;
			dialogueService.OnSequenceEnd += DialogueService_OnSequenceEnd;

			view.OnContinueDialogueClick += View_OnContinueDialogueClick;
		}

		private void DialogueService_OnSequenceEnd()
		{
			view.SetActive(false);
		}

		private void DialogueService_OnLineChange(DialogueSequenceConfig arg1, int arg2)
		{
			if (!view.IsActive) view.SetActive(true);

			view.SetLine(arg1.GetLineAt(arg2));
		}

		private void View_OnContinueDialogueClick()
		{
			service.NextLine();
		}
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace maleric.Dialogue
{
	/// <summary>
	/// Brain Of Dialogue System which should be used in custom logic in service or controller or in somethign similar
	/// </summary>
	public class DialogueService
	{
		public int DialoguesInSequence => _sequencesQueue.Count;
		public event Action<DialogueSequenceConfig, int> OnLineChange;
		public event Action OnSequenceEnd;

		private int _activeLineIndex;
		private DialogueSequenceConfig _activeSequence;
		private Queue<DialogueSequenceToPlayInData> _sequencesQueue = new Queue<DialogueSequenceToPlayInData>();
		private Nullable<DialogueSequenceToPlayInData> _activeSequenceFromQueue;

		private bool _isEnabled = false;
		private int _disabledCount = 1;

		private Func<string, bool> _checkWasDialoguePlayed;

		public struct DialogueSequenceToPlayInData
		{
			public DialogueSequenceConfig Sequence;
			public Action OnPlayAction;
			public Action OnPlayComplete;

			public DialogueSequenceToPlayInData(DialogueSequenceConfig sequence, Action playAction = null, Action completeAction = null)
			{
				Sequence = sequence;
				OnPlayAction = playAction;
				OnPlayComplete = completeAction;
			}
		}

		/// <summary>
		/// Constructor for DialogueSystem. Also implement bool CheckWasDialogeuPlayed(string dialogueName) method
		/// </summary>
		/// <param name="checkWasDialoguePlayed">Implement it and load saved dialogues to avoid repeating old dialogues</param>
		public DialogueService(Func<string, bool> checkWasDialoguePlayed)
		{
			_checkWasDialoguePlayed = checkWasDialoguePlayed;
			if (_checkWasDialoguePlayed == null) Debug.LogWarning("Dialogue System checkWasDialoguePlayed is not implemented");
			_disabledCount = 1;
		}

		public void Enable(bool enableDialogues)
		{
			if (enableDialogues) _disabledCount = Mathf.Clamp(_disabledCount - 1, 0, _disabledCount);
			else _disabledCount++;

			_isEnabled = _disabledCount <= 0;
			if (_isEnabled) StartSequenceFromQueue();
		}

		/// <summary>
		/// Adding multiple dialogues in queue and they will be played as soon other dialogues are completed
		/// </summary>
		/// <param name="sequences"></param>
		/// <param name="onPlayAction"></param>
		/// <param name="onCompleteAction"></param>
		public void AddDialogueSequencesInPlayQueue(DialogueSequenceConfig[] sequences, Action onPlayAction = null, Action onCompleteAction = null)
		{
			for (int i = 0; i < sequences.Length; i++) AddDialogueSequenceInPlayQueue(sequences[i], onPlayAction, onCompleteAction, i == sequences.Length - 1);
		}

		/// <summary>
		/// Adding dialogue in queue and they will be played as soon other dialogues are completed
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="onPlayAction"></param>
		/// <param name="onCompleteAction"></param>
		/// <param name="startQueueCheck"></param>
		public void AddDialogueSequenceInPlayQueue(DialogueSequenceConfig sequence, Action onPlayAction = null, Action onCompleteAction = null, bool startQueueCheck = true)
		{
			if (sequence != null)
			{
				_sequencesQueue.Enqueue(new DialogueSequenceToPlayInData(sequence, onPlayAction, onCompleteAction));
			}
			//Debug.LogError("Dialogue Sequence: " + _sequencesQueue.Count + ", Actions " + _sequencesInQueuePlayAction.Count);
			if (startQueueCheck) StartSequenceFromQueue();
		}


		/// <summary>
		/// Starts single dialogue sequence immediately
		/// </summary>
		/// <param name="sequence"></param>
		/// <returns></returns>
		public bool PlayDialogueSequence(DialogueSequenceConfig sequence, Action onCompleteAction = null)
		{
			var previusActiveLineIndex = _activeLineIndex;
			var previusActiveSequence = _activeSequenceFromQueue;

			_activeSequenceFromQueue = new DialogueSequenceToPlayInData()
			{
				OnPlayAction = null,
				OnPlayComplete = () =>
				{
					onCompleteAction?.Invoke();

					// Returning to previus action
					if (previusActiveSequence != null && previusActiveSequence.HasValue)
					{
						_activeSequenceFromQueue = previusActiveSequence;
						_activeSequence = previusActiveSequence.Value.Sequence;
						_activeLineIndex = Mathf.Max(0, previusActiveLineIndex - 1);
						NextLine();

					}
				},
				Sequence = sequence
			};
			return StartSequence(sequence);
		}

		private bool StartSequence(DialogueSequenceConfig sequence)
		{
			if (sequence.IsOneTimePlay && _checkWasDialoguePlayed != null && !_checkWasDialoguePlayed(sequence.name))
				return false;

			_activeSequence = sequence;

			_activeLineIndex = -1; // next line increments by one
			NextLine();
			return true;
		}

		private void StartSequenceFromQueue()
		{
			if (_activeSequence == null && _isEnabled && _sequencesQueue.Count > 0)
			{
				_activeSequenceFromQueue = _sequencesQueue.Dequeue();
				//Debug.LogError("Dialogue Starting From Sequence Current Count: " + _sequencesInQueuePlayAction.Count);
				if (StartSequence(_activeSequenceFromQueue.Value.Sequence)) _activeSequenceFromQueue.Value.OnPlayAction?.Invoke();
			}
			/*else
			{
				Debug.LogError("Dialogue Starting From Sequence Failed _activeSequnece " + _activeSequence + " !=null || Sequence Count  " + _sequencesInQueuePlayAction.Count + " == 0");
			}*/
		}

		public void NextLine()
		{
			_activeLineIndex++;
			if (IsLineAvailable())
			{
				DisplayLine();
			}
			else
			{
				if (_activeSequenceFromQueue.HasValue)
				{
					_activeSequenceFromQueue.Value.OnPlayComplete?.Invoke();
					_activeSequenceFromQueue = null;
				}

				OnSequenceEnd?.Invoke();
				_activeSequence = null;

				//OnLineChange?.Invoke(new DialogueSequenceDefinition.DialogueLine(), true); // empty line
				StartSequenceFromQueue();
			}
		}

		public bool IsLastLine()
		{
			return _activeLineIndex >= _activeSequence.LineCount - 1;
		}

		public bool IsLineAvailable() { return _activeSequence != null && _activeSequence.LineCount > _activeLineIndex; }

		private void DisplayLine()
		{
			OnLineChange?.Invoke(_activeSequence, _activeLineIndex);
		}

		public void CancelSequence()
		{
			_activeSequence = null;
		}

	}
}
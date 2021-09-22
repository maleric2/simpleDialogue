using System;
using UnityEngine;


namespace maleric.Dialogue
{


	public class Dialogue
	{
		public const string DIALOGUE_SINGLETON_DISABLED_ERROR = "Dialogue Singleton is disabled";

		public DialogueSequenceConfig[] DialogueSequenceConfigs => dialogueSequenceConfigs;

		public static Dialogue Instance
		{
			get
			{
				if (instance == null)
				{
					instance = CreateInstance();
				}

				if (instance != null)
				{
					if (instance.config.UseSingleton) return instance;
					else Debug.LogError(DIALOGUE_SINGLETON_DISABLED_ERROR);
				}

				return null;
			}
		}

		public static bool Initialize()
		{
			Dialogue instance = Instance;
			return instance != null;
		}

		private static Dialogue CreateInstance()
		{
			Dialogue instance = null;
			var configs = Resources.LoadAll<DialogueConfig>("");
			if (configs != null && configs.Length > 0)
			{
				instance = new Dialogue(configs[0]);
			}
			return instance;
		}

		private static Dialogue instance;
		private DialogueConfig config;
		private DialogueSequenceConfig[] dialogueSequenceConfigs;

		private bool isInitialized;
		private DialogueController controller;
		private DialogueService service;


		// Add logic similar to Kickstarter
		// Initialize it any way on game start
		private Dialogue(DialogueConfig config)
		{
			this.config = config;
			this.dialogueSequenceConfigs = Resources.LoadAll<DialogueSequenceConfig>(config.DialogueSequencePath);

#if UNITY_EDITOR
			// Playing it inside the editor
			for (int i = 0; i < this.dialogueSequenceConfigs.Length; i++)
			{
				var dialogue = this.dialogueSequenceConfigs[i];
				dialogue.OnAddToQueueRequest += Dialogue_OnAddToQueueRequest;
				dialogue.OnPlayRequest += Dialogue_OnPlayRequest;
			}
#endif
		}

		~Dialogue()
		{
#if UNITY_EDITOR
			for (int i = 0; i < this.dialogueSequenceConfigs.Length; i++)
			{
				var dialogue = this.dialogueSequenceConfigs[i];
				dialogue.OnAddToQueueRequest -= Dialogue_OnAddToQueueRequest;
				dialogue.OnPlayRequest -= Dialogue_OnPlayRequest;
			}
#endif
		}

		private void Dialogue_OnPlayRequest(DialogueSequenceConfig obj)
		{
			PlayImmediately(obj);
		}

		private void Dialogue_OnAddToQueueRequest(DialogueSequenceConfig obj)
		{
			Play(obj);
		}

		public void PlayImmediately(DialogueSequenceConfig sequenceConfig, Action onCompleteAction = null)
		{
			// Auto Initializes if needed
			if (!isInitialized) AutoInitialize();

			// Calls service to start sequence
			service.PlayDialogueSequence(sequenceConfig, onCompleteAction);
		}

		public void Play(DialogueSequenceConfig sequenceConfig, Action onPlayAction = null, Action onCompleteAction = null)
		{
			// Auto Initializes if needed
			if (!isInitialized) AutoInitialize();

			// Calls service to start sequence
			service.AddDialogueSequenceInPlayQueue(sequenceConfig, onPlayAction, onCompleteAction);

		}

		private void AutoInitialize()
		{
			if (config.AutoInitializeSingleton)
			{
				Initialize(null);
			}
		}


		/// <summary>
		/// Initialize Dialogue. If AutoInitialize is enabled it's called automatically. 
		/// To enable additional feature, implement checkWasDialoguePlayed to load wich dialogues were played.
		/// </summary>
		/// <param name="checkWasDialoguePlayed">Implement it and load saved dialogues to avoid repeating old dialogues</param>
		public void Initialize(Func<string, bool> checkWasDialoguePlayed)
		{
			if (!isInitialized)
			{
				service = new DialogueService(checkWasDialoguePlayed);
				controller = new DialogueController(config, service);
				service.Enable(true);
				isInitialized = true;
			}
		}
	}
}
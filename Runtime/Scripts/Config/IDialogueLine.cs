
namespace maleric.Dialogue
{
	public interface IDialogueLine
	{
		DialoguePosition CharacterPosition { get; }
		DialogueCharacterExpressionPair CharacterData { get; }
		public bool CanSkip { get; }
		public string LineText { get; }
	}
}
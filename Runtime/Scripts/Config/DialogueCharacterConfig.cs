using UnityEngine;

[CreateAssetMenu(menuName = "maleric/Dialogue/Character")]
public class DialogueCharacterConfig : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private DialogueCharacterExpression[] _expressions;

    public string Name => _name;

    public DialogueCharacterExpression[] Expressions => _expressions;

#if UNITY_EDITOR
    public void EDITOR_SetName(string name)
    {
        _name = name;
    }
#endif

    public Sprite GetExpression(int index)
    {
        if (_expressions != null && _expressions.Length > index)
            return _expressions[index].Sprite;
        else
            return null;
    }
}

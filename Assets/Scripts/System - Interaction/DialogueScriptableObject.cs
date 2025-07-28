using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class DialogueScriptableObject : ScriptableObject
{
    [TextArea(1, 10)] public string DialogueText;
}

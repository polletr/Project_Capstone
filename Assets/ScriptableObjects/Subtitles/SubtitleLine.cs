using UnityEngine;

[CreateAssetMenu(fileName = "SubtitleLine", menuName = "Scriptable Objects/SubtitleLine")]
public class SubtitleLine : ScriptableObject
{
        [TextArea] public string text;
        public float duration = 3f;
        public Color textColor = Color.white;
}

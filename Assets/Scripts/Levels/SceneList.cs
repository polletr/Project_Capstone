using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SceneList")]
public class SceneList : ScriptableObject
{
    private static SceneList _instance;

    public static SceneList Instance
    {
        get
        {
            // If the instance is not already set, create a new instance
            if (_instance == null)
            {
                Debug.LogWarning("SceneList instance is null Create new Asset");
            }

            // Return the instance
            return _instance;
        }
    }
}
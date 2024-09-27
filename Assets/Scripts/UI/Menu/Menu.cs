using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("Menu Start Up"),SerializeField]
    protected bool _startActive;

    [Header("Menu Game Objects")]
    [SerializeField]
    protected GameObject _currentMenu;

    protected bool _isMusted;

    //Menu Management

    public void OnToggleCurrentMenu() => _currentMenu.SetActive(!_currentMenu.activeSelf);

    public void OnQuitGame()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
/*#elif UNITY_WEBGL
            CloseTab();*/
#else
            Application.Quit();
#endif
    }

    
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    public UnityEvent OnPause;
    public UnityEvent OnResume = new();

    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip buttonClickClip;

    public bool IsPaused { get; private set; }
    private float _initialTimeScale;
    private void Awake()
    {
        IsPaused = false;
        _currentMenu.gameObject.SetActive(IsPaused);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnButtonClick()
    {
        //AudioManager.Instance.PlayAudio(buttonClickClip);
    }
   
    public void OnTogglePauseMenu()
    {
        IsPaused = !IsPaused;
        _currentMenu.gameObject.SetActive(IsPaused);

        AudioManagerFMOD.Instance.PauseAllAudio(IsPaused);


        //AudioManager.Instance.PlayAudio(pauseClip);

        Cursor.lockState = IsPaused? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsPaused;

        if (IsPaused)
        {
            _initialTimeScale = Time.timeScale;
            //AudioManager.Instance.PauseBGAudio();
            Time.timeScale = 0; 
            OnPause.Invoke(); 
        }
        else
        {
            Time.timeScale = _initialTimeScale;
            //AudioManager.Instance.ResumeBGAudio();
           InputManager.Instance.EnablePlayerInput();
            Debug.Log("Resuming");
        }

    }


    public void OnLoadMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }
}

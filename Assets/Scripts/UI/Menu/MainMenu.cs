using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField]
    private GameObject creditsMenu;
    [SerializeField]
    private GameObject controlsMenu;

    public GameEvent gameEvent;
    

    private void Start()
    {
        _startActive = true;
    }

    public void OnPlay() 
    {

        Cursor.lockState = CursorLockMode.Locked;//Move this from here later
        Cursor.visible = false;//Move this from here later
        SceneManager.LoadScene("GameScene");
    } 

    public void PlayClickSound()
    {
        //play click sound
    }

    public void OnGiveFeedback()
    {
        Application.OpenURL("https://forms.gle/9ptFvdmDXz5eSe5y9");
    }

/*    public void OnToggleMute()
    {
        _isMusted = !_isMusted;
        MuteButtonImage.sprite = !_isMusted ? MuteSprite : UnMuteSprite;
    }
*/
}

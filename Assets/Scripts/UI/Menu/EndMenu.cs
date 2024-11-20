using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : Menu
{
    
    
    [SerializeField] private UIAnimator blackScreenUIAnimator;
    
    private void Start() => blackScreenUIAnimator.FadeAnimate();

    public void ReturnToMainMenu()
    {
        blackScreenUIAnimator.OnAnimateFinished.AddListener(LoadMainMenu);
        blackScreenUIAnimator.FadeAnimate();
    }

    private void LoadMainMenu() => SceneManager.LoadScene(0);
}
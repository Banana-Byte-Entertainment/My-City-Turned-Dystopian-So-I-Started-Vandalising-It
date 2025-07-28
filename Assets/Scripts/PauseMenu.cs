using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string CurrentScene;
    public TransitionManager transitionManager;

    public void Play()
    {
        SceneManager.LoadScene(CurrentScene);
    }

    public void Quit()
    {
        transitionManager.FadeToBlack("TitleScreen");
    }

    public void Creds()
    {
        SceneManager.LoadScene("Creds");
    }
}

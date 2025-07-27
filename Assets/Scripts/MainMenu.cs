using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Variables
    public string sceneName;
    public TransitionManager transitionManager;

    private void Start()
    {
        transitionManager.Hide();
    }

    public void Play()
    {
        // Do transition to the game scene
        transitionManager.FadeToBlack(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string CurrentScene;
    public void Play()
    {
        SceneManager.LoadScene(CurrentScene);
    }

    public void Quit()
    {
        SceneManager.LoadScene("Start");
    }
}

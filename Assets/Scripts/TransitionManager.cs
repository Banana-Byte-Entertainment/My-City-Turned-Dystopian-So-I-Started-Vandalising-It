using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    // Variables
    private bool fadeToBlack = false;
    //private bool fadeFromBlack = false;

    [SerializeField] private float fadeSpeed; // Speed of the fade effect

    private Color startColour;
    private Color endColour;

    private string sceneName;

    [SerializeField] private Image image;
    [SerializeField] private GameObject child;

    // Update is called once per frame
    private void Update()
    {
        if (fadeToBlack)
        {
            child.SetActive(true);

            // Gradually change the color from startColour to endColour
            image.color = Color.Lerp(image.color, endColour, fadeSpeed * Time.deltaTime);

            // If the color is close enough to endColour, set it to endColour and stop fading
            if (Vector4.Distance(image.color, endColour) < 0.01f)
            {
                image.color = endColour;
                fadeToBlack = false;

                LoadScene();
            }
        }
        //else if (fadeFromBlack)
        //{
        //    // Gradually change the color from startColour to endColour
        //    image.color = Color.Lerp(image.color, endColour, fadeSpeed * Time.deltaTime);

        //    // If the color is close enough to endColour, set it to endColour and stop fading
        //    if (Vector4.Distance(image.color, endColour) < 0.01f)
        //    {
        //        image.color = endColour;
        //        fadeFromBlack = false;

        //        child.SetActive(false);
        //    }
        //}
    }

    public void FadeToBlack(string sceneName)
    {
        this.sceneName = sceneName;

        startColour = Color.clear;
        endColour = Color.black;

        image.color = startColour;

        //fadeFromBlack = false;
        fadeToBlack = true;
    }

    //public void FadeFromBlack()
    //{
    //    image.color = Color.black;

    //    startColour = Color.black;
    //    endColour = Color.clear;

    //    fadeToBlack = false;
    //    fadeFromBlack = true;
    //}

    //IEnumerator Wait(float duration)
    //{
    //    yield return new WaitForSeconds(duration);
    //}

    public void Hide()
    {
        image.color = Color.clear;
        child.SetActive(false);
    }

    public void Show()
    {
        image.color = Color.black;
        child.SetActive(true);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}

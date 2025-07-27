using TMPro;
using UnityEngine;

public class GraffitiCounter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI counterText;
    public int count = 0;
    void Start()
    {
        counterText.text = count.ToString() + "/7";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addOneCount()
    {
        count += 1;
        counterText.text = count.ToString() + "/7";
    }
}

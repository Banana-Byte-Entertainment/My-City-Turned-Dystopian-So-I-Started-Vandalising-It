using UnityEngine;
using TMPro;
using System;

public class PlayerScore : MonoBehaviour
{
    private float score;
    private int combo;
    // public TextMeshProUGUI textScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetTextScore();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetTextScore()
    {
        // textScore.text = "Score: " + (int)Math.Ceiling(score);
        // Debug.Log("Score: " + (int)Math.Ceiling(score));
    }

    public void UpdateScore(float change)
    {
        score += change;
        SetTextScore();
    }

    public void PerformTrick(float trickScore, string trickName)
    {
        UpdateScore(trickScore);
        combo++;
        Debug.Log($"Performed trick: {trickName}, Score: {trickScore}, Combo: {combo}");
    }

    public void ResetScore()
    {
        UpdateScore(-score);
    }
}

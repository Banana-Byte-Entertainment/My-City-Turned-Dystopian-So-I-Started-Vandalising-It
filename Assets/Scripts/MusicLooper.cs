using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class MusicLooper : MonoBehaviour
{
    public AudioResource[] musicFiles;
    public float delayBetweenMusic = 60;
    private float delayTimer;
    public AudioSource audioSource;
    private Queue<int> lastPlayed;
    public int repetitionAvoidanceNumber = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        delayTimer = delayBetweenMusic;
        lastPlayed = new Queue<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
            delayTimer -= Time.deltaTime;

        if (delayTimer <= 0)
        {
            var rand = Random.Range(0, musicFiles.Length);
            while (lastPlayed.Contains(rand))
                rand = Random.Range(0, musicFiles.Length);
            if (lastPlayed.Count != 0 && lastPlayed.Count == repetitionAvoidanceNumber) lastPlayed.Dequeue();
            lastPlayed.Enqueue(rand);
            audioSource.resource = musicFiles[rand];
            audioSource.Play();
            delayTimer = delayBetweenMusic;
        }
    }
}

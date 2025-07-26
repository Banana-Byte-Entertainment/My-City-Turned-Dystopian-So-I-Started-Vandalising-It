using System.Collections.Generic;
using UnityEngine;

public class Playlist : MonoBehaviour
{
    public static Playlist instance;
    public List<string> songs = new List<string>(); // Use List<string> instead of array

    private void Awake()
    {
        // Singleton pattern setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Add(string s)
    {
        songs.Add(s);
    }
}
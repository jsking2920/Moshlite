using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastSongTracker : MonoBehaviour
{
    public static LastSongTracker S;

    [HideInInspector] public int lastSong = -1;
    [HideInInspector] public int secondToLastSong = -1;

    private void Awake()
    {
        if (S != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            S = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private List<Song> songs = new List<Song>();
    private AudioSource audioSource;

    [Header("UI")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject songselectionPanel;

    // Game state
    private enum GameState { title, songSelection, playingSong, playingFreeplay };
    private GameState curState = GameState.title;

    private bool receivedInputThisFrame = false; 
    private float idleTimer = 0.0f;
    private float timeToIdle = 60.0f;

    private void Start()
    {
        InputSystem.onAnyButtonPress.CallOnce(ctrl => { receivedInputThisFrame = true; idleTimer = 0.0f; } ); // Sets a bool whenever it gets any inputs from input system (like midi input)
    }

    private void Update()
    {
        // Close game on escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        // Reset scene on r
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
        }

        // Handle idling game
        if (Input.anyKeyDown)
        {
            receivedInputThisFrame = true;
            idleTimer = 0.0f;
        }
        if (curState != GameState.title && !receivedInputThisFrame)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer > timeToIdle)
            {
                ResetScene(); // TODO: should probably reset in a less jarring way in this case
            }
        }
        
        // Handle title screen logic
        if (curState == GameState.title)
        {
            if (receivedInputThisFrame)
            {
                // Go to song selection
                titlePanel.SetActive(false);
                songselectionPanel.SetActive(true);
                curState = GameState.songSelection;
            }
        }
        else if (curState == GameState.songSelection)
        {
            // just waiting for user to hit a button
        }
        else if (curState == GameState.playingSong)
        {
            // Reset when song is over
            if (!audioSource.isPlaying)
            {
                ResetScene();
            }
        }
        else if (curState == GameState.playingFreeplay)
        {
            // will reset after being idle for 60 seconds (or if someone presses r)
        }


        // reset input flag
        receivedInputThisFrame = false; 
    }

    public void btn_PlaySong(int songListIndex)
    {
        curState = GameState.playingSong;
        
        if (songListIndex < songs.Count)
        {
            audioSource.clip = songs[songListIndex].clip;
            audioSource.Play();
        }
    }

    public void btn_PlayFreeplay()
    {
        curState = GameState.playingFreeplay;
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

[System.Serializable]
public struct Song
{
    public string title;
    public string artist;
    public AudioClip clip;
    // TODO: Have fields here to set a particular scene and preset to go with each song
}

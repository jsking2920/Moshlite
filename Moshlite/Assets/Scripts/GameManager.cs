using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private List<Song> songs = new List<Song>();
    [SerializeField] private AudioSource audioSource;
    private Song curSong = null;

    [Header("UI")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject songSelectionPanel;
    [SerializeField] private Image fadeToBlackImage;
    
    [SerializeField] private GameObject songTitlePanel;
    [SerializeField] private TextMeshProUGUI songTitleText;
    [SerializeField] private TextMeshProUGUI songArtistText;
    [SerializeField] private TextMeshProUGUI timeText;

    // Game state
    private enum GameState { title, songSelection, playingSong, playingFreeplay, fadingToReset };
    private GameState curState = GameState.title;

    private bool receivedInputThisFrame = false; 
    private float idleTimer = 0.0f;
    private float timeToIdle = 60.0f;

    [SerializeField] private CameraManager camManager;
    private float camTimer = 0.0f;
    private float camSwitchDelay = 10.0f;

    private void Start()
    {
        InputSystem.onAnyButtonPress.CallOnce(ctrl => { receivedInputThisFrame = true; idleTimer = 0.0f; } ); // Sets a bool whenever it gets any inputs from input system (like midi input)

        songSelectionPanel.SetActive(false);
        titlePanel.SetActive(true);
        songTitlePanel.SetActive(false);
        songTitleText.text = "";
        songArtistText.text = "";
        timeText.text = "";
        curState = GameState.title;
        idleTimer = 0.0f;
        receivedInputThisFrame = false;

        fadeToBlackImage.enabled = true;
        fadeToBlackImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        audioSource.Stop();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.clip = null;
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

            if (idleTimer > timeToIdle && curState != GameState.fadingToReset)
            {
                curState = GameState.fadingToReset;
                StartCoroutine(FadeOnIdleReset());
            }
        }
        
        // Handle title screen logic
        if (curState == GameState.title)
        {
            // Go to song selection on enter, space, or mouse button
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) //(receivedInputThisFrame)
            {
                // Go to song selection
                titlePanel.SetActive(false);
                songSelectionPanel.SetActive(true);
                camTimer = 0.0f;
                curState = GameState.songSelection;
            }
            else
            {
                camTimer += Time.deltaTime;
                if (camTimer > camSwitchDelay)
                {
                    camTimer = 0.0f;
                    camManager.SwapToNext();
                }
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
                curState = GameState.fadingToReset;
                StartCoroutine(FadeToCredits(curSong));
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
            songSelectionPanel.SetActive(false);

            curSong = songs[songListIndex];
            audioSource.clip = curSong.clip;
            audioSource.Play();

            StartCoroutine(FadeToPlayMode(curSong));
        }
    }

    public void btn_PlayFreeplay()
    {
        curState = GameState.playingFreeplay;
        songSelectionPanel.SetActive(false);
        curSong = null;

        StartCoroutine(FadeToPlayMode());
    }

    public IEnumerator FadeToPlayMode(Song song = null)
    {
        float cur_alpha = 0.0f;

        // Fade to black
        while (cur_alpha < 0.99f)
        {
            cur_alpha = Mathf.Lerp(cur_alpha, 1.0f, Time.deltaTime * 2.0f);
            fadeToBlackImage.color = new Color(fadeToBlackImage.color.r, fadeToBlackImage.color.g, fadeToBlackImage.color.b, cur_alpha);
            yield return null;
        }
        fadeToBlackImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);

        // Title card
        if (song == null)
        {
            songTitleText.text = "";
            songArtistText.text = "";
        }
        else
        {
            songTitleText.text = '"' + song.title.ToUpper() + '"';
            songArtistText.text = song.artist;
        }
        timeText.text = System.DateTime.Now.ToString() + ", FINAL GAMES";
        songTitlePanel.SetActive(true);
        yield return new WaitForSeconds(4.0f);

        fadeToBlackImage.color = new Color(0, 0, 0, 0);
        songTitlePanel.SetActive(false);
    }

    public IEnumerator FadeToCredits(Song song = null)
    {
        float cur_alpha = 0.0f;

        // Fade to black
        while (cur_alpha < 0.99f)
        {
            cur_alpha = Mathf.Lerp(cur_alpha, 1.0f, Time.deltaTime * 2.0f);
            fadeToBlackImage.color = new Color(fadeToBlackImage.color.r, fadeToBlackImage.color.g, fadeToBlackImage.color.b, cur_alpha);
            yield return null;
        }
        fadeToBlackImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);

        // Credits
        if (song == null)
        {
            songTitleText.text = "";
            songArtistText.text = "";
        }
        else
        {
            songTitleText.text = '"' + song.title.ToUpper() + '"';
            songArtistText.text = song.artist;
        }
        timeText.text = System.DateTime.Now.ToString() + ", FINAL GAMES";
        songTitlePanel.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        ResetScene();
    }

    public IEnumerator FadeOnIdleReset()
    {
        float cur_alpha = 0.0f;

        // Fade to black
        while (cur_alpha < 0.99f)
        {
            cur_alpha = Mathf.Lerp(cur_alpha, 1.0f, Time.deltaTime * 2.0f);
            fadeToBlackImage.color = new Color(fadeToBlackImage.color.r, fadeToBlackImage.color.g, fadeToBlackImage.color.b, cur_alpha);
            yield return null;
        }
        fadeToBlackImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);

        ResetScene();
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

[System.Serializable]
public class Song
{
    public string title;
    public string artist;
    public AudioClip clip;
    // TODO: Have fields here to set a particular scene and preset to go with each song
}

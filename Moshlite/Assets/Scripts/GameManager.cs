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
    [SerializeField] private List<Song> songs = new List<Song>(); // Will be 11 songs
    [SerializeField] private AudioSource audioSource;
    private Song curSong = null;

    [Header("UI")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private Image fadeToBlackImage;
    
    [SerializeField] private GameObject songTitlePanel;
    [SerializeField] private TextMeshProUGUI songTitleText;
    [SerializeField] private TextMeshProUGUI songArtistText;
    [SerializeField] private TextMeshProUGUI timeText;

    // Game state
    private enum GameState { title, playingSong, playingFreeplay, fadingToReset };
    private GameState curState = GameState.title;

    private bool receivedInputThisFrame = false; 
    private float idleTimer = 0.0f;
    private float timeToIdle = 60.0f;

    [SerializeField] private CameraManager camManager;
    private float camTimer = 0.0f;
    private float camSwitchDelay = 10.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // hide cursor

        InputSystem.onAnyButtonPress.CallOnce(OnReceivedInput); // Sets a bool whenever it gets any inputs from input system (like midi input)
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

    private void LateUpdate()
    {
        // Handle keyboard inputs 

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
        else if (Input.anyKeyDown)
        {
            receivedInputThisFrame = true;
            idleTimer = 0.0f;
        }

        // Handle idling game
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
                int r = Random.Range(0, songs.Count - 1); // 11th song is not in random pool, it's secret
                while (r == LastSongTracker.S.lastSong || r == LastSongTracker.S.secondToLastSong)
                {
                    r = Random.Range(0, songs.Count - 1);
                }
                PlaySong(r);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1)) PlaySong(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) PlaySong(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) PlaySong(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) PlaySong(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) PlaySong(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) PlaySong(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) PlaySong(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) PlaySong(7);
            else if (Input.GetKeyDown(KeyCode.Alpha9)) PlaySong(8);
            else if (Input.GetKeyDown(KeyCode.Alpha0)) PlaySong(9);
            else if (Input.GetKeyDown(KeyCode.Minus)) PlaySong(10);
            else if (Input.GetKeyDown(KeyCode.Equals)) PlayFreeplay();
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

    public void OnReceivedInput(InputControl ctrl)
    {
        receivedInputThisFrame = true; 
        idleTimer = 0.0f;
    }

    public void PlaySong(int i)
    {
        titlePanel.SetActive(false);
        camTimer = 0.0f;

        curState = GameState.playingSong;
        curSong = songs[i];
        audioSource.clip = songs[i].clip;
        audioSource.Play();
        StartCoroutine(FadeToPlayMode(songs[i]));

        LastSongTracker.S.secondToLastSong = LastSongTracker.S.lastSong;
        LastSongTracker.S.lastSong = i;
    }

    public void PlayFreeplay()
    {
        titlePanel.SetActive(false);
        camTimer = 0.0f;

        curState = GameState.playingFreeplay;
        curSong = null;
        StartCoroutine(FadeToPlayMode());
    }

    // TODO: sync this up with intros of the songs
    public IEnumerator FadeToPlayMode(Song song = null)
    {
        float cur_alpha = 0.0f;

        // Fade to black
        while (cur_alpha < 0.99f)
        {
            cur_alpha += Time.deltaTime * 0.8f;
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
        timeText.text = System.DateTime.Now.ToString().Replace("/", ".") + ", GAMES PREMIERE";
        songTitlePanel.SetActive(true);
        yield return new WaitForSeconds(4.25f);

        fadeToBlackImage.color = new Color(0, 0, 0, 0);
        songTitlePanel.SetActive(false);
    }

    public IEnumerator FadeToCredits(Song song = null)
    {
        float cur_alpha = 0.0f;

        // Fade to black
        while (cur_alpha < 0.99f)
        {
            cur_alpha += Time.deltaTime * 0.8f;
            fadeToBlackImage.color = new Color(fadeToBlackImage.color.r, fadeToBlackImage.color.g, fadeToBlackImage.color.b, cur_alpha);
            yield return null;
        }
        fadeToBlackImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);

        timeText.text = System.DateTime.Now.ToString().Replace("/", ".") + ", GAMES PREMIERE";
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
            cur_alpha += Time.deltaTime;
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

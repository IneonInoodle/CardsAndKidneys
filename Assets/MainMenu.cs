using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class MainMenu : MonoBehaviour {

    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;
    public List<GameObject> PauseMenuUI2;
    private SceneLoader sceneloader;
	public void PlayGame()
    { 
        Time.timeScale = 1f;
        GameIsPaused = false;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameObject playbutton = GameObject.Find("MainMenu");
        sceneloader = playbutton.transform.GetComponent<SceneLoader>();
        Debug.Log(sceneloader);
        if (sceneloader != null)
        sceneloader.startloading = true;
        for (int i = 0; i < PauseMenuUI2.Count; i++)
        {
            //PauseMenuUI2[i].SetActive(false);
        }

    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OptionGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }
    
	public void Break()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ResumePause()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }


    public void Resume()
    {       
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Restart()
    {

        

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.CurrentPlayerTurn = GameManager.Instance.players[0];
        GameManager.Instance.players[0].IsTurnComplete = true;

        GameManager.Instance.UpdateTurn();
        Time.timeScale = 1f;
        GameIsPaused = false;
        PauseMenuUI.SetActive(false);

        
    }

    public void Pause()
    {       
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    void Awake()
    {
        //Debug.Log("Awake");
        
    }
    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
    }

    // called third
    void Start()
    {
        //Debug.Log("Start");
        AudioManager.instance.rePlay("GameStart");
    }

    // called when the game is terminated
    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
   
}

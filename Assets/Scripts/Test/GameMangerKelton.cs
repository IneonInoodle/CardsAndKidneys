using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameMangerKelton : MonoBehaviour {
    //switch players turn 

    public PlayerManager[] players;

    public PlayerManager CurrentPlayerTurn; 

    BoardManager  board;

    bool hasLevelStarted = false;
    public bool HasLevelStarted { get { return hasLevelStarted; } set { hasLevelStarted = value; } }

    bool isGamePlaying = false;
    public bool IsGamePlaying { get { return isGamePlaying; } set { isGamePlaying = value; } }

    bool isGameOver = false;
    public bool IsGameOver { get { return isGameOver; } set { isGameOver = value; } }

    bool hasLevelFinished = false;
    public bool HasLevelFinished { get { return hasLevelFinished; } set { hasLevelFinished = value; } }
    // Use this for initialization

    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    

    private void Awake()
    {   

        players = Object.FindObjectsOfType<PlayerManager>(); //find player
        board = Object.FindObjectOfType<BoardManager>().GetComponent<BoardManager>(); //find board


        //create both players
        //assign one to top and one to bottom 
        //set one on bottom as going first 

        //deal in starting cards 

    }
    // Update is called once per frame
    void Update () {
 
    }

    void Start()
    {
        if (players == null && board == null)
        {
            Debug.LogWarning("no player or board");
        } else {
            Debug.LogWarning("run game loop");
            StartCoroutine("RunGameLoop");
            //can be called from anything, command to start Level (events
            PlayLevel();

            // for testing
            PlayPlayerTurn(players[1]);


        }
    }

    IEnumerator RunGameLoop()
    {
        Debug.Log("startlevel");
        yield return StartCoroutine("StartLevelRoutine");
        Debug.LogWarning("run game loop");
        yield return StartCoroutine("PlayLevelRoutine");
        Debug.LogWarning("end game loop");
        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine()
    {
        Debug.Log("StartLevelRoutine");

        //for testing 

        yield return StartCoroutine(board.UpdateBoard());
        players[1].playerInput.InputEnabled = true;
        players[0].playerInput.InputEnabled = false;
        /* while (!hasLevelStarted)
         {   
             //show start screen
             // user will press a button here to start
             // HasLevelStarted = true
             yield return null;
         }*/
        yield return null;
        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }

    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("PlayLevelRoutine");
        IsGamePlaying = true;

        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!isGameOver)
        {   

            //check for game over condition

            //win
            //reach end of the level
            // lose
            //player dies
            // IsGameOver = true; 

            yield return null;
        }
    }
    
    IEnumerator EndLevelRoutine()
    {
        Debug.Log("EndLevelRoutine");

        foreach (PlayerManager p in players)
        {
            p.playerInput.InputEnabled = false;
        }

        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }

        while (!hasLevelFinished)
        {   
            //user presses button to continue
            // HasLevelFinished = true;
            yield return null;
        }
        //show end screen 
        
        RestartLevel();
    }

    void RestartLevel()
    {   
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayLevel()
    {
        hasLevelStarted = true;
    }

    public IEnumerator UpdateTurn()
    {
        Debug.Log("update turn");
        //changes player turn 
        if (CurrentPlayerTurn == players[0])
        {
            if (players[0].IsTurnComplete)
            {
                players[0].playerInput.InputEnabled = false;
                yield return StartCoroutine(board.UpdateBoard());    
                players[1].playerInput.InputEnabled = true;
                PlayPlayerTurn(players[1]);
            }
        }
        else if (CurrentPlayerTurn == players[1])
        {
            if (players[1].IsTurnComplete)
            {

                players[1].playerInput.InputEnabled = false;
                yield return StartCoroutine(board.UpdateBoard());
                players[0].playerInput.InputEnabled = true;
                PlayPlayerTurn(players[0]);
            }
        }
    }

    public void initPlayerBoard()
    {
        
        // TODO change setAdjencyto Coroutine
        


    }
        

    public void PlayPlayerTurn(PlayerManager player)
    {
        if (player.myLocation == location.board)
        {
            player.turnsOnBoard++; 
        }
        player.ActionPoints = player.turnsOnBoard;
        CurrentPlayerTurn = player;
        player.IsTurnComplete = false;

        //deal out cards
        //increase turnsonboard
        //increase turnswithout kidney if nessacary

        //player now has control, waiting for them to call Unity action (from event) update turn to switch players

    }
}

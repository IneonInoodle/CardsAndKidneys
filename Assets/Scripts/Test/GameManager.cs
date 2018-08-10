using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //switch players turn 

    public PlayerManager[] players;
    public PlayerManager CurrentPlayerTurn;

    public Sprite BottomPortraitMask;
    public Sprite TopPortraitMask;

    public SoundManager soundManager;
    public MessageManager messageManager;
    public SelectionManager selectionManager;

    public GameObject DamagePrefab;

    public GameObject SpellCardPrefab;

    public TableCollider TableCollider;

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

    enum State
    {
        Initializing,
        MainMenu,
        Game,
        PauseMenu,
        Credits
    }
    State state=State.Initializing;



    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }


    private void Awake()
    {
        Debug.Assert(selectionManager != null);
        Debug.Assert(messageManager != null);
        Debug.Assert(soundManager != null);

        soundManager = Object.FindObjectOfType<SoundManager>().GetComponent<SoundManager>();
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

        foreach (PlayerManager p in players)
        {
            p.button.interactable = false;
        }

            //for testing 


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
        players[0].playerInput.InputEnabled = false;
        players[1].playerInput.InputEnabled = false;
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
        StartCoroutine(UpdateTurn());
    }
    public PlayerManager getOtherPlayer(PlayerManager p)
    {
        if (players[0] == p) return players[1];
        else return players[0];
    }
    public IEnumerator UpdateTurn() //switches player turns
    {
        SoundManager.PlaySound("pressEndTurnButtonSound");

        Debug.Log("what");
        if (CurrentPlayerTurn == null)
        {
            //for first round here we do a trick, set the condition as if enemy just finished playing
            CurrentPlayerTurn = players[0];
            players[0].IsTurnComplete = true;
        }
        Debug.Log("update turn");
        //changes player turn 
        if (CurrentPlayerTurn == players[0])
        {
            if (players[0].IsTurnComplete)
            {
                if (players[0].TurnsWithoutKidney >= 2)
                {
                    messageManager.ShowMessage("You Loose Boy", 1f);
                    isGameOver = true;
                    Debug.Log("YouLoose");
                }
                else if (players[0].patientKidneys.Count == 2)
                {
                    messageManager.ShowMessage("You Win", 1f);
                    isGameOver = true;
                }
                else
                {
                    Debug.Log("playerturn0complete");
                    players[0].playerInput.InputEnabled = false;
                    players[0].button.interactable = false;
                    players[0].PortaitGlowObject.SetActive(false);

                    if (players[0].myCardManager != null)
                        players[0].myCardManager.CardFaceGlowObject.SetActive(false);
                    players[0].EndTurnGlowObject.SetActive(false);

                    messageManager.ShowMessage("Your Turn", 1f);
                    yield return StartCoroutine(board.UpdateBoard());

                    CurrentPlayerTurn = players[1];
                    PlayPlayerTurn(players[1]);
                }
                
            }
        }
        else if (CurrentPlayerTurn == players[1])
        {
            if (players[1].IsTurnComplete)
            {

                if (players[1].TurnsWithoutKidney >= 2)
                {
                    messageManager.ShowMessage("You Loose Boy", 1f);
                    isGameOver = true;
                    Debug.Log("YouLoose");
                } else if (players[1].patientKidneys.Count == 2)
                {
                    messageManager.ShowMessage("You Win", 1f);
                    isGameOver = true;
                } else
                {
                    Debug.Log("playerturn1complete");
                    players[1].playerInput.InputEnabled = false;
                    players[1].button.interactable = false;
                    players[1].PortaitGlowObject.SetActive(false);

                    if (players[1].myCardManager != null)
                        players[1].myCardManager.CardFaceGlowObject.SetActive(false);
                    players[1].EndTurnGlowObject.SetActive(false);

                    messageManager.ShowMessage("Enemy Turn", 1f);
                    yield return StartCoroutine(board.UpdateBoard());
                    players[0].playerInput.InputEnabled = true;

                    CurrentPlayerTurn = players[0];
                    PlayPlayerTurn(players[0]);
                }

                
            }
        }
    }

    public void initPlayerBoard()
    {
        
        // TODO change setAdjencyto Coroutine
        


    }

    public void DisableInputs() {
        foreach (PlayerManager p in players)
        {
            p.playerInput.InputEnabled = false;
            p.button.interactable = false;
            p.handvisual.gameObject.SetActive(false);
        }

    }

    public void EnableInputs()
    {
        CurrentPlayerTurn.playerInput.InputEnabled = true;
        CurrentPlayerTurn.button.interactable = true;

        foreach (PlayerManager p in players) //for testing
        {
            p.handvisual.gameObject.SetActive(true);
        }
    }


    public void PlayPlayerTurn(PlayerManager player)
    {   
        if (player.patientKidneys.Count  == 0)
        {
            player.TurnsWithoutKidney++;
            Debug.Log("TurnWithoutKidney " + player.TurnsWithoutKidney); 
        } 

        player.playerInput.InputEnabled = true;
        
        if (player.myLocation == location.board)
        {
            player.turnsOnBoard++;
            player.myCardManager.CardFaceGlowObject.SetActive(true);
        } else
        {
            player.PortaitGlowObject.SetActive(true);
        }

        if (player.Hp == 0)
        {
            player.Hp = 10;
        }


    
        player.ActionPoints = player.turnsOnBoard;
        CurrentPlayerTurn = player;
        player.IsTurnComplete = false;


        StartCoroutine(player.DealPlayerCards(player.turnsOnBoard));
        //deal out cards
        //increase turnsonboard
        //increase turnswithout kidney if nessacary

        //player now has control, waiting for them to call Unity action (from event) update turn to switch players

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour {
    //switch players turn 

    public PlayerManager[] players;
    public PlayerManager CurrentPlayerTurn;

    public Sprite BottomPortraitMask;
    public Sprite TopPortraitMask;

    public Camera camera;
    public MessageManager messageManager;
    public SelectionManager selectionManager;

    public GameObject mainMenu;
    public GameObject DamagePrefab;
    public GameObject SpellCardPrefab;

    public TableCollider TableCollider;

    public Material TopSpellMat;
    public Material BottomSpellMat;
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

    public void flipBoard()
    {

    }

    private void Awake()
    {
        Debug.Assert(selectionManager != null);
        Debug.Assert(messageManager != null);
        players = Object.FindObjectsOfType<PlayerManager>(); //find player
        board = BoardManager.Instance; //find board


        //create both players
        //assign one to top and one to bottom 
        //set one on bottom as going first 

        //deal in starting cards 

    }
    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.I))
        {
            RotateEverything();
            
            //SwapCard(new Point(1, 1), new Point(0, 0));
            //Generate();
            //StartCoroutine(GenerateTest());
        }


        
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

        yield return StartCoroutine("StartLevelRoutine");

        yield return StartCoroutine("PlayLevelRoutine");

        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine()
    {


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
        
        //AudioManager.instance.Play("pressEndTurnButtonSound");
        if (CurrentPlayerTurn == null)
        {
            //for first round here we do a trick, set the condition as if enemy just finished playing
            CurrentPlayerTurn = players[0];
            players[0].IsTurnComplete = true;
        }

        //changes player turn 
        if (CurrentPlayerTurn == players[0])
        {
           
            if (players[0].IsTurnComplete)
            {
                if (players[0].patientKidneys.Count == 2)
                {
                    messageManager.ShowMessage("merlinWin", 1f);
                    isGameOver = true;
                    yield return new WaitForSeconds(3f);
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {

                    players[0].playerInput.InputEnabled = false;
                    players[0].button.interactable = false;
                    players[0].PortaitGlowObject.SetActive(false);

                    if (players[0].myCardManager != null)
                        players[0].myCardManager.CardFaceGlowObject.SetActive(false);
                    players[0].EndTurnGlowObject.SetActive(false);

                    
                    messageManager.ShowMessage("patric", 1f);
                    PlayPlayerTurn(players[1]);
                }
                
            }
        }
        else if (CurrentPlayerTurn == players[1])
        {
            if (players[1].IsTurnComplete)
            {
                 if (players[1].patientKidneys.Count == 2)
                {
                    messageManager.ShowMessage("patricWin", 1f);
                    isGameOver = true;
                    yield return new WaitForSeconds(3f);
                    SceneManager.LoadScene("MainMenu");
                } else
                {
                    players[1].playerInput.InputEnabled = false;
                    players[1].button.interactable = false;
                    players[1].PortaitGlowObject.SetActive(false);

                    if (players[1].myCardManager != null)
                        players[1].myCardManager.CardFaceGlowObject.SetActive(false);
                    players[1].EndTurnGlowObject.SetActive(false);

                    
                    messageManager.ShowMessage("merlin", 1f);                   
                    PlayPlayerTurn(players[0]);
                }  
            }
        }
        yield return null;
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

            //foreach car in hand visual 

            //PreviewsAllowed= false 

        }

        board.Top.GetComponent<EndzoneManager>().isSelectable = false;
        board.Bottom.GetComponent<EndzoneManager>().isSelectable = false;

    }

    public void EnableInputs()
    {
        CurrentPlayerTurn.playerInput.InputEnabled = true;
        CurrentPlayerTurn.button.interactable = true;

        foreach (PlayerManager p in players) //for testing
        {
            p.handvisual.gameObject.SetActive(true);
        }

        board.Top.GetComponent<EndzoneManager>().isSelectable = true;
        board.Bottom.GetComponent<EndzoneManager>().isSelectable = true;
    }

    public void RotateEverything()
    {
        // camera.transform.DOLocalRotate(new Vector3(90f, 0f, camera.transform.localRotation.eulerAngles.y + 180), 0.25f, RotateMode.FastBeyond360);

        camera.transform.DOLocalRotateQuaternion(camera.transform.localRotation * Quaternion.Euler(0, 0, 180), 0.25f);

        selectionManager.transform.DOLocalRotateQuaternion(selectionManager.transform.localRotation * Quaternion.Euler(0, 180, 0), 0.25f);
        mainMenu.transform.DOLocalRotateQuaternion(selectionManager.transform.localRotation * Quaternion.Euler(0, 180, 0), 0.25f);
        messageManager.transform.DOLocalRotateQuaternion(messageManager.transform.localRotation * Quaternion.Euler(0, 0, 180), 0.25f);

        StartCoroutine(board.RotateFieldCards());
        board.RotatePlayerPortaitZones();

        foreach (PlayerManager p in players)
        {
            p.RotateElements();
        }

       

 

        
        //camera.transform.DOLocalRotate(new Vector3(90f, 0f, 180), 0.25f, RotateMode.FastBeyond360);

    }

    public void PlayPlayerTurn(PlayerManager player)
    {
        GameManager.Instance.getOtherPlayer(player).handvisual.MakeCardsGrey(true);
        player.handvisual.MakeCardsGrey(false);

        GameManager.Instance.getOtherPlayer(player).setAnimationState(false);
        player.setAnimationState(true);

        if (player.patientKidneys.Count  == 0)
        {
            //player.TurnsWithoutKidney++;
        } 

        if (player.myLocation == location.board)
        {
            //player.MaxAp++;
            player.myCardManager.CardFaceGlowObject.SetActive(true);
        } else
        {
            player.PortaitGlowObject.SetActive(true);
        }

        if (player.Hp == 0)
        {
            player.Hp = 10;
        }

        //player.applyStatusEffects();
    
        player.ActionPoints = player.AvailableAp;
        CurrentPlayerTurn = player;
        player.IsTurnComplete = false;

        StartCoroutine(board.UpdateBoard());
        StartCoroutine(player.DealPlayerCards(1));


        player.playerInput.InputEnabled = true;
        //deal out cards
        //increase turnsonboard
        //increase turnswithout kidney if nessacary

        //player now has control, waiting for them to call Unity action (from event) update turn to switch players

    }

  
}

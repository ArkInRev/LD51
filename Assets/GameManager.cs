using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // TIME Control
    private float _triggerEvery = 10f;
    private float _triggerNext = 0f;

    // GAME Tick
    private float _tickElapsed = 0f;
    private float _ticksPerSecond = 2f;

    // POWER State
    private bool _powerState = true;

    // Wandering
    public Transform[] wanderingSpots;

    // Items Returned
    public bool[] itemsReturned;
    public Transform[] exhaustLocations;
    public Transform[] sideLocations;
    public Transform[] backLocations;
    public Transform[] enemySpawnLocations;



    // Game over screens
    public CanvasGroup TitleScreen;
    public CanvasGroup GameOverScreen;
    public TMP_Text GameOverMessage;
    public TMP_Text GameOverDetails;
    public TMP_Text GameOverButtonText;
    



    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {

        Time.timeScale = 0f;
        powerChange();
        //GameStart();  // Reset game settings to begginning values
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        TimeHandler();
        TickHandler();


    }

    private void TimeHandler()
    {
        _triggerNext += Time.fixedDeltaTime;
        if (_triggerNext >= _triggerEvery)
        {
            _triggerNext = 0;
            _powerState = !_powerState;
            powerChange();
        }
    }

    private void TickHandler()
    {
        _tickElapsed += Time.fixedDeltaTime;
        if (_tickElapsed >= (1 / _ticksPerSecond))
        {
            //Debug.Log("Tick");

            Tick();
            _tickElapsed = 0;
        }
    }

    public Transform getRandomWanderingSpot()
    {
        return wanderingSpots[UnityEngine.Random.Range(0, wanderingSpots.Length)];
    }




    //Every time the power toggles
    public event Action onTick;
    public void Tick()
    {

        if (onTick != null)
        {
            onTick();
        }

    }

    //Every time the power toggles
    public event Action<bool> onPowerChange;
    public void powerChange()
    {
 //       Debug.Log("Power: " + _powerState.ToString());
        if (onPowerChange != null)
        {
            onPowerChange(_powerState);
        }

    }

    public void GameStart()
    {
        itemsReturned = new bool[] { false, false, false };
        _tickElapsed = 0;
        Time.timeScale = 1;
        TitleScreen.alpha = 0;
        TitleScreen.interactable = false;
        TitleScreen.blocksRaycasts = false;
    }
    public void gameOver(bool win)
    {
        Time.timeScale = 0;
        if (win)
        {
            GameOverButtonText.SetText("Play Again");
            GameOverMessage.SetText("You Win!");
            GameOverDetails.SetText("You fixed the generator and the security patrol has captured the alien life form!");

        }
        else
        {
            int piecesFound = 0;
            foreach(bool itemFixed in itemsReturned)
            {
                if (itemFixed)
                {
                    piecesFound += 1;
                }
            }


            GameOverButtonText.SetText("Try Again");
            GameOverMessage.SetText("You Have Lost");
            GameOverDetails.SetText("You have been caught by the alien life form! You fixed " + piecesFound.ToString() + " out of 3 parts of the generator.");
        }
        GameOverScreen.alpha = 1;
        GameOverScreen.interactable = true;
        GameOverScreen.blocksRaycasts = true;
    }

    public void ReloadLevel()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }
}

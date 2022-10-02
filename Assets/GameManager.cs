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

    public Transform[] wanderingSpots;

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
}

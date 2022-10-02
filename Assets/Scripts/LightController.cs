using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LightController : MonoBehaviour
{

    public bool emergencyLight = false;
    public Light[] lightArray;
    private bool thisLightState = false;
    

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.onPowerChange += OnPowerChanged;
    }


    public void OnPowerChanged(bool powerState)
    {
        thisLightState = powerState;
        if(emergencyLight) { thisLightState = !thisLightState; }
        foreach(Light thisLight in lightArray)
        {
            thisLight.enabled = thisLightState;
        }
    }


    public void OnDisable()
    {
        GameManager.Instance.onPowerChange -= OnPowerChanged;
    }
}

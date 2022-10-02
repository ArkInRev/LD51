using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingController : MonoBehaviour
{

    public List<GameObject> heardSounds;
    public SphereCollider sc;

    public Vector3 lastHeardPosition;
    public Vector3 lastLoudestSound;
    public float lastLoudestVolume;



    private void Start()
    {
        GameManager.Instance.onTick += gameTick;
    }

    private void FixedUpdate()
    {
        //lastLoudestSound =  findLoudestSound();

    }

    public void gameTick()
    {
        reduceHeardNoise();
        lastLoudestSound = findLoudestSound();
        if(lastLoudestSound == Vector3.zero)
        {
            lastHeardPosition = Vector3.zero;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Entered trigger: "+other.gameObject.ToString());
        heardSounds.Add(other.gameObject);
        

    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Exited trigger: " + other.gameObject.ToString());
        //heardSounds.Remove(other.gameObject.transform);
        //lastHeardPosition = other.transform;
        heardSounds.Remove(other.gameObject);

    }

    private void OnTriggerStay(Collider other)
    {

    }


    private Vector3 findLoudestSound()
    {
        float loudestSound = 0f;
        Vector3 loudestObject = Vector3.zero;
   
        foreach(GameObject sound in heardSounds)
        {
            if(sound != null)
            {
                // check if the sound is the loudest
                if(sound.transform.localScale.x > loudestSound)
                {
                    if(sound.transform.position != Vector3.zero)
                    {
                        lastHeardPosition = sound.transform.position;
                    }
                    loudestObject = sound.transform.position;
                }            
            }
        }


 //       if(other.transform.localScale.x > loudestSound)
 //       {
 //           loudestObject = other.transform.position;
 //       }
        return loudestObject;
    }

    private void reduceHeardNoise()
    {
        lastLoudestVolume -= 1;
        if(lastLoudestVolume <= 0.5f)
        {
            lastLoudestVolume = 0;
            lastHeardPosition = lastLoudestSound;
        }
    }

    private void removeNullFromHeard()
    {
        for(int i = heardSounds.Count -1; i > -1; i--)
        {
            if (heardSounds[i] == null)
            {
                heardSounds.RemoveAt(i);
            }
        }
    }

    public Vector3 getHearingLocation()
    {  
        return lastLoudestSound;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onTick -= gameTick;
    }
}

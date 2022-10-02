using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{

    public int pickupIndex;
    public MeshRenderer[] mr;
    public AudioSource thisAS;

    private void Start()
    {
        // Randomize Location
        getRandomStartingPosition();
        mr = this.GetComponentsInChildren<MeshRenderer>();
        thisAS = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (!pc.isCarrying)
            {
                pc.pickupItem(pickupIndex);

                foreach (MeshRenderer thisMR in mr)
                {
                    thisMR.enabled = false;
                }
                // play pickup sound
                thisAS.Play();
                Destroy(this.gameObject, 3);
            }
        }
    }

    private void getRandomStartingPosition()
    {
        Transform[] thisLocationList;
        switch (pickupIndex)
        {
            case 0://Exhaust
                thisLocationList = GameManager.Instance.exhaustLocations;
                break;
            case 1://Side
                thisLocationList = GameManager.Instance.sideLocations;
                break;
            case 2://Back
                thisLocationList = GameManager.Instance.backLocations;
                break;
            default:
                thisLocationList = GameManager.Instance.exhaustLocations;
                break;
        }
        Transform thisDestination =  thisLocationList[UnityEngine.Random.Range(0, thisLocationList.Length)];
        this.transform.position = thisDestination.position;
    }


}

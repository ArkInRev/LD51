using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffController : MonoBehaviour
{

    public int dropoffIndex;
    public MeshRenderer dropoffMR;
    public MeshRenderer returnedMR;
    public SphereCollider dropoffSC;

    public AudioSource thisAS;

    private void Start()
    {
        thisAS = GetComponent<AudioSource>();


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {          
            PlayerController pc = other.GetComponent<PlayerController>();
            if((pc.isCarrying)&&(pc.itemCarried==dropoffIndex))
            {
                // PC is carrying an item matching this location
                pc.dropoffItem(dropoffIndex);
                //Debug.Log("PC is carrying && dropping off item: " +pc.itemCarried.ToString() + " at location: " + dropoffIndex.ToString());
                returnedMR.enabled = true;
                dropoffMR.enabled = false;
                dropoffSC.enabled = false;
                //Destroy(this.gameObject, 3);
                // play dropoff sound
                thisAS.Play();
            }



        }
    }


}

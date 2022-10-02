using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool tryCrouch;
    public bool isWalking = false;
    public bool isCarrying = false;
    public int itemCarried = 0;

    public float speedModifier;
    [SerializeField]
    private float _normalSpeedMod = 1f;
    [SerializeField]
    private float _crouchSpeedMod = 0.6f;
    [SerializeField]
    private float _carrySpeedMultiplier = 0.6f;
    [SerializeField]
    private float _normalSoundScale = 25f;
    [SerializeField]
    private float _crouchSoundScale = 10f;
    [SerializeField]
    private float _carrySoundMultiplier = 3f;

    public float currentCarrySpeedMultiplier = 1f;

    public GameObject soundSpherePrefab;
    public GameObject soundGroupParent;


    public List<AudioClip> heavyAudioClips;
    public List<AudioClip> crouchAudioClips;
    public AudioClip currentClip;
    public AudioSource audioSource;

    public float pitchMin = 0.8f;
    public float pitchMax = 1.2f;
    public float volumeMin = 0.25f;
    public float volumeMax = 0.75f;

    public MeshRenderer carriedExhaustMR;
//    public SphereCollider carriedExhaustSC;
    public MeshRenderer carriedSideMR;
 //   public SphereCollider carriedSideSC;
    public MeshRenderer carriedBackMR;
//    public SphereCollider carriedBackSC;

    [SerializeField]
    private Transform followPoint;
    [SerializeField]
    private Transform playerModel;

    public float localFollowCrouchOffset = -0.5f;
    public float localModelCrouchOffset = -0.25f;




    void Start()
    {
        GameManager.Instance.onTick += gameTick;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isCarrying)
        {
            currentCarrySpeedMultiplier = _carrySpeedMultiplier;
        } else
        {
            currentCarrySpeedMultiplier = 1;
        }

        if ((Input.GetButton("Fire2")) || (Input.GetButton("Fire1")))
        {
            tryCrouch = true;
            speedModifier = _crouchSpeedMod * currentCarrySpeedMultiplier; 
        } else
        {
            tryCrouch = false;
            speedModifier = _normalSpeedMod * currentCarrySpeedMultiplier;
        }

    }

    private void FixedUpdate()
    {
        if (tryCrouch)
        {
            //Debug.Log("Attempting to Crouch");
            followPoint.localPosition = new Vector3(0f, localFollowCrouchOffset, 0f);
            playerModel.localPosition = new Vector3(0f, localModelCrouchOffset, 0f);


        }
        else
        {
            //Debug.Log("Attempting to Stand");
            followPoint.localPosition = new Vector3(0f, 0f, 0f);
            playerModel.localPosition = new Vector3(0f, 0f, 0f);

        }

        //Debug.Log("PC is walking: "+isWalking.ToString());
    }


    public void gameTick()
    {
        if (isWalking)
        {
            makeNoise();
        }
    }

    public void makeNoise()
    {
        float currentSoundModifier = 1f;
        float currentSoundScale = 1f;

        if(isCarrying) { currentSoundModifier = _carrySoundMultiplier;  }

        if (tryCrouch)
        {
            //Debug.Log("Play a Crouchstep Sound.");
            currentClip = crouchAudioClips[Random.Range(0, crouchAudioClips.Count)];
            audioSource.clip = currentClip;
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.volume = Random.Range(volumeMin, ((volumeMax - volumeMin) / 4) + volumeMin);
            audioSource.PlayOneShot(currentClip);
            //Debug.Log("Leave Crouch Sound.");
            currentSoundScale = _crouchSoundScale * currentSoundModifier;
        }
        else
        {
            //Debug.Log("Play a Footstep Sound.");
            currentClip = heavyAudioClips[Random.Range(0, heavyAudioClips.Count)];
            audioSource.clip = currentClip;
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.volume = Random.Range(((volumeMax-volumeMin)/4)+volumeMin, volumeMax);
            audioSource.PlayOneShot(currentClip);
            //            Debug.Log("Leave Walking Sound.");
            currentSoundScale = _normalSoundScale * currentSoundModifier;

        }
        GameObject go = Instantiate(soundSpherePrefab, transform.position, transform.rotation, soundGroupParent.transform);
        go.transform.localScale = new Vector3(currentSoundScale, currentSoundScale, currentSoundScale);

    }

    public void pickupItem(int itemPickedUp)
    {
        if (!isCarrying)
        {
            isCarrying = true;
            itemCarried = itemPickedUp;
            switch (itemCarried)
            {
                case 0: //Exhaust
                    carriedExhaustMR.enabled = true;
                    //carriedExhaustSC.enabled = true;
                    break;
                case 1: //Side
                    carriedSideMR.enabled = true;
                    //carriedSideSC.enabled = true;
                    break;
                case 2: //Back
                    carriedBackMR.enabled = true;
                    //carriedBackSC.enabled = true;
                    break;

            }
        }
    }

    public void dropoffItem(int itemPickedUp)
    {
        if (isCarrying)
        {
            isCarrying = false;
            itemCarried = itemPickedUp;
            GameManager.Instance.itemsReturned[itemCarried] = true;
            switch (itemCarried)
            {
                case 0: //Exhaust
                    carriedExhaustMR.enabled = false;
                    //carriedExhaustSC.enabled = false;
                    break;
                case 1: //Side
                    carriedSideMR.enabled = false;
                    //carriedSideSC.enabled = false;
                    break;
                case 2: //Back
                    carriedBackMR.enabled = false;
                    //carriedBackSC.enabled = false;
                    break;

            }
            if (checkForWin())
            {
                //Debug.Log("You Win!");
                GameManager.Instance.gameOver(true);
            } else
            {
                //Debug.Log("More Items to Collect");
            }
        }
    }

    private bool checkForWin()
    {
        if(GameManager.Instance.itemsReturned[0]&& GameManager.Instance.itemsReturned[1]&& GameManager.Instance.itemsReturned[2])
        {
            return true;
        } else
        {
            return false;
        }

    }

    public void OnDisable()
    {
        GameManager.Instance.onTick -= gameTick;
    }
}

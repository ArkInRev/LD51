using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool tryCrouch;
    public bool isWalking = false;
    public bool isCarrying = false;

    public float speedModifier;
    [SerializeField]
    private float _normalSpeedMod = 1f;
    [SerializeField]
    private float _crouchSpeedMod = 0.6f;
    [SerializeField]
    private float _normalSoundScale = 25f;
    [SerializeField]
    private float _crouchSoundScale = 10f;
    [SerializeField]
    private float _carrySoundMultiplier = 1.5f;


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
        if ((Input.GetButton("Fire2")) || (Input.GetButton("Fire1")))
        {
            tryCrouch = true;
            speedModifier = _crouchSpeedMod;
        } else
        {
            tryCrouch = false;
            speedModifier = _normalSpeedMod;
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
            Debug.Log("Play a Crouchstep Sound.");
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
            Debug.Log("Play a Footstep Sound.");
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


    public void OnDisable()
    {
        GameManager.Instance.onTick -= gameTick;
    }
}

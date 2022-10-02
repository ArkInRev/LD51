using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private GameManager gm;

    private NavMeshAgent agent;
    enum AIState { Wander, Wandering, Stun, Alert, Searching, Spotting, Chasing, Deciding };
    // Wander - Decide on target
    // Wandering - Heading to a Target
    // Stun - Can't do anything
    // Alert - Transition to Searching
    // Search - Following encountered Audio
    // Chasing - Face player and close distance

    [SerializeField]
    private AIState enemyAIState;

    public float ticksUntilAIChange = 10f;
    public float ticksSinceLastAIChange = 0f;
    public Transform[] wanderLocations;


    public bool canSeePlayer = false;
    public bool canHearPlayer = false;

    public float alienWanderSpeed = 6;
    public float alienSearchSpeed = 8;
    public float alienChaseSpeed = 12;

    public float ticksUntilFootstep = 3f;
    public float ticksSinceLastFootstep = 0f;

    public List<AudioClip> alienFootstepAudioClips;
    public AudioClip currentClip;
    public AudioSource audioSource;

    public AudioClip alienStunScreamClip;
    public AudioClip alienIdleClip;


    public float pitchMin = 0.8f;
    public float pitchMax = 1.0f;
    public float volumeMin = 0.25f;
    public float volumeMax = 0.75f;

    public Animator alienAnimator;

    private HearingController hearing;



    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        alienAnimator = GetComponentInChildren<Animator>();
        hearing = GetComponentInChildren<HearingController>();
        enemyAIState = AIState.Stun;
        GameManager.Instance.onPowerChange += onPowerChanged;
        GameManager.Instance.onTick += onTick;
        AIStateChanged();
    }

    void FixedUpdate()
    {

        bool stateChanged = false;
        switch (enemyAIState)
        {
            case AIState.Wandering:
                // set to the target destination

                break;
            case AIState.Stun:
                agent.destination = this.transform.position;
                break;
            case AIState.Searching:
                if ((hearing.lastLoudestSound != Vector3.zero) || (hearing.lastLoudestSound != null))
                {
                    agent.destination = hearing.lastLoudestSound;
                } else
                {
                    canHearPlayer = false;
                }
                break;

        }

        // Don't do anything if currently stunned:
        if(enemyAIState != AIState.Stun)
        {
            if (ticksSinceLastAIChange >= ticksUntilAIChange)
            {
                // May not have random behaviors to worry about
                // set a new point if arriving?
                stateChanged = true;
                enemyAIState = AIState.Wander;
            }

            // If you can't hear the player, listen for it
            // and if you hear it, this state has priority
            if (!canHearPlayer)
            {
                bool heardPlayer = listenForPlayer();
                canHearPlayer = heardPlayer;
                if (canHearPlayer)
                {
                    enemyAIState = AIState.Alert;
                    stateChanged = true;
                }
            }
            else
            {
                // you can already hear the player
                // don't change the state
            }

            if (!canSeePlayer)
            {
                bool sawPlayer = lookForPlayer();
                canSeePlayer = sawPlayer;
                if (canSeePlayer)
                {
                    enemyAIState = AIState.Spotting;
                    stateChanged = true;
                }
            }
            else
            {
                // you could already see the player
                // override any state changes and ensure that
                // you are continuing to chase the player and 
                // in the chase state. 
                enemyAIState = AIState.Chasing;
                stateChanged = false;
            }

            if (stateChanged)
            {
                AIStateChanged();
            }
        }

        if (ticksSinceLastFootstep > ticksUntilFootstep)
        {
            playAlienFootstep();
        }

    }

    private bool listenForPlayer()
    {
        if ((hearing.lastLoudestSound != Vector3.zero) && (hearing.lastLoudestSound != null))
        {
            return true;
        } else
        {
            return false;
        }
    }

    private bool lookForPlayer()
    {
        return false;
    }

    private void playAlienFootstep()
    {
        ticksSinceLastFootstep = 0;
        if (!audioSource.isPlaying)
        {
            currentClip = alienFootstepAudioClips[Random.Range(0, alienFootstepAudioClips.Count)];
            audioSource.clip = currentClip;
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.volume = Random.Range(volumeMin, volumeMax);
            audioSource.PlayOneShot(currentClip);
        }
        //            Debug.Log("Leave Walking Sound.");
    }

    private void playAlienStunScream()
    {
        //always override the existing alien sound 
        audioSource.clip = alienStunScreamClip;
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = 0.75f;
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void playAlienIdleScream()
    {
        //always override the existing alien sound 
        audioSource.clip = alienIdleClip;
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.volume = 0.75f;
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void AIStateChanged()
    {
        ticksSinceLastAIChange = 0;
        if(enemyAIState == AIState.Deciding)
        {
            // Temporary State to look, listen, or wander
            // set to wandering for now
            enemyAIState = AIState.Wander;
        }

        if(enemyAIState == AIState.Wander)
        {
            enemyAIState = AIState.Wandering;
            agent.destination = gm.getRandomWanderingSpot().position;
            agent.speed = alienWanderSpeed;
        }

        if((enemyAIState == AIState.Wandering) && canHearPlayer)
        {
            enemyAIState = AIState.Alert;

        }

        if((enemyAIState == AIState.Alert))
        {
            // Play alerted sound?
            if((hearing.lastLoudestSound != Vector3.zero)||(hearing.lastLoudestSound != null))
            {
                agent.destination = hearing.lastLoudestSound;
            }
            agent.speed = alienSearchSpeed;
            enemyAIState = AIState.Searching;
        }

    }

    private void onPowerChanged(bool powerState)
    {
        if (powerState)
        {
            enemyAIState = AIState.Stun;
            alienAnimator.SetBool("isStunned", true);
            hearing.sc.enabled = false;
            hearing.heardSounds.Clear();
            playAlienStunScream();
            canHearPlayer = false;
        } else
        {
            enemyAIState = AIState.Deciding;
            alienAnimator.SetBool("isStunned", false);
            hearing.sc.enabled = true;
            // Lets do this on aggro
            //playAlienIdleScream();
        }
        AIStateChanged();
    }

    private void onTick()
    {
        ticksSinceLastAIChange += 1;
        ticksSinceLastFootstep += 1;
    }


    private void OnDisable()
    {
        GameManager.Instance.onPowerChange -= onPowerChanged;
    }
}

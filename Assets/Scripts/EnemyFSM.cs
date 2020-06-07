using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyFSM : MonoBehaviour
{

     // enum to keep state
    
    public enum ENEMY_STATE {PATROL,CHASE,ATTACK};

    public enum PersonState { SAVE,DEAD,DEDUCT}

    public PersonState currentpersonState = PersonState.SAVE; 
    // we need a function get the current state
   
    [SerializeField]
    private ENEMY_STATE curentState;
    // we need property to access current state

    public ENEMY_STATE CurrentState
    {
        get { return curentState; }
        set { 
            curentState = value;

            // stop all courotines
            StopAllCoroutines();
            switch (curentState)
            {
                case ENEMY_STATE.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case ENEMY_STATE.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
                case ENEMY_STATE.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
                    
            }

        
        }
    }

    // what about some referneces

    private CheckMyVision checkMyVision;

    private NavMeshAgent agent = null;

    private Transform playerTransform = null;

    //reference to patrol destination

    private Transform patrolDestoination = null;

    private Health playerHealth = null;

    private float maxDamage =10f;

    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        // playerTransform = GameObject.FindGameObjectsWithTag("Player");
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerTransform = playerHealth.GetComponent<Transform>();

    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestoination = destinations[Random.Range(0, destinations.Length)]
            .GetComponent<Transform>();
        CurrentState = ENEMY_STATE.PATROL;
        
    }

    public IEnumerator EnemyPatrol()
    {
        while (curentState == ENEMY_STATE.PATROL)
        {
            
            checkMyVision.sensitity = CheckMyVision.enmSensitivity.HIGH;
            agent.isStopped=false;
            agent.SetDestination(patrolDestoination.position);
            while (agent.pathPending)
                yield return null;
            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.CHASE;
                yield break;
            }
            yield break;
        }
        yield break;
    }

    public IEnumerator EnemyChase()
    {
       while(curentState==ENEMY_STATE.CHASE)
        {
            // in this let us keep sensitvity low

            checkMyVision.sensitity = CheckMyVision.enmSensitivity.LOW;
            // The idea is go to last known position
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastknownSighting);
            while(agent.pathPending)
            {
                yield return null;
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
             currentpersonState  = PersonState.DEDUCT;
                agent.isStopped = true;
                // what should if rech destination
                if (!checkMyVision.targetInSight)
                    CurrentState = ENEMY_STATE.ATTACK;
                else
                    CurrentState = ENEMY_STATE.ATTACK;
             

            
            }
            yield return null;
        }
    }

    public IEnumerator EnemyAttack()
    {
        while (curentState == ENEMY_STATE.ATTACK)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            while (agent.pathPending)
                yield return null;
            
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                  CurrentState = ENEMY_STATE.CHASE;

            }

            else {
                playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
                 
     
            
        }
            if (playerHealth.HealthPoints < 0)
            {
                currentpersonState = PersonState.DEAD;
            }


            yield return null;
        }
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

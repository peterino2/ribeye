using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Gameplay.Stats;

public class AIController : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Transform destination = null;
    [SerializeField] private Patrolling patrolling = null;
    [SerializeField] private Animator animator = null;

    [Header("Specifcations")]
    [SerializeField] private States startingState = States.Stand;
    [SerializeField] private AnimationStates startingAnimationState = AnimationStates.Idle;

    [Header("Debugging")]
    [SerializeField] public AnimationStates animationState = AnimationStates.Idle;
    [SerializeField] public States state = States.Stand;
    [SerializeField] public FallingStates fallingState = FallingStates.NotFalling;
    [SerializeField] public GroundStates groundState = GroundStates.InAir;

    [Header("Testing")]
    [SerializeField] public bool activateCrawling = false;
    [SerializeField] private float fallingSpeed = 0.85f;
    [SerializeField] private AnimationFinished fallingFinished = null;
    [SerializeField] private AnimationAttackFinished attackingFinished = null;
    [SerializeField] private AnimationMutantSwipingFinished mutantSwipingFinished = null;
    [SerializeField] private float runningSpeed = 3.5f;
    [SerializeField] private float stoppingDistance = 1f;

    //Declare privates
    [NonSerialized] public NavMeshAgent navMeshAgent = null;
    private Vector3 previousTargetLocation = Vector3.zero;
    private int hash = 0;

    #endregion

    #region Enumerators

    public enum States {
        Stand, GoToDestination, Patrol
    }

    public enum AnimationStates {
        Idle, FastRun, MutantSwiping, Attack, FallingIdle, FallingToLand, Crawling, FastSlap
    }

    public enum FallingStates {
        NotFalling, Falling, HitGround
    }

    public enum GroundStates {
        InAir, Grounded
    }

    #endregion

    #region Constants

    //Declare private constants
    private const string STATE = "State";

    #endregion

    #region Start and initialization

    private void Start() {
        //Check if no destination
        if (!destination) {
            //Set target
            destination = GameObject.Find("CharacterController").transform;
        }
        //Set
        navMeshAgent = GetComponent<NavMeshAgent>();
        //Set starting state
        state = startingState;
        navMeshAgent.angularSpeed = 360 * 8;
        //Set
        hash = Animator.StringToHash(STATE);
        //Play animation
        PlayAnimation(startingAnimationState);
        //Set destination
        if (destination != null && state == States.GoToDestination) {
            //Set destination now
            SetDestinationNow();
        }
        //Check to initialize
        patrolling?.Initialize();
        //Check
        if (animationState == AnimationStates.FastRun) {
            //Set speed
            navMeshAgent.speed = runningSpeed;
        }
        navMeshAgent.acceleration = runningSpeed;
    }

    private void PlayAnimation(AnimationStates animationStateToBe) {
        //Set
        animationState = animationStateToBe;
        //Play animation
        animator.SetInteger(hash, (int)animationState);
    }

    private void SetDestinationNow() {
        //Set
        previousTargetLocation = destination.position;
        //Set
        navMeshAgent.SetDestination(destination.position);
    }

    #endregion


    public bool FastSlapFinished = false;

    private void Update() {
        //Check
        if (activateCrawling) {
            //Play animation
            PlayAnimation(AnimationStates.Crawling);
            //Set destination
            navMeshAgent.SetDestination(destination.position);
            //Reset
            activateCrawling = false;
        }
        //Check state
        switch (state) {
            case States.Stand:
                break;
            case States.GoToDestination:
                GoToDestinationState();
                break;
            case States.Patrol:
                //Check
                if (patrolling != null) {
                    //Patrol
                    patrolling.Patrol();
                }
                break;
        }
        //Check if about to fall
        if (navMeshAgent.isOnOffMeshLink) {
            //Set
            fallingState = FallingStates.Falling;
            //Set speed
            navMeshAgent.speed = fallingSpeed;
            //Play animation
            PlayAnimation(AnimationStates.FallingIdle);
        } else if (navMeshAgent.isOnNavMesh) {
            //Check state
            if (fallingState == FallingStates.Falling) {
                //Change state
                fallingState = FallingStates.HitGround;
                //Play animation
                PlayAnimation(AnimationStates.FallingToLand);
            }
        }
        //Check
        if (animationState == AnimationStates.FallingToLand) {
            //Check
            if (fallingFinished.AnimationHasFinished) {
                //Change state
                fallingState = FallingStates.NotFalling;
                //Change speed
                navMeshAgent.speed = runningSpeed;
                //Play animation
                PlayAnimation(AnimationStates.FastRun);
                //Reset
                fallingFinished.AnimationHasFinished = false;
            }
        }

        if (FastSlapFinished)
        {
            FastSlapFinished = false;
            PlayAnAttackingAnimation();
        }
        
        //Check
        if (attackingFinished.AnimationHasFinished) {
            //Reset
            attackingFinished.AnimationHasFinished = false;
            //Play an attacking animation
            PlayAnAttackingAnimation();
        }
        //Check
        if (mutantSwipingFinished.AnimationHasFinished) {
            //Reset
            mutantSwipingFinished.AnimationHasFinished = false;
            //Play an attacking animation
            PlayAnAttackingAnimation();
        }
        
        //Check if attacking
        if (animationState == AnimationStates.Attack || animationState == AnimationStates.MutantSwiping || animationState == AnimationStates.FastSlap) {
            //Check distance
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, AttackTargetRotation, 720f*Time.deltaTime);
            
            if (Vector3.Distance(navMeshAgent.transform.position, destination.position) > stoppingDistance) {
                //Continue
                navMeshAgent.isStopped = false;
                //Play animation
                PlayAnimation(AnimationStates.FastRun);
                //Set destination
                navMeshAgent.SetDestination(destination.position);
            }
        } else if (animationState == AnimationStates.FastRun) {
            //Check distance
            if (Vector3.Distance(navMeshAgent.transform.position, destination.position) <= stoppingDistance) {
                //Play an attacking animation
                PlayAnAttackingAnimation();
            } else {
                //Continue
                navMeshAgent.isStopped = false;
                //Set destination
                navMeshAgent.SetDestination(destination.position);
            }
        }
    }

    private Quaternion AttackTargetRotation;

    private void PlayAnAttackingAnimation() {
        //Stop
        navMeshAgent.isStopped = true;
        //Prepare to play an attack
        var p = FindObjectOfType<RibPlayer>(); // todo replace with static
        var rv = p.transform.position - transform.position;
        rv.y = 0;
        AttackTargetRotation = Quaternion.LookRotation(rv);
        int seq = UnityEngine.Random.Range(0, 3);
        
        if (seq == 0) {
            //Play animation
            // fuck it
            PlayAnimation(AnimationStates.Attack);
        } else if(seq == 1)
        {
            //Play animation
            PlayAnimation(AnimationStates.MutantSwiping);
        } else if(seq == 2)
        {
            PlayAnimation(AnimationStates.FastSlap);
        }
    }

    private void GoToDestinationState() {
        //Check
        if (destination.position != previousTargetLocation) {
            //Set destination now
            SetDestinationNow();
        }
    }

}
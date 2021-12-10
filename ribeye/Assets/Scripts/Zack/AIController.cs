using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

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
    [SerializeField] private AnimationStates animationState = AnimationStates.Idle;
    [SerializeField] private States state = States.Stand;
    [SerializeField] private FallingStates fallingState = FallingStates.NotFalling;
    [SerializeField] private GroundStates groundState = GroundStates.InAir;

    [Header("Testing")]
    [SerializeField] private bool activateCrawling = false;
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

    private enum States {
        Stand, GoToDestination, Patrol
    }

    private enum AnimationStates {
        Idle, FastRun, MutantSwiping, Attack, FallingIdle, FallingToLand, Crawling
    }

    private enum FallingStates {
        NotFalling, Falling, HitGround
    }

    private enum GroundStates {
        InAir, Grounded
    }

    #endregion

    #region Constants

    //Declare private constants
    private const string STATE = "State";

    #endregion

    #region Start and initialization

    private void Start() {
        //Set
        navMeshAgent = GetComponent<NavMeshAgent>();
        //Set starting state
        state = startingState;
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
        if (animationState == AnimationStates.Attack || animationState == AnimationStates.MutantSwiping) {
            //Check distance
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

    private void PlayAnAttackingAnimation() {
        //Stop
        navMeshAgent.isStopped = true;
        //Prepare to play an attack
        if (UnityEngine.Random.Range(0, 2) == 0) {
            //Play animation
            PlayAnimation(AnimationStates.Attack);
        } else {
            //Play animation
            PlayAnimation(AnimationStates.MutantSwiping);
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
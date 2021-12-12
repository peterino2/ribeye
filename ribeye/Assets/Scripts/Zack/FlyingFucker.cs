using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingFucker : MonoBehaviour {

    [SerializeField] private Animator animator = null;
    [SerializeField] private float timeBeforeSwitching = 0.5f;

    private float timeBeforeSwitchingSubtract = 0f;

    private int hash = 0;

    private void Start() {
        hash = Animator.StringToHash(STATE);
    }

    private const string STATE = "State";

    private void Update() {
        if (timeBeforeSwitchingSubtract > 0f) {
            timeBeforeSwitchingSubtract -= Time.deltaTime;
        }
        if (timeBeforeSwitchingSubtract <= 0f) {
            if (Random.Range(0, 4) == 0) {
                animator.SetInteger(hash, Random.Range(0, 5));
            } else {
                animator.SetInteger(hash, Random.Range(0, 4));
            }
            timeBeforeSwitchingSubtract = timeBeforeSwitching;
        }
    }

}
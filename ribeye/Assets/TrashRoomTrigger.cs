using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Stats;
using UnityEngine;

public class TrashRoomTrigger : EventDirector
{
    [SerializeField]
    private AIController[] ais;
    
    [SerializeField]
    private AIController[] nonCrawlers;

    private int killCount = 0;
    public RibInteractable door;

    public override void EnemyKilled(EntityBase entity)
    {
        killCount += 1;
    }

    private bool ready = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!ready)
        {
            return;
        }
        if (other.gameObject.GetComponent<RibPlayer>())
        {
            ready = false;
            foreach (var ai in ais)
            {
                ai.activateCrawling = true;
            }

            foreach (var ai in nonCrawlers)
            {
                ai.enabled = true;
                //ai.animationState = AIController.AnimationStates.FastRun;
            }
        }
    }

    private bool doorReady = true;

    IEnumerator SlowTick()
    {
        while (true)
        {
            if (killCount >= 7 && doorReady)
            {
                doorReady = false;
                door.Activate(PeterFPSCharacterController._controller);
                this.enabled = false;
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(1.0f);
        }
    }


    private int doorOpenCount;
    private void Start()
    {
        foreach(var ai in ais)
        {
            ai.GetComponent<EntityBase>().owner = this;
        }
        foreach(var ai in nonCrawlers)
        {
            ai.GetComponent<EntityBase>().owner = this;
        }

        doorOpenCount = ais.Length + nonCrawlers.Length;
        StartCoroutine(SlowTick());
    }
}

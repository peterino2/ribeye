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

    public override void OnSlowTick(float dt)
    {
        if (killCount >= 6 && doorReady)
        {
            doorReady = false;
            door.Activate(PeterFPSCharacterController._controller);
            this.enabled = false;
            StopAllCoroutines();
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
        Init();
    }
}

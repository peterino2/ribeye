using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Stats;
using UnityEngine;

public class HighHallwayDirector : EventDirector
{
    public GameObject prefabEnemy;
    public Transform[] spawnLocs;
    public RibInteractable door;

    public int spawnCount = 10;

    int enemies_killed = 0;

    public override void EnemyKilled(EntityBase entity)
    {
        enemies_killed += 1;
    }

    private bool doorReady = true;
    public override void OnSlowTick(float dt)
    {
        if (enemies_killed >= (spawnCount*spawnLocs.Length))
        {
            if (doorReady)
            {
                door.Activate(PeterFPSCharacterController._controller);
                doorReady = false;
            }
        }
    }

    
    private bool first = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!first) return;
        
        if (other.GetComponent<RibPlayer>())
        {
            first = false;
            DoSpawn();
        }
    }

    void DoSpawn()
    {
        foreach (var loc in spawnLocs)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                var x = Instantiate(prefabEnemy, loc.position, Quaternion.identity);
                x.GetComponent<EntityBase>().owner = this;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

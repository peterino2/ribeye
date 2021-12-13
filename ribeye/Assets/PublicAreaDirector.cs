using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Stats;
using UnityEngine;

public class PublicAreaDirector : EventDirector
{
    static public PublicAreaDirector director;

    private void Awake()
    {
        director = this;
    } 

    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    
    [Serializable]
    public class Wave
    {
        public Spawner[] spawners;
    }

    public Wave[] waves;
    

    private int waveId = 0;
    private bool started = false;

    public override void OnSlowTick(float dt)
    {
    }

    public override void EnemyKilled(EntityBase entity)
    {
        enemiesKilled += 1;
    }

    private bool first = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RibPlayer>())
        {
            if (first)
            {
                NextWave();
            }
            first = false;
        }
    }

    private int waveid = 0;
    public void NextWave()
    {
        StartWave(waveid);
        waveid += 1;
    }
    void StartWave(int wave)
    {
        foreach (var spawner in waves[wave].spawners)
        {
            spawner.DoSpawn();
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

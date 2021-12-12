using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Stats;
using UnityEngine;

public class SpawnerTrigger : MonoBehaviour
{

    public Spawner[] spawners;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RibPlayer>())
        {
            foreach (var spawner in spawners)
            {
                spawner.DoSpawn();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Stats;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RibPlayer>())
        {
            other.GetComponent<RibPlayer>().DoFilmGrainDeath();
        }
    }
}

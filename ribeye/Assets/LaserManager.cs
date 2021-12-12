using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

public class LaserManager : MonoBehaviour
{
    // Start is called before the first frame update
    public LazerBody[] lasers;
    void Start()
    {
    }

    // Update ifs called once per frame

    private float selectNextLaserTime = 2f;
    private bool first = true;


    void Update()
    {
        
        if (selectNextLaserTime > 0)
        {
            selectNextLaserTime -= Time.deltaTime;
            if (selectNextLaserTime <= 0)
            {
                if (first)
                {
                    first = false;
                    lasers[0].Fire();
                    selectNextLaserTime = 3f;
                    return;
                }
                
                int closestLaser = 0;
                float close = 3500f;

                bool found = false;
                foreach (var laser in lasers)
                {
                    if (laser.player)
                    {
                        laser.StartCharge();
                        selectNextLaserTime = 10f;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    selectNextLaserTime = 1f;
                }
            }
        }
    }
}

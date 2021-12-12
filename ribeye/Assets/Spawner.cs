using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject enemy;
    public int spawnCount;

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool first = true;
    public bool DoOnce = true;
    public void DoSpawn()
    {
        if (DoOnce)
        {
            if (!first) return;
        }
        
        for (int i = 0; i < spawnCount; i++ )
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
        }

        first = false;
    }
}

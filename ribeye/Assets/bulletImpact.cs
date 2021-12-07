using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletImpact : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float deathtime = 2.0f;
    // Update is called once per frame
    void Update()
    {
        deathtime -= Time.deltaTime;
        if(deathtime < 0) Destroy(gameObject);
    }
}

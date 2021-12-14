using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedRun : MonoBehaviour
{
    // Start is called before the first frame update

    private float t = 0;
    public TextMeshProUGUI text;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        int minutes = Mathf.FloorToInt(t / 60F); 
        int seconds = Mathf.FloorToInt(t - minutes * 60);
        text.text = String.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, (t*1000)%1000);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static SoundManager _soundManager;
    
    void Start()
    {
        _soundManager = GetComponentInChildren<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

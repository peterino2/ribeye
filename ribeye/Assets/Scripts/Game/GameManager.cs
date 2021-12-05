using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static SoundManager _soundManager;

    public static int hitSoundIndex = 2;
    public static int critSoundIndex = 3;
    public static int heavySoundIndex = 4;
    
    void Start()
    {
        _soundManager = GetComponentInChildren<SoundManager>();
    }

    public static void playHitSound(Vector3 loc)
    {
        _soundManager.PlaySound(hitSoundIndex, loc, volume:0.3f);
    }
    
    public static void playCritSound(Vector3 loc)
    {
        _soundManager.PlaySound(hitSoundIndex, loc, volume:0.3f);
    }
    
    public static void playHeavySound(Vector3 loc)
    {
        _soundManager.PlaySound(heavySoundIndex, loc, volume:0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

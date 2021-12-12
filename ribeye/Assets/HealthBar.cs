using System.Collections;
using System.Collections.Generic;
using Gameplay.Stats;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private RibPlayer player;
    private Slider slider;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<RibPlayer>();
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            slider.value = player.health / player.maxHp;
        }
        else
        {
            print("what");
            player = FindObjectOfType<RibPlayer>();
        }
    }
}

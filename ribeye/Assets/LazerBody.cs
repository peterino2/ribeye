using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Stats;
using UnityEngine;

public class LazerBody : MonoBehaviour
{
    public MeshRenderer laservisible;
    public MeshRenderer lasercharger;
    // Start is called before the first frame update
    void Start()
    {
        laservisible.enabled = false;
        lasercharger.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RibPlayer>())
        {
            player = other.GetComponent<RibPlayer>();
        }
    }

    public RibPlayer player;
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<RibPlayer>())
        {
            player = null;
        }
    }

    public Transform basePosition;
    IEnumerator DoCharge()
    {
        GameManager._soundManager.PlaySound(7, basePosition.position);
        lasercharger.enabled = true;
        yield return new WaitForSeconds(8);
        lasercharger.enabled = false;
        Fire();
    }

    IEnumerator DoFire()
    {
        GameManager._soundManager.PlaySound(6, basePosition.position);
        laservisible.enabled = true;
        float t = 0f;
        while (t < 6)
        {
            t += 0.1f;
            if (player)
            {
                player.TakeDamageFromSource(1, basePosition.gameObject);
                player.DoFilmGrainExplosion();
            }
            yield return new WaitForSeconds(0.1f);
        }
        laservisible.enabled = false;
    }

    public void Fire()
    {
        StartCoroutine(DoFire());
    }
    
    public void StartCharge()
    {
        StartCoroutine(DoCharge());
    }
}

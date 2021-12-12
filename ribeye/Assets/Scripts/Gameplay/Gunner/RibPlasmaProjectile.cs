using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DefaultNamespace;
using Gameplay.Stats;
using UnityEngine;

public class RibPlasmaProjectile : MonoBehaviour
{
    public float velocity = 32f;
    public float damage = 10f;
    public int team; // team 0 is friendly
    public RibPlasmaExplosionHurtbox hurtbox; 
    public GameObject explosionPrefab;

    private Vector3 CachedDir;
    
    public Rigidbody _rigidbody;
    // Start is called before the first frame update
    
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.GetComponent<RibPlayer>())
        {
            GameManager._soundManager.PlaySound(12, transform.position);
            hurtbox.Detonate(damage, team);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void SetVelocityDir(Vector3 dir)
    {
        CachedDir = dir.normalized;
        print(CachedDir);
        _rigidbody.AddForce(dir * velocity, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
    }
}

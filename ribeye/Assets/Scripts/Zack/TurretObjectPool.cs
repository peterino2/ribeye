using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretObjectPool : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private GameObject prefabBullet = null;
    [SerializeField] private Transform turretMuzzlePoint = null;

    [Header("Specifications")]
    [SerializeField] private int numberOfBullets = 100;

    //Declare privates
    private BulletPool[] bulletPool = null;

    #endregion

    #region Classes

    private class BulletPool {
        //Declare
        public GameObject TheGameObject = null;
        public Rigidbody TheRigidbody = null;
        //Constructor
        public BulletPool(GameObject theGameObject) {
            //Set
            TheGameObject = theGameObject;
            TheRigidbody = theGameObject.GetComponent<Rigidbody>();
            //Disable
            TheGameObject.SetActive(false);
        }
    }

    #endregion

    #region Start and initialization

    private void Start() {
        //Expand
        bulletPool = new BulletPool[numberOfBullets];
        //Loop
        for (int i = 0; i < bulletPool.Length; i++) {
            //Create
            bulletPool[i] = new BulletPool(Instantiate(prefabBullet, transform));
        }
    }

    #endregion

}
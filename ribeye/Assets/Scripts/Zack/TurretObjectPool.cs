using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretObjectPool : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private TurretBehavior turretBehavior = null;
    [SerializeField] private GameObject prefabBullet = null;

    [Header("Specifications")]
    [SerializeField] private int numberOfBullets = 100;

    //Declare privates
    private BulletPool[] bulletPool = null;
    private int index = 0;

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

    #region Public functions

    public void Shoot() {
        //Reset physics
        ResetPhysicsOfBullet(index);
        //Position object
        bulletPool[index].TheGameObject.transform.SetPositionAndRotation(turretBehavior.turretMuzzlePoint.transform.position, turretBehavior.turretMuzzlePoint.transform.rotation);
        //Enable
        bulletPool[index].TheGameObject.SetActive(true);
        //Add force
        bulletPool[index].TheRigidbody.AddForce(turretBehavior.turretMuzzlePoint.forward * turretBehavior.shootingForce);
        //Increase
        index = (index + 1) % numberOfBullets;
    }

    private void ResetPhysicsOfBullet(int indexToBe) {
        //Reset
        bulletPool[indexToBe].TheRigidbody.velocity = Vector3.zero;
        bulletPool[indexToBe].TheRigidbody.angularVelocity = Vector3.zero;
    }

    #endregion

    #region Update

    private void Update() {
        
    }

    #endregion

}
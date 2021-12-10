using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretObjectPool : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private TurretBehavior turretBehavior = null;
    [SerializeField] private GameObject prefabBullet = null;
    [SerializeField] private string Tag = "CharacterControllerTag";

    [Header("Specifications")]
    [SerializeField] private int numberOfBullets = 100;
    [SerializeField] private float lifetime = 3f;

    //Declare privates
    private BulletPool[] bulletPool = null;
    private int index = 0;
    private RaycastHit[] raycastHits = new RaycastHit[10];

    #endregion

    #region Classes

    private class BulletPool {
        //Declare
        public GameObject TheGameObject = null;
        public float LifetimeUp = 0f;
        public Vector3 LastPosition = Vector3.zero;
        //Constructor
        public BulletPool(GameObject theGameObject) {
            //Set
            TheGameObject = theGameObject;
            //Disable
            TheGameObject.SetActive(false);
        }
    }

    #endregion

    #region Constants

    private const float LIFETIME_START = 0.00001f;

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

    public void Shoot(Transform shootFrom) {
        //Position object
        bulletPool[index].TheGameObject.transform.SetPositionAndRotation(shootFrom.position, shootFrom.rotation);
        //Enable
        bulletPool[index].TheGameObject.SetActive(true);
        //Set lifetime
        bulletPool[index].LifetimeUp = LIFETIME_START;
        //Increase
        index = (index + 1) % numberOfBullets;
    }

    public void CheckLifetimes(int layerMask) {
        //Loop
        for (int i = 0; i < bulletPool.Length; i++) {
            //Check lifetime
            if (bulletPool[i].LifetimeUp > 0f) {
                //Increase
                bulletPool[i].LifetimeUp += Time.deltaTime;
                //Set last position
                bulletPool[i].LastPosition = bulletPool[i].TheGameObject.transform.position;
                //Move bullet
                bulletPool[i].TheGameObject.transform.position += bulletPool[i].TheGameObject.transform.forward * turretBehavior.shootingForce;
                //Declare
                float distance = Vector3.Distance(bulletPool[i].LastPosition, bulletPool[i].TheGameObject.transform.position);
                //Raycast
                if (Physics.Raycast(bulletPool[i].TheGameObject.transform.position, -bulletPool[i].TheGameObject.transform.forward, out RaycastHit raycastHit, distance, layerMask, QueryTriggerInteraction.Ignore)) {
                    //Check
                    if (raycastHit.transform.gameObject.CompareTag(Tag)) {
                        //Player hit
                        Debug.Log("PLAYER HIT");
                    }
                    //Disable object
                    DisableObject(i);
                } else {
                    //Check lifetime
                    if (bulletPool[i].LifetimeUp >= lifetime) {
                        //Disable object
                        DisableObject(i);
                    }
                }
            }
        }
    }

    private void DisableObject(int indexToBe) {
        //Disable object
        bulletPool[indexToBe].TheGameObject.SetActive(false);
        //Reset lifetime
        bulletPool[indexToBe].LifetimeUp = 0f;
    }

    #endregion

}
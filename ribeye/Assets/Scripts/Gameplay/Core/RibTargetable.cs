using System;
using Gameplay.Stats;
using UnityEngine;

namespace Game
{
    public class RibTargetable: MonoBehaviour
    {
        [SerializeField] public Transform targetingLoc = null;
        public bool targetSet = true;

        public EntityBase target;

        private void Start()
        {
            if (targetingLoc == null)
            {
                targetingLoc = transform;
            }

            target = GetComponent<EntityBase>();
        }
    }
}
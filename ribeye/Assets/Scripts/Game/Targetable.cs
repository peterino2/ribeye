using System;
using UnityEngine;

namespace Game
{
    public class Targetable: MonoBehaviour
    {
        [SerializeField] public Transform targetingLoc = null;
        public bool targetSet = true;

        private void Start()
        {
            if (targetingLoc == null)
            {
                targetingLoc = transform;
            }
        }
    }
}
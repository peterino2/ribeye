using System;
using UnityEngine;

namespace Game
{
    public class Targetable: MonoBehaviour
    {
        [SerializeField] public Transform targetingLoc = null;
        public bool targetSet;

        private void Start()
        {
            if (targetingLoc != null)
            {
                targetSet = true;
            }
            else
            {
                targetSet = false;
            }
        }
    }
}
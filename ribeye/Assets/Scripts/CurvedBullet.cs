using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class CurvedBullet : MonoBehaviour
    {
        public Vector3[] points;

        public int pointIndex = 0;

        public float velocity;
        
        public int numberOfPoints;

        private TrailRenderer _tr;

        private void Start()
        {
            _tr = GetComponent<TrailRenderer>();
            pointIndex = 256;
            _tr.enabled = false;
        }

        public void SetPositions(Vector3[] positions)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                points[i] = positions[i];
            }
            pointIndex = 0;
            transform.position = positions[0];
            _tr.enabled = true;
            end = false;
            doDie = false;
        }

        private float deathTime = 0f;
        private bool doDie = false;

        private void Update()
        {
            if ((pointIndex > points.Length ) || (points.Length <= 0) || end)
            {
                _tr.enabled = false;
                gameObject.SetActive(false);
                return;
            }

            deathTime -= Time.deltaTime;
            var dist = (points[pointIndex] - transform.position).magnitude;
            var dv = velocity * Time.deltaTime;
            if (dv < dist)
            {
                transform.position += (points[pointIndex] - transform.position).normalized * dv;
            }
            else
            {
                if (pointIndex + 1 == points.Length)
                {
                    if (!doDie)
                    {
                        doDie = true;
                        deathTime = 0.1f;
                    }
                    
                    if (deathTime < 0)
                    {
                        end = true;
                        _tr.enabled = false;
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    pointIndex += 1;
                }
            }
        }

        private bool end = true;
        private void FixedUpdate()
        {
        }
    }
}
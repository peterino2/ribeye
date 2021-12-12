using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.PeterFPS
{
    public class HurtIndicatorUi : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Image[] images;
        
        private float[] opacity = {
            0f,0f,0f,0f
        };

        [SerializeField] private Transform tf;
        
        private int up = 0;
        private int right = 1;
        private int down = 2;
        private int left = 3;

        public void ShowHurtDirection(Vector3 source)
        {
            var direction = source - tf.position;
            var horizontalDir = direction;
            horizontalDir.y = 0;

            var q = Quaternion.Euler(direction);

            var angle = (q.eulerAngles.y - tf.eulerAngles.y + 180);
            if (angle > 360)
            {
                angle -= 360;
            }
            //print(String.Format("source: {3} {4} my_angle: {0} hurt_angle:{1} result: {2}", tf.eulerAngles.y, q.eulerAngles.y, angle, source, direction));
            int dir = up;

            if (angle > (90 - 45))
            {
                dir = right;
            }
            if (angle > (180 - 45))
            {
                dir = down;
            }
            if (angle > (270 - 45))
            {
                dir = left;
            }

            opacity[dir] = 1.0f;
        }

        public void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                if (opacity[i] > 0.0f)
                {
                    opacity[i] -= Time.deltaTime;
                    if (opacity[i] <= 0f)
                    {
                        opacity[i] = 0f;
                    }
                }
                images[i].color = new Color(1.0f, 1.0f, 1.0f, opacity[i]);
            }
        }
    }
}
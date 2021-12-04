using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunLean : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Transform player;
    [SerializeField] private AnimationCurve animCurve;
    public IEnumerator Lean()
    {
        float pos = 0;
        float ogYpos = player.position.y;
        while (pos < 1)
        {
            pos += Time.deltaTime * 2.5f;
            cam.rotation = Quaternion.Euler(new Vector3(0, 0, animCurve.Evaluate(pos)));
            player.position = new Vector3(player.position.x, ogYpos + animCurve.Evaluate(pos), player.position.z);
            yield return null;
        }
    }
}
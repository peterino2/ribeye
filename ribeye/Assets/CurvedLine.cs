using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedLine : MonoBehaviour
{
    // Start is called before the first frame update

    private LineRenderer lr;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }


    private float opacity = 0;
    public void Fade(bool autodie)
    {
        opacity = 1f;
        if (autodie)
        {
            Destroy(gameObject, 2f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (opacity > 0.1)
        {
            lr.enabled = true;
            lr = GetComponent<LineRenderer>();
            opacity -= Time.deltaTime * 4;
            lr.material.color = new Color(lr.material.color.r, lr.material.color.g, lr.material.color.b, opacity);
            lr.material.SetFloat("_EmissiveIntensity", 20*opacity);
        }
        else
        {
            lr.enabled = false;
        }
    }
}

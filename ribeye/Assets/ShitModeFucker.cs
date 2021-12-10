using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShitModeFucker : MonoBehaviour
{
    public Canvas canvasRoot;
    public RectTransform sourceTexture;
    private Vector2 root_size = new Vector2(480, 270);
    
    // Start is called before the first frame update
    void Start()
    {
        sourceTexture = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        var l = canvasRoot.pixelRect.size;
        var my_aspect_ratio = root_size.y / root_size.x;
        var target_aspect_ratio = l.y / l.x;
        sourceTexture.sizeDelta = l;
        //if (my_aspect_ratio > target_aspect_ratio)
        //{
        //    sourceTexture.sizeDelta = root_size * (canvasRoot.pixelRect.size.x/root_size.x);
        //}
        //else
        //{
        //    sourceTexture.sizeDelta = root_size * (canvasRoot.pixelRect.size.y/root_size.y);
        //}
    }
}

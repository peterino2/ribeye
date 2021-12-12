using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiIcon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Image[] images_active;
    
    public void Select()
    {
        foreach (var image in images_active)
        {
            image.enabled = true;
        }
    }
    
    public void Deselect()
    {
        foreach (var image in images_active)
        {
            image.enabled = false;
        }
    }
}

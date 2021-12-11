using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RibGunnerInventory : MonoBehaviour
{
    public Image[] icons;
    public Image[] selectors;
    
    [SerializeField] public TextMeshProUGUI ammoText;
    [SerializeField] public TextMeshProUGUI weaponText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideIcon(int index)
    {
        icons[index].gameObject.SetActive(false);
    }
    
    public void ShowIcon(int index)
    {
        icons[index].gameObject.SetActive(true);
    }

    public void SetSelector(int index)
    {
        foreach (var sel in selectors)
        {
            sel.enabled = false;
        }
        if (index == -1)
        {
            return;
        }
        selectors[index].enabled = true;
        // selector.rectTransform.SetPositionAndRotation(icons[index].rectTransform.rect.position, Quaternion.identity);
    }
    
}

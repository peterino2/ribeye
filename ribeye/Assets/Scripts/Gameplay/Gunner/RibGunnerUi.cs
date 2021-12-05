using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Main ui for the gunner class itself.
public class RibGunnerUi : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TextMeshProUGUI gunnerDebug;
    private RibGunner _gunner;

    [SerializeField] private bool showDebug = true;

    private string m_debugText = "";
    void Start()
    {
        _gunner = FindObjectOfType<RibGunner>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDebug();
    }

    void HandleDebug()
    {
        if (gunnerDebug && showDebug)
        {
            gunnerDebug.text = m_debugText;
        }
    }
}

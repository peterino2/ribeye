using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class displayFramerate : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI _text;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string x = string.Format("FPS: {0}\nVsync: {1}\n", 1.0f / Time.deltaTime , QualitySettings.vSyncCount);
        _text.text = x;
    }
}

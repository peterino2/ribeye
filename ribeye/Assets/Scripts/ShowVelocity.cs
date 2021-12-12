using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowVelocity : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody _target;

    private TextMeshProUGUI _text;
    
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var a = _target.velocity;
        var b = a;
        b.y = 0;
        
        _text.text = string.Format("{0,8:N5} m/s\n{1,8:N5} m/s(Horizontal)", a.magnitude, b.magnitude);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowVelocity : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody _target;

    private Text _text;
    
    void Start()
    {
        _text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var x = _target.velocity;
        var y = x;
        y.y = 0;
        
        _text.text = string.Format("{0} m/s\n{1} m/s(Horizontal)", x.magnitude, y.magnitude);
    }
}

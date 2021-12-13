using System.Collections;
using System.Collections.Generic;
using Gameplay.Core;
using TMPro;
using UnityEngine;

public class HeldButton : RibInteractable
{
    // Start is called before the first frame update
    private HoldTimerUi tui;
    public float holdTimeLeft;
    public float holdTimeNeeded = 4f;
    public RibInteractable bind;
    private bool started = false;
    
    void Start()
    {
        tui = HoldTimerUi.ui;
    }

    public override void Activate(PeterFPSCharacterController controller)
    {
        _controller = controller;
        holdTimeLeft = holdTimeNeeded;
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            tui.watch(this);
            if (Input.GetKey(KeyCode.F) && (_controller._interactable == this))
            {
                holdTimeLeft -= Time.deltaTime;

                if (holdTimeLeft <= 0)
                {
                    bind.Activate(_controller);
                    tui.watch(null);
                    _controller._interactable = null;
                    ready = false;
                    started = false;
                }
            }
        }
    }
}

# ribeye

High speed first person shooter project;

this was added on integration

Just wanted to make something very quickly.

Premise; 

    So in titanfall, there's a smart pistol section that is incredibly fun.

Hierarchical Design Document


===== v0.1.0 =====
 
- high speed first person locomotion - Peter;
    - double jump
    - dash
    - slam/slide
        - slide: partial implementation
            - When sliding, an initial burst of speed is given
            - When we are below a certain speed, move slowly but have full control instead
            - When transitioning from sliding to air, we benefit from reduced gravity
        - slam: when in the air press a button to slam down to the ground and deal aoe damage.
    - wall-jump/wall running

- dumb enemy targets moving around - zack
    - enemys that walk around on the floor
    - enemies that walk around on walls
    - flying enemies
    - ragdolls on death

- weaponry

    - smart pistol
        - automatically locks on and hits targets in the head when the trigger is pressed.
        - hitscan/borderline hitscan

    - revolver pistol
        - single shot high powered revolver.

===== v0.2.0 =====

- items
- powerups

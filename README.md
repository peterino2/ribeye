# ribeye

High speed first person shooter project;

this was added on integration

Just wanted to make something very quickly.

Premise; 

    So in titanfall, there's a smart pistol section that is incredibly fun.

Hierarchical Design Document


===== v0.1.0 =====

- gameplay base
    - health
    - energy
 
- high speed first person locomotion - Peter;
    - double jump
        - single jump has been implemented, double jump next?
    - dash
        - should be done with a coroutine setting a constant velocity
    - slam/slide
        - slide: partial implementation
            - When sliding, an initial burst of speed is given
            - When we are below a certain speed, move slowly but have full control instead
            - When transitioning from sliding to air, we benefit from reduced gravity
            - todo: need to shrink capsule collider when sliding to be able to duck under 1 unit tall squares.
        - slam: when in the air press the slide button to slam down to the ground stun, pushback and stun enemies.
            - the slam is working.
    - wall-jump/wall running
        - how to do?
        - extra colliders on the sides? 
        - if active and traveling at a shallow angle of attack, then engage wallrunning, 
            - preset horizontal and up velocity against the wall
            - speed increases steadily

- dumb enemy targets moving around - zack
    - enemys that walk around on the floor
    - enemies that walk around on walls
    - flying enemies
    - ragdolls on death

- weaponry and combat

    - smart pistol
        - automatically locks on and hits targets in the head when the trigger is pressed, high rate of fire, always accurate.
        - hitscan/borderline hitscan,
        - alt fire to shot single high powered shot, that is NOT auto-aimed.
    
    - blade & hook
        - short range melee weapon
        - alt fire to throw grappling hook
            - choose to pull self to target or pull target to self.
            - can latch onto floors, walls or enemies
 
    - thermobaric launcher
        - heavy weapon with delay before firing and self-knockback
            - slight delay between impact and explosion
            - am implosion/levitating effect happens during this delay
        - significant cooldown
        - high damage

- gameplay loop?
    - need a designer for this;

===== v0.2.0 =====

- items
- powerups
- health pickups
- levels
- hazards and more enemies

===== v0.3.0 =====
- models
- sounds
- not going further than this.

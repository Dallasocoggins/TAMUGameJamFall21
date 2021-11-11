using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is abstract, so in order to make MovingObject objects you have to you a child class
// This is just so GameManager has a consistent object type to use for all the different moving things that need to be slowed down
public abstract class MovingObject : MonoBehaviour
{
    // What we multiply Time.deltaTime by
    // 1 to be normal, <1 to slow down, >1 to speed up
    // 0 will stop it
    public abstract void SetSpeedMultiplier(float s);

    // true if associated with player 1, false if associated with player 2
    public abstract bool IsPlayer1Area();
}

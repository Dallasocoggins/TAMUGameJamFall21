using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyGearController : MovingObject
{
    // What we multiply Time.deltaTime by
    // 1 to be normal, <1 to slow down, >1 to speed up
    // 0 will stop it
    public float speedMultiplier;

    // true if associated with player 1, false if associated with player 2
    public bool player1Area;

    // How fast the visual component spins, make it negative to change direction (in degrees/second)
    public float rotationSpeed;

    // What gets rotated
    private Transform visual;

    // Start is called before the first frame update
    void Start()
    {
        visual = transform.GetChild(0);

        GameManager.instance.AddMovingObject(this);
    }

    // Update is called once per frame
    void Update()
    {
        visual.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime * speedMultiplier);
    }

    public override void SetSpeedMultiplier(float s)
    {
        speedMultiplier = s;
    }

    public override bool IsPlayer1Area()
    {
        return player1Area;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyGearController : MonoBehaviour
{
    // How fast the visual component spins, make it negative to change direction (in degrees/second)
    public float rotationSpeed;

    // What gets rotated
    private Transform visual;

    // Start is called before the first frame update
    void Start()
    {
        visual = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        visual.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}

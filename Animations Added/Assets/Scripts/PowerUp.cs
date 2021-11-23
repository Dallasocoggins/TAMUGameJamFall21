using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // How long it takes to reappear
    public float resetTime;

    // Keeps track of how long it has been since the powerup was collected
    private float timeSinceCollected;

    // False normally, true after being collected unitl being reset
    private bool collected;

    // Start is called before the first frame update
    void Start()
    {
        collected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceCollected > 0)
        {
            timeSinceCollected -= Time.deltaTime;
        }
        else if (collected)
        {
            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;

            collected = false;
        }
    }

    public void Collect()
    {
        //Destroy(this.gameObject);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        collected = true;
        timeSinceCollected = resetTime;
    }

}

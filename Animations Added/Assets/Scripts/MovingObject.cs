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

    // For synchronization with each other and the music
    // Resets the position back to what it was initially
    public abstract void Reset();

    /***
    public bool isRewinding = false;

    public List<Vector2> positions;

    public Rigidbody2D rb;

    public float nextTimeToSearch = 0;

    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Not IEnumerator version (less jank)

    private void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    void Rewind()
    {
        if (positions.Count > 0)
        {
            transform.position = positions[0];
            positions.RemoveAt(0);
        } else
        {
            stopRewind();
        }
    }

    private void Record()
    {
        if (nextTimeToSearch <= Time.time)
        {
            positions.Insert(0, transform.position);
            nextTimeToSearch = Time.time + 0.5f;

        }
    }

    public void startRewind()
    {
        Debug.Log("start rewind");
        isRewinding = true;
    }

    public void stopRewind()
    {
        isRewinding = false;
    } **/

    //Ienumerator version (very jank)
    /**
    //50 times per second
    private void FixedUpdate()
    {
        if (!isRewinding)
        {
            Record();
        }
    }

    IEnumerator Rewind(float sec, float time, int numOfSteps)
    {
        int num = (int)(sec * 50);
        int step = num / numOfSteps;
        float timeSteps = time / numOfSteps;
        if (positions.Count > 0)
        {
            for (int i = 0; i <= num; i += step)
            {
                if (i > positions.Count)
                {
                    transform.position = positions[positions.Count-1];
                }
                else
                {
                    transform.position = positions[i];
                }
                yield return new WaitForSeconds(timeSteps);
            }
            if(positions.Count > num)
                positions.Clear();
            else
                positions.RemoveRange(0, num);
            stopRewind();
        } else
        {
            stopRewind();
        }
    }

    // Called 50 times a second
    private void Record()
    {
        positions.Insert(0, transform.position);
    }

    public void startRewind(float sec, float time, int numOfSteps)
    {
        Debug.Log("start rewind");
        isRewinding = true;
        StartCoroutine(Rewind(sec, time, numOfSteps));
    }

    public void stopRewind()
    {
        isRewinding = false;
    } 

    //For when the player dies or is teleported
    public void clearPos()
    {
        positions.Clear();
    } **/

}

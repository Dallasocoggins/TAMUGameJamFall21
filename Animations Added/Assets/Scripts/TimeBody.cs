using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour
{
    public bool isRewinding = false;

    List<Vector2> positions;

    Rigidbody2D rb;

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
        positions.Insert(0, transform.position);
    }

    public void startRewind()
    {
        Debug.Log("start rewind");
        isRewinding = true;
    }

    public void stopRewind()
    {
        Debug.Log("stop rewind");
        isRewinding = false;
    }

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
    } **/

    public void clearPos()
    {
        positions.Clear();
    }
}

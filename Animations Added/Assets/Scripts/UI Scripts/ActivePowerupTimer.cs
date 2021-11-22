using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePowerupTimer : MonoBehaviour
{

    public GameObject player;

    [SerializeField]
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        doTimerAnimation(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // play the countdown timer for powerups that last for a few seconds (slow, speed, stop)
    public void doTimerAnimation(bool powerupActivated)
    {
        animator.SetBool("StartedTimer", powerupActivated);
    }

    // play activated powerup animation (for all powerups)
    public void doActivationAnimation(bool powerupActivated)
    {
        animator.SetBool("ActivatedPowerup", powerupActivated);
    }

}

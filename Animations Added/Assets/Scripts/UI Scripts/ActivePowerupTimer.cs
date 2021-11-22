using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePowerupTimer : MonoBehaviour
{

    public GameObject player;

    [SerializeField]
    public Animator animator;


    public void doTimerAnimation(bool powerupActivated)
    {
        animator.SetBool("ActivatedPowerup", powerupActivated);
        Debug.Log("playing animation??");
    }

}

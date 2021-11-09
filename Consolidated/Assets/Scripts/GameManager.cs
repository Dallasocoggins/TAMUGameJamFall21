using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    // VARIABLES
    public static GameManager instance;

    public static string[] powerUpOptions = { "slow", "skip", "speed",
                                        "pause", "rewind" };

    public static string currentPowerUp;

    public GameObject player1;
    public Transform P1pos;

    public GameObject player2;
    public Transform P2pos;

    // The list of moving objects associated with player 1
    // Note: if we ever change scenes, this and the next list need to be reset
    public List<MovingObject> player1MovingObjects;

    // Same thing but for player 2
    public List<MovingObject> player2MovingObjects;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // throw new NotImplementedException();
    }

    // When a moving object registers itself with the game manager
    public void AddMovingObject(MovingObject m)
    {
        if (m.IsPlayer1Area())
            player1MovingObjects.Add(m);
        else
            player2MovingObjects.Add(m);
    }

    #region powerup
    public void CollectPowerUp()
    {
        currentPowerUp = powerUpOptions[UnityEngine.Random.Range(0, powerUpOptions.Length)];
        
        Debug.Log("currentPowerUp: " + currentPowerUp);

    }
    #endregion powerup

    public void usePowerUp(GameObject Player)
    {
        GameObject target = null;
        String tag = "";
        try { 
            if (Player.GetComponent<PlayerController>().getIsPlayerOne())
            {
                target = player2;
                tag = "P2";
            } else
            {
                target = player1;
                tag = "P1";
            }
        }
        catch {

            }
        stopTime(target);

    }

    public void stopTime(GameObject p)
    {
        p.GetComponent<PlayerController>().stopTime();
    }

}

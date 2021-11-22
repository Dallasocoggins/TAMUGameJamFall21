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

    public string[] powerUpOptions = { "slow", "skip", "speed", "pause", "rewind" };

    // Stores what power up is in effect
    private string currentPowerUp;

    // True if the power up is being applied dto player 1
    private bool powerUpOnPlayer1;

    // How long the power up will last for
    private float powerUpTime;

    // How long power ups last
    public float powerUpLength;

    // How much to slow down / speed up things by
    public float timeSlowFactor;

    public GameObject player1;
    public Transform P1pos;

    public GameObject player2;
    public Transform P2pos;

    public GameObject audioManager;
    private float musicTime;

    public int laps;

    // Variables forrewind
    public float rewindFor;
    public float stopSec;

    /***
    //[SerializeField]
    private float timeTaken;
    //Number of the positions in between the rewind start and rewind end
    [SerializeField]
    private int numOfSteps; ***/

    // The list of moving objects associated with player 1
    // Note: if we ever change scenes, this and the next list need to be reset
    public List<MovingObject> player1MovingObjects;

    // Same thing but for player 2
    public List<MovingObject> player2MovingObjects;


    private bool canWin;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        // The music used to wait a second before coming in, because otherwise it can be a little off
        // This should be fixed with reset now
        musicTime = 0;

        currentPowerUp = "";

        canWin = true;
    }

    // Update is called once per frame
    private void Update()
    {
        // I had some issues where the music didn't quite line up with the moving platform movement, so I decided to let GameManager take care of repeating it
        if (musicTime >= 120)
        {
            ((AudioSource)audioManager.GetComponent(typeof(AudioSource))).Play();
            musicTime = 0;

            // To synchronize with the music
            if (currentPowerUp == "")
            {
                foreach (MovingObject element in player1MovingObjects)
                {
                    element.Reset();
                }

                foreach (MovingObject element in player2MovingObjects)
                {
                    element.Reset();
                }
            }
        }
        musicTime += Time.deltaTime;


        if (powerUpTime > 0)
        {
            powerUpTime -= Time.deltaTime;

        }

        else if (currentPowerUp != "")
        {
            if (currentPowerUp == "slow" || currentPowerUp == "speed")
            {
                List<MovingObject> movingObjects = powerUpOnPlayer1 ? player1MovingObjects : player2MovingObjects;

                foreach (MovingObject element in movingObjects)
                {
                    element.SetSpeedMultiplier(1);
                }
            }

            currentPowerUp = "";
            Debug.Log("Powerup is no longer active");
        }

        /**if(player1 == null || player2 == null)
        {
            FindPlayer();
        } **/
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

    // Takes in the string saying what power up the player is trying to use, and who to use it on
    // If we can't apply the powerup right now, returns false
    // If we sucessfully apply the powerup, returns true
    public bool UsePowerUp(string powerUp, bool onPlayerOne)
    {
        if (currentPowerUp == "")
        {
            if (powerUp == "slow")
            {
                List<MovingObject> movingObjects = onPlayerOne ? player1MovingObjects : player2MovingObjects;

                foreach (MovingObject element in movingObjects)
                {
                    element.SetSpeedMultiplier(1 / timeSlowFactor);
                }
            }
            else if (powerUp == "speed")
            {
                List<MovingObject> movingObjects = onPlayerOne ? player1MovingObjects : player2MovingObjects;

                foreach (MovingObject element in movingObjects)
                {
                    element.SetSpeedMultiplier(timeSlowFactor);
                }
            } else if (powerUp == "rewind")
            {
                if (onPlayerOne)
                    StartCoroutine(rewindTime(player2));
                else
                    StartCoroutine(rewindTime(player1));
            } else if (powerUp == "stop")
            {
                if (onPlayerOne)
                    stopTime(player2);
                else
                    stopTime(player1);
            }
            else
            {
                return false;
            }

            currentPowerUp = powerUp;
            powerUpOnPlayer1 = onPlayerOne;
            powerUpTime = powerUpLength;

            return true;
        }

        return false;
    }


    // Since I did the power ups a little differently, I commented this out
    /*
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
    */

    #endregion powerup

    

    public void stopTime(GameObject p)
    {
        StartCoroutine(p.GetComponent<PlayerController>().stopTime(stopSec));
    }

    //Less jank version of rewind
    IEnumerator rewindTime(MovingObject p)
    {
        p.GetComponent<TimeBody>().startRewind();
        yield return new WaitForSeconds(rewindFor);
        p.GetComponent<TimeBody>().stopRewind();
    }

    //Less jank version of rewind
    IEnumerator rewindTime(GameObject p)
    {
        p.GetComponent<TimeBody>().startRewind();
        yield return new WaitForSeconds(rewindFor);
        p.GetComponent<TimeBody>().stopRewind();
    }

    //Jank version of rewind
    /**
    public void rewindTime(GameObject p)
    {
        p.GetComponent<TimeBody>().startRewind(rewindFor, timeTaken, numOfSteps);
    }
    **/


    #region player input
    public void OnMoveP1(InputAction.CallbackContext context)
    {
        player1.GetComponent<PlayerController>().OnMove(context);
    }

    public void OnJumpP1(InputAction.CallbackContext context)
    {
        player1.GetComponent<PlayerController>().OnJump(context);
    }

    public void OnPowerP1(InputAction.CallbackContext context)
    {
        player1.GetComponent<PlayerController>().OnPower(context);
    }

    public void OnDashP1(InputAction.CallbackContext context)
    {
        player1.GetComponent<PlayerController>().OnDash(context);
    }


    public void OnMoveP2(InputAction.CallbackContext context)
    {
        player2.GetComponent<PlayerController>().OnMove(context);
    }

    public void OnJumpP2(InputAction.CallbackContext context)
    {
        player2.GetComponent<PlayerController>().OnJump(context);
    }

    public void OnPowerP2(InputAction.CallbackContext context)
    {
        player2.GetComponent<PlayerController>().OnPower(context);
        //Testing purposes
        /**List<MovingObject> movingObjects = player2MovingObjects;

        foreach (MovingObject element in movingObjects)
        {
            StartCoroutine(rewindTime(element));
        } **/
        //StartCoroutine(rewindTime(player2));
        //rewindTime(player2);
    }

    public void OnDashP2(InputAction.CallbackContext context)
    {
        player2.GetComponent<PlayerController>().OnDash(context);
    }
    #endregion

    public int getLapsNeeded()
    {
        return laps;
    }

    public IEnumerator win(PlayerController player)
    {
        if (canWin)
        {
            canWin = false;
            GameObject text = GameObject.FindGameObjectWithTag("Text");
            if (player.getIsPlayerOne())
            {
                text.GetComponent<ShowText>().setText("Player one wins!");
            }
            else
            {
                text.GetComponent<ShowText>().setText("Player two wins!");
            }

            yield return new WaitForSeconds(5);

            SceneManager.LoadScene(0);
        }
    }

    // This function doesn't work for some reason for player 1 but I've been staring at it for like an hour at this point
    // and I have no idea what in the world is wrong so I decided to do something very different
    /**
    void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject searchResult in searchResults) {
                if (searchResult != null)
                {
                    if (searchResult.GetComponent<PlayerController>().getIsPlayerOne())
                    {
                        Debug.Log("Found Player 1 again");
                        player1 = searchResult;
                    }
                    else
                    {
                        Debug.Log("Found Player 2 again");
                        player2 = searchResult;
                    }
                }
            }

            nextTimeToSearch = Time.time + 0.5f;

        }
    }
    **/
}

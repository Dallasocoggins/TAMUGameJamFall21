using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{

    // VARIABLES
    public static GameManager instance;

    public static string[] powerUpOptions = { "slow", "skip", "speed",
                                        "pause", "rewind" };

    public static string currentPowerUp;

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

    // Update is called once per frame
    private void Update()
    {
        throw new NotImplementedException();
    }

    #region powerup
    public void CollectPowerUp()
    {
        currentPowerUp = powerUpOptions[UnityEngine.Random.Range(0, powerUpOptions.Length)];
        
        Debug.Log(currentPowerUp);

    }
    #endregion powerup

}

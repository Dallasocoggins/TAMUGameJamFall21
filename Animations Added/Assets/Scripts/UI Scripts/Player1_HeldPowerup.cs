using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1_HeldPowerup : MonoBehaviour
{
        public Image image;

    // list of the powerup images
    // [slow-down.png, speed-up.png, stop.png, rewind.png]
    public Sprite[] powerUpAssets;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    // no held powerup -- so hide the image
    public void hideActivePowerup()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    // show the powerup and make it whatever it is
    public void showActivePowerup(string powerUp)
    {
        image = GetComponent<Image>();
        image.enabled = true;

        switch (powerUp) {
            case "slow":
                image.sprite = powerUpAssets[0];
                Debug.Log("Slow-down displayed - p1 held powerup");
                break;
            case "speed":
                image.sprite = powerUpAssets[1];
                Debug.Log("speed displayed - p1 held powerup");
                break;
            case "pause":
                image.sprite = powerUpAssets[2];
                Debug.Log("stop displayed - p1 held powerup");
                break;
            case "rewind":
                image.sprite = powerUpAssets[3];
                Debug.Log("rewind displayed - p1 held powerup");
                break;
            case "skip":
                Debug.Log("skip not displayed bc we don't have an image lol - p1 held powerup");
                break;
            default:
                Debug.Log("invalid display - p1 held powerup");
               break;

         }
    }
}

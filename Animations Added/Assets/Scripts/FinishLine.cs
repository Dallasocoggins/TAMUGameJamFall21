using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public int nextSceneLoad;

    // Start is called before the first frame update
    void Start()
    {
        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController controller = collision.GetComponent<PlayerController>();
            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCount - 1)
            {
                controller.addLap();
                Debug.Log("You have completed " + controller.getLaps() + " laps");
                if (controller.getLaps() >= GameManager.instance.getLapsNeeded())
                {
                    Debug.Log("YOU WIN THE GAME");
                    GameManager.instance.win(controller);
                } else
                {
                    controller.setCheckpoint(0);
                    controller.Respawn();
                }
            }
          /**  else
            {
                //Move to next level
                SceneManager.LoadScene(nextSceneLoad);

                //Setting int for Index
                if (nextSceneLoad > PlayerPrefs.GetInt("levelAt"))
                {
                    PlayerPrefs.SetInt("levelAt", nextSceneLoad);
                }
            } **/
        }
    }
}

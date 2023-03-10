using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.InfiniteRunnerEngine
{
    public class RockGate : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collidingObject)
        {
            PlayableCharacter player = collidingObject.GetComponent<PlayableCharacter>();
            // we verify that the colliding object is a PlayableCharacter with the Player tag. If not, we do nothing.			
            if (collidingObject.gameObject.tag == "Scissors")
            {
                collidingObject.gameObject.SetActive(false);
                // we ask the LevelManager to kill the character
                LevelManager.Instance.KillCharacter(player);
                Debug.Log("Triggered gate as:" + collidingObject.gameObject.tag + ". You fucked up");
                return;
            }
            else if (collidingObject.gameObject.tag == "Paper")
            {
                collidingObject.gameObject.SetActive(false);
                // we ask the LevelManager to kill the character
                LevelManager.Instance.KillCharacter(player);
                Debug.Log("Triggered gate as:" + collidingObject.gameObject.tag + ". You fucked up");
                return;
            }
            else if (collidingObject.gameObject.tag == "Rock")
            {
                this.gameObject.SetActive(false);
                Debug.Log("Triggered gate as:" + collidingObject.gameObject.tag + ". You may pass");
                return;
            }
            else
            {
                // we ask the LevelManager to kill the character
                LevelManager.Instance.KillCharacter(player);
                return;
            }
        }
    }
}
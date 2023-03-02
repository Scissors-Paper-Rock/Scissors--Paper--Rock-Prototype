using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class Gate : MonoBehaviour
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
        // we verify that the colliding object is a PlayableCharacter with the Player tag. If not, we do nothing.			
        if (collidingObject.gameObject.tag == "Scissors")
        {
            collidingObject.gameObject.SetActive(false);
            Debug.Log("Triggered gate as:" + collidingObject.gameObject.tag);
            return;
        }
        else if (collidingObject.gameObject.tag == "Paper")
        {
            collidingObject.gameObject.SetActive(false);
            Debug.Log("Triggered gate as:" + collidingObject.gameObject.tag);
            return;
        }
        else if (collidingObject.gameObject.tag == "Rock")
        {
            collidingObject.gameObject.SetActive(false);
            Debug.Log("Triggered gate as:" + collidingObject.gameObject.tag);
            return;
        }
        else
        {
            return;
        }
    }
}

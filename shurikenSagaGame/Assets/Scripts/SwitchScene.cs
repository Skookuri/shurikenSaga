using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class SwitchScene : MonoBehaviour
{
    public string sceneToSwitchTo; // Name of the scene to switch to
    public Transform player; // Reference to the player object
    
    public bool isDungeon1; //bools for scene ur switching FROM
    public bool isDungeon2; //bools for scene ur switching FROM
    public bool isDungeon3; //bools for scene ur switching FROM

    // Called when another collider enters this object's collider (make sure one collider is set to "Is Trigger")
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.transform == player) {
            //Debug.Log("Collision detected with player"); // Check if the message appears
            HomeOnLoad.isDungeon1 = isDungeon1;
            HomeOnLoad.isDungeon2 = isDungeon2;
            HomeOnLoad.isDungeon3 = isDungeon3;
            SceneManager.LoadScene(sceneToSwitchTo);
        }
    }
}

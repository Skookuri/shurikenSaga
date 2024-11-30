using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedEnemies : MonoBehaviour
{
    public GameObject shadowEnemy;
    public GameObject overworldEnemy;
    private GameHandler g;
    Vector2 sharedPosition;

    //private bool isOverworld;

    private void Start()
    {
        // Initialize isOverworld based on overworldEnemy's active state
        g = GameObject.Find("GameHandler").GetComponent<GameHandler>();
    }

    private void Update()
    {
        // Check if the active state of overworldEnemy has changed
        if (g.doneSwitchingRealms)
        {
            //Debug.Log("SWITCHING, GETTING POSITIONS");
            // Determine the shared position
            //Vector2 sharedPosition = g.isOverWorld ? overworldEnemy.transform.position : shadowEnemy.transform.position;

            // Update the position of the newly active enemy
            if(GameHandler.isOverWorld)
            {
                overworldEnemy.transform.position = sharedPosition;
                shadowEnemy.transform.position = sharedPosition;
            } else
            {
                shadowEnemy.transform.position = sharedPosition;
                overworldEnemy.transform.position = sharedPosition;
            }

            g.doneSwitchingRealms = false;
        }
        
        if (GameHandler.isOverWorld)
        {
            //Debug.Log("getting overworld pos: " + sharedPosition);
            sharedPosition = overworldEnemy.transform.position;
        }
        else
        {
            //Debug.Log("getting shadow pos: " + sharedPosition);
            sharedPosition = shadowEnemy.transform.position;
        }
    }
}

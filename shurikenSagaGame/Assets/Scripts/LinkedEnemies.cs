using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedEnemies : MonoBehaviour
{
    public GameObject shadowEnemy;
    public GameObject overworldEnemy;
    private GameHandler g;

    private BasicEnemyValues oBasicVals;
    private BasicEnemyValues sBasicVals;

    public bool hasProcessedSwitch = true;
    private Vector2 sharedPosition;

    private void Start()
    {
        oBasicVals = overworldEnemy.GetComponent<BasicEnemyValues>();
        sBasicVals = shadowEnemy.GetComponent<BasicEnemyValues>();
        g = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        // Initialize sharedPosition from the active realm
        sharedPosition = GameHandler.isOverWorld ? overworldEnemy.transform.position : shadowEnemy.transform.position;
    }

    private void Update()
    {
        // Check if either enemy is dead
        if (oBasicVals.isDead || sBasicVals.isDead)
        {
            overworldEnemy.SetActive(false);
            shadowEnemy.SetActive(false);
            return; // No further processing needed
        }

        // Handle realm switching
        if (!hasProcessedSwitch)
        {
            // Sync the position from the previous active realm
            if (GameHandler.isOverWorld)
            {
                overworldEnemy.transform.position = sharedPosition;
                //overworldEnemy.SetActive(true);
                //shadowEnemy.SetActive(false);
                Debug.Log($"{name}: Switched to Overworld. Position updated.");
            }
            else
            {
                shadowEnemy.transform.position = sharedPosition;
                //shadowEnemy.SetActive(true);
                //overworldEnemy.SetActive(false);
                Debug.Log($"{name}: Switched to Shadow. Position updated.");
            }

            // Mark this enemy as having processed the switch
            hasProcessedSwitch = true;
        }

        // Continuously update the shared position from the currently active enemy
        if (GameHandler.isOverWorld && overworldEnemy.activeSelf)
        {
            sharedPosition = overworldEnemy.transform.position;
        }
        else if (!GameHandler.isOverWorld && shadowEnemy.activeSelf)
        {
            sharedPosition = shadowEnemy.transform.position;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatAds : MonoBehaviour
{

    GameHandler gh;
    [SerializeField]
    GameObject[] npcs;
    public bool completed = true;
    public bool startBoss = false;
    [SerializeField]
    BoxCollider2D b;
    // Start is called before the first frame update
    void Start()
    {
        startBoss = false;
        gh = GameObject.Find("GameHandler").GetComponent<GameHandler>();
    }

    void Update()
    {
        completed = true;

        foreach (GameObject g in npcs)
        {
            if (g.activeSelf)
            {
                completed = false;
                break;
            }
        }

        //Debug.Log($"All NPCs defeated: {completed}, Overworld: {GameHandler.isOverWorld}, Switching: {gh.switching}");

        if (completed && !GameHandler.isOverWorld && !gh.switching)
        {
            startBoss = true;
            //Debug.Log("Boss fight started!");
        }
        else
        {
            startBoss = false;
        }
    }
}

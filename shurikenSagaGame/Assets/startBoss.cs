using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startBoss : MonoBehaviour
{
    public bool startFinalBoss = false;
    [SerializeField]
    public GameObject boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "player")
        {
            startFinalBoss = true;
            GameObject.Find("BackgroundMusic").GetComponent<BGSoundScript>().PlayBoss();
            boss.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}

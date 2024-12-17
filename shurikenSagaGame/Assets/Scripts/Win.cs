using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    [SerializeField]
    public GameObject startBoss;

    [SerializeField]
    public GameObject boss;


    // Update is called once per frame
    void Update()
    {
        if (startBoss.GetComponent<startBoss>().startFinalBoss && boss.activeSelf == false)
        {
            SceneManager.LoadScene("WinScene");
        }
    }


}

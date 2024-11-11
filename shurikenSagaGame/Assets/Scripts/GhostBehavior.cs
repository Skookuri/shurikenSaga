using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GhostBehavior : MonoBehaviour
{
    [SerializeField]
    Transform player;
    public float detRange;
    public float sideAmp;
    private Vector2 v1;
    private Vector2 v2;
    private float timePassedInAggro = 0f;
    public float cooldown = 2f;  // Time for each floaty movement segment
    public float floatSpeed = 1f; // Speed multiplier for movement
    private bool pickFloatDir = true;

    Vector2 baseFloat;
    private void Start()
    {
        baseFloat = transform.position;
    }

    private void Update()
    {
        float pDistance = Vector2.Distance(transform.position, player.position);
        if (pDistance < detRange)
        {
            Float();
        } 
        else
        {
            Aggro();
        }
    }

    private void Float()
    {
        // Side-to-side oscillation based on starting position
        float sideMovement = Mathf.Sin(Time.time) * sideAmp;
        transform.position = new Vector2(baseFloat.x + sideMovement, transform.position.y);
    }

    private void Aggro()
    {
        timePassedInAggro += Time.deltaTime;

        if(timePassedInAggro < cooldown)
        {
            if(pickFloatDir)
            {
                v1 = new Vector2(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 3));
                v2 = new Vector2(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 3));
                pickFloatDir = false;
            }
            
        }
    }
}
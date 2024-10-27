using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour{
    public Transform player;          
    public float threshold = 0f;         
    public float smoothSpeed = 0.025f;    
    public float moveAmountMultiplier = 1f;   

    private float targetPositionX;     
    private float targetPositionY;      
    private float moveAmountX;
    private float moveAmountY;
    private bool isMovingX = false;       
    private bool isMovingY = false;      

    void Start(){
        Camera cam = Camera.main;
        moveAmountY = cam.orthographicSize * 2 * moveAmountMultiplier;       
        moveAmountX = moveAmountY * cam.aspect;                            
        targetPositionX = transform.position.x;                   
        targetPositionY = transform.position.y;             
    }

    void Update(){
        Vector3 viewPos = Camera.main.WorldToViewportPoint(player.position);

        if (!isMovingX){
            if (viewPos.x >= 1 - threshold){
                MoveCamera(Vector3.right * moveAmountX, true);
            }
            else if (viewPos.x <= threshold){
                MoveCamera(Vector3.left * moveAmountX, true);
            }
        }
        
        if (!isMovingY){
            if (viewPos.y >= 1 - threshold){
                MoveCamera(Vector3.up * moveAmountY, false);
            }
            else if (viewPos.y <= threshold){
                MoveCamera(Vector3.down * moveAmountY, false);
            }
        }

        float newX = Mathf.Lerp(transform.position.x, targetPositionX, smoothSpeed);
        float newY = Mathf.Lerp(transform.position.y, targetPositionY, smoothSpeed);
        transform.position = new Vector3(newX, newY, transform.position.z);

        if (Mathf.Abs(transform.position.x - targetPositionX) < 0.1f){
            isMovingX = false;
        }

        if (Mathf.Abs(transform.position.y - targetPositionY) < 0.1f){
            isMovingY = false;
        }
    }

    void MoveCamera(Vector3 direction, bool horizontal){
        if (horizontal){
            isMovingX = true;
            targetPositionX += direction.x;
        }
        else{
            isMovingY = true;
            targetPositionY += direction.y;
        }
    }
}
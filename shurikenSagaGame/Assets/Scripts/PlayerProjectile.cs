using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour{

      public int damage = 1;
      public GameObject hitEffectAnim;
      public float SelfDestructTime = 4.0f;
      public float SelfDestructVFX = 0.5f;
      public int counter = 0;
    private float destroyTimer = 0f;
      public SpriteRenderer projectileArt;
    [SerializeField]
    private float rotationSpeed;

    private 

      void Start(){
           projectileArt = GetComponentInChildren<SpriteRenderer>();
           selfDestruct();
      }

    void Update()
    {

        destroyTimer += Time.deltaTime;
      transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
      if (counter > 500){
        Destroy (gameObject);
      }
      counter ++;

        if (destroyTimer > 6f) { Destroy (gameObject); }
    }

    //if the bullet hits a collider, play the explosion animation, then destroy the effect and the bullet
    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("enemy"))
        {
            Debug.Log("HIT ENEMY");
            BasicEnemyValues enemyVals = other.GetComponent<BasicEnemyValues>();
            enemyVals.TakeDamage(10);
            enemyVals.DealKB(gameObject);

            Destroy (gameObject);
            //StartCoroutine(selfDestructHit();
        }
    }

      IEnumerator selfDestructHit(GameObject VFX){
            yield return new WaitForSeconds(SelfDestructVFX);
            Destroy (VFX);
            Destroy (gameObject);
      }

      IEnumerator selfDestruct(){
            yield return new WaitForSeconds(SelfDestructTime);
            Destroy (gameObject);
      }
}
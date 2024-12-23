using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicEnemyValues : MonoBehaviour
{
    [SerializeField]
    public int damageAmount;
    [SerializeField]
    public int health;
    public bool damaged = false;
    [SerializeField]
    public Color hurtColor; // Color to flash when hurt
    [SerializeField]
    public Material material;
    private Color startColor; // Original color of the enemy
    [SerializeField]
    private SpriteRenderer sp;
    [SerializeField]
    public Rigidbody2D rb;

    private AudioSource hitAudio;

    public KozouBehavior kozouBehavior;

    private GameObject player;

    public bool isDead = false;
    public bool kozouHit = false;

    private void Start()
    {
        player = GameObject.Find("player");
        if (sp == null)
        {
            sp = GetComponent<SpriteRenderer>(); // Auto-assign SpriteRenderer if not set
        }
        if (material == null)
        {
            startColor = sp.color;
        } else
        {
            startColor = material.GetColor("_ColorShift");
        }

        hitAudio = GameObject.Find("shuriHitAudioSource").GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        hitAudio.Play();
        damaged = true;
        // Reduce health
        health -= damage;
        if (health <= 0)
        {
            Kill(); // Call kill method if health reaches 0
        }
        else
        {
            StartCoroutine(FlashWhite()); // Trigger flash effect
        }
        
        if (gameObject.TryGetComponent<KozouBehavior>(out var kozouBehavior))
        {
            kozouBehavior.StopAllCoroutines();
            kozouBehavior.ResetBools();
            kozouBehavior.lastKnownPosition = player.transform.position;
            kozouBehavior.MoveToLastKnownPosition();
        }
        
    }

    public void DealKB(GameObject s)
    {
        Vector2 knockbackDirection = (transform.position - s.transform.position).normalized * 0.55f;
        if (rb != null)
        {
            // If Rigidbody2D is available, use it for knockback
            rb.AddForce(knockbackDirection, ForceMode2D.Impulse);
        }
        else
        {
            // Use KozouBehavior's coroutine to apply knockback
            /*
            if (gameObject.TryGetComponent<KozouBehavior>(out var kozouBehavior))
            {
            }
            */
            kozouBehavior = gameObject.GetComponent<KozouBehavior>();
            if (kozouBehavior != null)
            {
                kozouBehavior.StartCoroutine(ApplyKnockbackCoroutine(knockbackDirection * 3.5f));
                //kozouBehavior.hurt = true;
            }

        }
    }

    // Coroutine to handle knockback
    private IEnumerator ApplyKnockbackCoroutine(Vector2 direction)
    {
        float knockbackDuration = 0.3f; // Duration of knockback
        float knockbackSpeed = 3f;     // Speed of knockback

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            transform.position += (Vector3)(direction * knockbackSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void Kill()
    {
        if (gameObject.name == "boss_monk_meditate")
        {
            //GameObject.Find("Dungeon1ExitDoorPair").transform.GetChild(1).position = transform.position;
            //GameObject.Find("Dungeon1ExitDoorPair").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("ScrollDropper").transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log("SET SCROLL ACTIVE");
        }
        isDead = true;
        gameObject.SetActive(false); // Disable the enemy
    }

    private IEnumerator FlashWhite()
    {

        // Change the sprite's color to the hurt color
        if (material == null)
        {
            Debug.Log("CHANGING COLOR");
            sp.color = hurtColor;
        } else
        {
            Debug.Log("CHANGING MATERIAL COLOR");
            material.SetColor("_ColorShift", hurtColor);
        }

        // Wait for a short duration
        yield return new WaitForSeconds(0.13f);

        // Revert back to the original color
        if (material == null)
        {
            sp.color = Color.white;
        }
        else
        {
            material.SetColor("_ColorShift", startColor);
        }
        damaged = false;
    }
}


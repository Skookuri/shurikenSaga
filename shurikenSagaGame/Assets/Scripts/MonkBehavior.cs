using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonkBehavior : MonoBehaviour
{
    [SerializeField]
    DefeatAds da; // Reference to some logic controlling when the boss starts
    bool firstTeleport = true; // Flag for first teleport
    [SerializeField]
    BoxCollider2D tArea; // Area within which the monk can teleport
    [SerializeField]
    float teleportInterval = 8f; // Time interval between teleports
    [SerializeField]
    float fadeDuration = 1f; // Duration for fading in and out
    SpriteRenderer spriteRenderer; // SpriteRenderer for the monk
    [SerializeField]
    private NotificationBehavior n;
    public bool monkActivated = true;
    private BasicEnemyValues b;
    

    [SerializeField]
    GameObject minion;

    private float timeSinceSpawn = 0f;

    private List<GameObject> allMinions = new List<GameObject>(); // Use List for dynamic management

    void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        b = GetComponent<BasicEnemyValues>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on Monk. Ensure a SpriteRenderer is attached.");
        }

        // Optionally, teleport immediately at the start if desired
        if (firstTeleport)
        {

            firstTeleport = false;
        }
    }

    void Update()
    {
        // Only start teleporting if the boss fight has started
        if (da.startBoss && !IsInvoking(nameof(TeleportRoutine)))
        {

            if (monkActivated)
            {
                GetComponent<BoxCollider2D>().enabled = true;
                n.startNotif("The monk is testing your strength! You must defeat him!");
            }
            InvokeRepeating(nameof(TeleportRoutine), 0f, teleportInterval);
        }

        timeSinceSpawn += Time.deltaTime;

        // Spawn a minion every 3.8 seconds, up to a maximum of 4 minions
        if (timeSinceSpawn >= 6f && allMinions.Count < 4 && da.startBoss)
        {
            SpawnMinion();
            timeSinceSpawn = 0f;
        }

        // Clean up inactive minions
        for (int i = allMinions.Count - 1; i >= 0; i--)
        {
            if (!allMinions[i].activeSelf)
            {
                Destroy(allMinions[i]);
                allMinions.RemoveAt(i);
            }
        }
    }

    void SpawnMinion()
    {
        GameObject spawnedMinion = Instantiate(minion, transform.position, Quaternion.identity);
        Renderer renderer = spawnedMinion.GetComponent<Renderer>();
        Material newMaterial = null;

        if (renderer != null)
        {
            newMaterial = new Material(renderer.material); // Clone the material
            renderer.material = newMaterial; // Assign the unique material to the Renderer
        }

        GhostBehavior ghostBehavior = spawnedMinion.GetComponent<GhostBehavior>();
        if (ghostBehavior != null && newMaterial != null)
        {
            ghostBehavior.material = newMaterial; // Call a method to set the material
        }

        allMinions.Add(spawnedMinion);
        Debug.Log("Spawned minion at position: " + transform.position);
    }

    void TeleportRoutine()
    {
        if (da.startBoss) // Ensure teleportation continues only during the boss fight
        {
            StartCoroutine(TeleportWithFade());
        }
        else
        {
            CancelInvoke(nameof(TeleportRoutine));
        }
    }

    IEnumerator TeleportWithFade()
    {
        if (spriteRenderer == null) yield break;

        // Fade to black
        yield return StartCoroutine(FadeToColor(Color.black));

        // Teleport to random position
        TeleportToRandomPosition();

        // Fade back to normal color
        yield return StartCoroutine(FadeToColor(Color.white));
    }

    void TeleportToRandomPosition()
    {
        if (tArea != null)
        {
            // Get random position within the bounds of the teleportation area
            Bounds bounds = tArea.bounds;
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            // Set monk's position to the new random position
            transform.position = new Vector3(randomX, randomY, transform.position.z);
        }
        else
        {
            Debug.LogWarning("Teleportation area (tArea) is not assigned.");
        }
    }

    IEnumerator FadeToColor(Color targetColor)
    {
        Color startColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        spriteRenderer.color = targetColor; // Ensure exact final color
    }
}
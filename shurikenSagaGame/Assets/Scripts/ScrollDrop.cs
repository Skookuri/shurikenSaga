using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDrop : MonoBehaviour
{
    private List<GameObject> ScrollJutsuPairs = new List<GameObject>(); // List of ScrollJutsuPair objects
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); // Stores the original positions of ScrollJutsuPairs

    [SerializeField] private float dropSpeed = 0.5f; // Speed at which the scrolls drop
    [SerializeField] private float yOffset = 0.7f; // How much higher the scrolls start

    void Start()
    {
        // Find all GameObjects in the scene with the "ScrollJutsuPair" tag
        GameObject[] scrollJutsuObjects = GameObject.FindGameObjectsWithTag("ScrollJutsuPair");

        // Add the GameObjects to the list and move them upwards initially
        foreach (GameObject scrollJutsuObject in scrollJutsuObjects)
        {
            ScrollJutsuPairs.Add(scrollJutsuObject);
            
            // Store the original position
            Vector3 originalPosition = scrollJutsuObject.transform.position;
            originalPositions[scrollJutsuObject] = originalPosition;
            
            // Move the object upwards by the yOffset
            scrollJutsuObject.transform.position = originalPosition + new Vector3(0, yOffset, 0);
            
            // Deactivate the instance initially
            scrollJutsuObject.SetActive(false);
        }

        if (ScrollJutsuPairs.Count == 0)
        {
            Debug.LogWarning("No ScrollJutsuPair objects found in the scene.");
        }
    }

    // Activate all ScrollJutsuPair objects in the scene and make them drop
    public void DropScrolls()
    {
        Debug.Log("Dropping scrolls");

        foreach (var scroll in ScrollJutsuPairs)
        {
            scroll.SetActive(true); // Activate the ScrollJutsuPair instance
            StartCoroutine(DropToPosition(scroll)); // Start the drop animation
        }
    }

    // Coroutine to animate the drop to the original position
    private IEnumerator DropToPosition(GameObject scroll)
    {
        Vector3 targetPosition = originalPositions[scroll]; // The original position to drop to
        while (Vector3.Distance(scroll.transform.position, targetPosition) > 0.01f)
        {
            // Smoothly move towards the target position
            scroll.transform.position = Vector3.MoveTowards(
                scroll.transform.position, 
                targetPosition, 
                dropSpeed * Time.deltaTime
            );
            yield return null; // Wait for the next frame
        }

        // Snap to the target position to avoid tiny discrepancies
        scroll.transform.position = targetPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDrop : MonoBehaviour
{
    public GameObject ScrollJutsuPairPrefab; // Reference to the ScrollJutsuPair prefab
    private List<GameObject> ScrollJutsuPairs = new List<GameObject>(); // List of ScrollJutsuPair objects

    void Start()
    {
        // Locate all GameObjects in the scene with the "ScrollJutsuPair" prefab.
        ScrollJutsuPairs.Clear();
        ScrollJutsuPair[] existingInstances = FindObjectsOfType<ScrollJutsuPair>(); // Find all ScrollJutsuPair components in the scene
        
        foreach (ScrollJutsuPair instance in existingInstances)
        {
            // Add the GameObject of each ScrollJutsuPair instance to the list
            ScrollJutsuPairs.Add(instance.gameObject);
            instance.gameObject.SetActive(false); // Deactivate the instance initially
        }

        if (ScrollJutsuPairs.Count == 0)
        {
            Debug.LogWarning("No ScrollJutsuPair objects found in the scene.");
        }
    }

    // Activate all ScrollJutsuPair objects in the scene
    public void DropScrolls()
    {
        Debug.Log("Dropping scrolls");

        // Activate all ScrollJutsuPair objects
        foreach (GameObject scroll in ScrollJutsuPairs)
        {
            if (scroll != null)
            {
                scroll.SetActive(true); // Activate the ScrollJutsuPair instance
            }
            else
            {
                Debug.LogWarning("A ScrollJutsuPair object is null.");
            }
        }
    }
}
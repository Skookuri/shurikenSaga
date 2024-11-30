using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDrop : MonoBehaviour
{
    private List<GameObject> ScrollJutsuPairs = new List<GameObject>(); // List of ScrollJutsuPair objects

    void Start()
    {
        // Find all GameObjects in the scene with the "ScrollJutsuPair" tag
        GameObject[] scrollJutsuObjects = GameObject.FindGameObjectsWithTag("ScrollJutsuPair");

        // Add the GameObjects to the list and deactivate them initially
        foreach (GameObject scrollJutsuObject in scrollJutsuObjects)
        {
            ScrollJutsuPairs.Add(scrollJutsuObject);
            scrollJutsuObject.SetActive(false); // Deactivate the instance initially
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
        foreach (var scroll in ScrollJutsuPairs)
        {
            scroll.SetActive(true); // Activate the ScrollJutsuPair instance
        }
    }
}
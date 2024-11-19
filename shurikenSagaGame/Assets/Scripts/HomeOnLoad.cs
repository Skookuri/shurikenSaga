using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeOnLoad : MonoBehaviour
{
    //public Transform player; // Reference to the player object
    public GameObject Dungeon2KeyDoorPair; // Door pair for Dungeon2
    public GameObject Dungeon3KeyDoorPair; // Door pair for Dungeon3

    // These values should be set externally (e.g., via a scene manager) to track which scene the player came from
    public static bool isDungeon1 = false;
    public static bool isDungeon2 = false;
    public static bool isDungeon3 = false;

    void Start()
    {
        // Handle player spawn position and disabling GameObjects based on the previous scene
        if (isDungeon1) {
            transform.position = new Vector3(-13, 28, 0);
            ResetDungeonFlags(); // Reset flags
        } else if (isDungeon2) {
            transform.position = new Vector3(0, 28, 0); // Spawn player at the specified position
            if (Dungeon2KeyDoorPair != null) {
                Dungeon2KeyDoorPair.SetActive(false); // Disable Dungeon2KeyDoorPair
            }
            ResetDungeonFlags();// Reset flags
        } else if (isDungeon3) {
            transform.position = new Vector3(15, 28, 0); // Spawn player at the specified position

            if (Dungeon2KeyDoorPair != null) {
                Dungeon2KeyDoorPair.SetActive(false);
            }
            if (Dungeon3KeyDoorPair != null) {
                Dungeon3KeyDoorPair.SetActive(false); // Disable Dungeon2KeyDoorPair and Dungeon3KeyDoorPair
            }
            ResetDungeonFlags(); // Reset flags
        }
    }

    // Utility method to reset all dungeon flags
    private void ResetDungeonFlags()
    {
        isDungeon1 = false;
        isDungeon2 = false;
        isDungeon3 = false;
    }
}
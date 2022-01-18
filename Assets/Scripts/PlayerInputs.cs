using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
   // The index of this list is the direction this key is for
    List<KeyCode> dirKeys = new List<KeyCode>() {
        KeyCode.RightArrow,
        KeyCode.UpArrow,
        KeyCode.LeftArrow,
        KeyCode.DownArrow,
    };

    KeyCode shootKey = KeyCode.Space;

    // This stores the keys in the order they are pressed in
    // at index 0 is the most recesnt key press
    List<KeyCode> dirInputKeysDown = new List<KeyCode>();

    bool shootInputKeyDown = false;


    void Update()
    {   
        // Direction Input

        // Remove Keys that are no longer pressed
        foreach (KeyCode key in dirKeys)
        {
            if (Input.GetKey(key) == false)
            {
                while (dirInputKeysDown.Contains(key))
                {
                    dirInputKeysDown.Remove(key);
                }
            }
        }

        // Check for new keys
        foreach (KeyCode key in dirKeys)
        {
            if (Input.GetKeyDown(key) == true)
            {
                // Remove previous occurenses
                while (dirInputKeysDown.Contains(key))
                {
                    dirInputKeysDown.Remove(key);
                }

                // Add new key at beginning
                dirInputKeysDown.Insert(0, key);
            }

        }

        // Shoot Input
        shootInputKeyDown = Input.GetKey(shootKey);
    }

    // Getters
    public int GetDirInput()
    {
        if (dirInputKeysDown.Count == 0)
        {
            return -1;
        } else {
            return dirKeys.IndexOf(dirInputKeysDown[0]);
        }
    }

    public bool GetShootInput()
    {
        return shootInputKeyDown;
    }
}

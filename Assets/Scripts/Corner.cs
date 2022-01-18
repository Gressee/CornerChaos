using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Corner : NetworkBehaviour
{

    // TODO I think its possible that this hole class doesnt need networking

    NetworkList<int> possibleDirs;

    void Awake()
    {
        possibleDirs = new NetworkList<int>();
    }

    // Getters
    public List<int> GetPossibleDirs()
    {
        List<int> l = new List<int>();
        foreach (int d in possibleDirs)
        {
            l.Add(d);
        }
        return l;
    }

    // Setters
    public void SetPossibleDirs(List<int> possibleDirections)
    {
        if (NetworkManager.Singleton.IsHost) {
            foreach (int d in possibleDirections)
            {
                possibleDirs.Add(d);
            }
        }
    }

}

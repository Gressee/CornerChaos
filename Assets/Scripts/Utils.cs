using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public int ClampDir(int dir) {
        // Special case -1 means no dir or no input dir
        // so return just that
        if (dir == -1)
        {
            return dir;
        }
        
        if (dir < 0) {
            dir += 4;
        } else if (dir > 3) {
            dir -= 4;
        }
        return dir;
    }

    public bool IsOppositeDir(int dir1, int dir2) {
        // Special case if one is -1, which means its no input 
        // So it cant be opposite
        if (dir1 == -1 || dir2 == -1) return false;

        dir1 = ClampDir(dir1 + 2);
        return (dir1 == dir2);
    }
}

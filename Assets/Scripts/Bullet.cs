using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{   
    NetworkVariable<Vector3> pos = new NetworkVariable<Vector3>();
    NetworkVariable<int> dir = new NetworkVariable<int>();
    NetworkVariable<float> speed = new NetworkVariable<float>(20.0f);

    // Game object that instenistaed this bullet
    // ONLY READ/SET SERVERSIDE
    GameObject shooterPlayer;

    // Update is called once per frame
    void Update()
    {
        // Server side
        if (IsHost || IsServer) 
        {
            MoveStep();
            CheckDestroy();
        }

        // Update new position from network
        transform.position = pos.Value;

    }

    // Only call this on server side
    void MoveStep()
    {
        Vector3 dirVec = new Vector3(0,0,0);
        if (dir.Value == 0) dirVec = new Vector3(1,0,0);
        else if (dir.Value == 1) dirVec = new Vector3(0,0,1);
        else if (dir.Value == 2) dirVec = new Vector3(-1,0,0);
        else if (dir.Value == 3) dirVec = new Vector3(0,0,-1);

        pos.Value = transform.position + dirVec * speed.Value * Time.deltaTime;
        transform.position = pos.Value;
    }

    // Only call this on server side
    void CheckDestroy()
    {
        if (Mathf.Abs(pos.Value.x) > 25.0f || Mathf.Abs(pos.Value.z) > 25.0f)
        {
            // This destroys the game object this script belongs to
            // If this gets destroyed server side it gets automatically destroyed on all clients
            Destroy(gameObject);
        }
    }

    public void SetDir(int dir)
    {
        if (IsServer) this.dir.Value = dir;
    }

    public int GetDir()
    {
        return dir.Value;
    }

    public void SetShooterPlayer(GameObject playerObject)
    {
        if (IsServer) shooterPlayer = playerObject;
    }

    public GameObject GetShooterPlayer()
    {
        if (IsServer) return shooterPlayer;
        return null;
    }
}

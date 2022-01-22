using System.Collections;
using System.Collections.Generic;
using Core.Singleton;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports;

public class GameManager : NetworkSingleton<GameManager>
{

    [SerializeField]
    public GameObject cornerPrefab;
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public GameObject bulletPrefab;

    // These list are WRONG On client side but they dont need to
    // DONT USE ON CLIENTSIDE
    List<GameObject> cornerObjects = new List<GameObject>();
    List<GameObject> playerObjects = new List<GameObject>();

    // Networking
    RelayJoinData relayJoinData;
    RelayHostData relayHostData;

    // Self explainatory
    NetworkVariable<bool> gamePaused = new NetworkVariable<bool>(true);

    void Start()
    {   
        // Define Callbacks from NetworkManager

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            if (IsServer)
            {
                Debug.Log("ServerStart");
                // Spawn Cornes
                SpawnCorner(new Vector3(-20,0,-20), new List<int>() {0,1});
                SpawnCorner(new Vector3(20,0,-20), new List<int>() {1,2});
                SpawnCorner(new Vector3(20,0,20), new List<int>() {2,3});
                SpawnCorner(new Vector3(-20,0,20), new List<int>() {0,3});
                SpawnCorner(new Vector3(0,0,-20), new List<int>() {0,1,2});
                SpawnCorner(new Vector3(20,0,0), new List<int>() {1,2,3});
                SpawnCorner(new Vector3(0,0,20), new List<int>() {0,2,3});
                SpawnCorner(new Vector3(-20,0,0), new List<int>() {0,1,3});
                SpawnCorner(new Vector3(0,0,0), new List<int>() {0,1,2,3});

                SpawnPlayer(new Vector3(-20,0,-20), 0, false);
                SpawnPlayer(new Vector3(20,0,-20), 1, false);
                SpawnPlayer(new Vector3(20,0,20), 2, false);
                SpawnPlayer(new Vector3(-20,0,20), 3, false);

                // When starting a host, before this method is called, the 'OnClientConnectedCallback
                // method is called so there are no players spawned and the client on the server side has
                // no controll over a player
                // This gived controll to the first player to the serverside client
                if (!playerObjects[0].GetComponent<Player>().GetPlayerControlled())
                {
                    // Chnaging the ownership isnt necessary bc the owner is already the serverside client
                    playerObjects[0].GetComponent<Player>().SetPlayerControlled(true);
                }


            }
        };

        NetworkManager.Singleton.OnClientConnectedCallback += (clientID) =>
        {
            if (IsServer)
            {
                // Only do when players are already spawned
                // See comment on 'NetworkManager.Singleton.OnServerStarted'
                // on why this could happen
                if (playerObjects.Count > 0)
                {
                    // Give the player the first COM player
                    foreach (GameObject p in playerObjects)
                    {
                        Debug.Log(!p.GetComponent<Player>().GetPlayerControlled());
                        if (!p.GetComponent<Player>().GetPlayerControlled())
                        {
                            p.GetComponent<NetworkObject>().ChangeOwnership(clientID);
                            p.GetComponent<Player>().SetPlayerControlled(true);
                            break;

                        }
                    }
                }

            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientID) =>
        {
            if (IsServer)
            {
                // Rese this players Ownership and controlls
                foreach (GameObject p in playerObjects)
                {
                    if (p.GetComponent<NetworkObject>().OwnerClientId == clientID)
                    {
                        p.GetComponent<NetworkObject>().RemoveOwnership();
                        p.GetComponent<Player>().SetPlayerControlled(false);
                        break;
                    }
                }

            }

        };
    }

    //// FUNCTIONS TO START THE GAME ////
    public async void StartHost()
    {   
        if (RelayManager.Singelton.IsRelayEnabled)
        {
            relayHostData = await RelayManager.Singelton.SetupRelay();
            NetworkManager.Singleton.StartHost();
        }

    }

    public async void StartJoin(string joinCode)
    {
        if (RelayManager.Singelton.IsRelayEnabled && !string.IsNullOrEmpty(joinCode))
        {
            relayJoinData = await RelayManager.Singelton.JoinRelay(joinCode);
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.Log("Cant join maybe wrong join code");
        }
    }

    public void StartSingleplayer()
    {
        NetworkManager.Singleton.StartHost();
    }



    //// SPAWN GAME ////

    void SpawnCorner(Vector3 pos, List<int> possibleDirs)
    {
        if (IsServer) 
        {
            GameObject newCorner = Instantiate(cornerPrefab, pos, Quaternion.identity);
            newCorner.GetComponent<Corner>().SetPossibleDirs(possibleDirs);
            newCorner.GetComponent<NetworkObject>().Spawn();
            cornerObjects.Add(newCorner);
        }
    }

    void SpawnPlayer(Vector3 pos, int direction, bool playerControlled)
    {
        if (IsServer)
        {
            Quaternion rot = transform.rotation; // Just to get a basic Quaternion this values have no use case
            if (direction == 0) rot.eulerAngles = new Vector3(0,0,0);
            else if (direction == 1) rot.eulerAngles = new Vector3(0,-90,0);
            else if (direction == 2) rot.eulerAngles = new Vector3(0,-180,0);
            else if (direction == 3) rot.eulerAngles = new Vector3(0,-270,0);

            GameObject newPlayer = Instantiate(playerPrefab, pos, rot);
            newPlayer.GetComponent<Player>().SetPosition(pos);
            newPlayer.GetComponent<Player>().SetPlayerControlled(playerControlled);
            newPlayer.GetComponent<NetworkObject>().Spawn();
            playerObjects.Add(newPlayer);
        }
    }

    public void SpawnBullet(GameObject shooterPlayer, Vector3 position, int direction)
    {
        if (IsServer)
        {
            GameObject newBullet;

            Quaternion rot = transform.rotation; // Just to get a basic Quaternion this values have no use case
            if (direction == 0) rot.eulerAngles = new Vector3(0,0,0);
            else if (direction == 1) rot.eulerAngles = new Vector3(0,-90,0);
            else if (direction == 2) rot.eulerAngles = new Vector3(0,-180,0);
            else if (direction == 3) rot.eulerAngles = new Vector3(0,-270,0);

            newBullet = Instantiate(bulletPrefab, position, rot);
            newBullet.GetComponent<Bullet>().SetDir(direction);
            newBullet.GetComponent<Bullet>().SetShooterPlayer(shooterPlayer);
            newBullet.GetComponent<NetworkObject>().Spawn();
        }
    }



    public List<GameObject> GetCornerObjects()
    {
        return cornerObjects;
    }

    public string GetJoinCode()
    {
        if (!string.IsNullOrEmpty(relayHostData.JoinCode))
        {
            return relayHostData.JoinCode;
        }
        else
        {
            return "";
        }
    }

    public bool IsGamePaused()
    {
        return gamePaused.Value;
    }

    public void SetPauseStatus(bool paused)
    {
        gamePaused.Value = paused;
    }
}

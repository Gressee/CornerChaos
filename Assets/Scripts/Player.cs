using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{      
    Utils utils;
    GameManager gameManager;
    PlayerInputs input;

    // TODO Maybe not networkvariable need
    // False means a computer controlles the player
    NetworkVariable<bool> IsPlayerControlled = new NetworkVariable<bool>(false);

    NetworkVariable<bool> alive = new NetworkVariable<bool>(true);

    NetworkVariable<Vector3> pos = new NetworkVariable<Vector3>();
    NetworkVariable<float> speed = new NetworkVariable<float>(0.0f);
    const float targetSpeed = 10.0f;
    const float acceleration = 0.4f;
    const float rotSpeed = 2.0f; // This scales aslo with the current speed
    NetworkVariable<int> dir = new NetworkVariable<int>(0);
    NetworkVariable<int> dirInput = new NetworkVariable<int>(0);
    NetworkVariable<bool> shootInput = new NetworkVariable<bool>(false);
    NetworkVariable<float> shootCooldownCount = new NetworkVariable<float>(0.0f);
    NetworkVariable<float> shootCooldown = new NetworkVariable<float>(1.5f);

    bool atCorner = false;


    void Start()
    {
        utils = GetComponent<Utils>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        input = GetComponent<PlayerInputs>();
    }

    void Update()
    {
        // Do nothing when dead
        if (!alive.Value) return;

        //// SERVER STUFF ////

        if (IsHost || IsServer)
        {   
            // MOVEMENT

            // Get Inputs when Computer controlled
            if (!IsPlayerControlled.Value)
            {
                dirInput.Value = GetComputerDirInput();
                shootInput.Value = GetComputerShootInput();
            }

            // Check if at a corner
            atCorner = false;
            GameObject corner = null;
            List<int> cornerDirs = new List<int>();
            foreach (GameObject c in gameManager.GetCornerObjects())
            {
                float dist = Vector3.Distance(pos.Value, c.transform.position);
                if (dist <= Mathf.Abs(speed.Value) * Time.deltaTime/1.2f)
                {
                    atCorner = true;
                    corner = c;
                    cornerDirs = corner.GetComponent<Corner>().GetPossibleDirs();
                    break;
                }
                else
                {
                    atCorner = false;
                }
            }

            if (atCorner)
            {
                // snap to corner
                pos.Value = corner.transform.position;

                if (dirInput.Value == -1)
                {
                    // If no input try to go straight
                    if (cornerDirs.Contains(dir.Value))
                    {
                        MoveStep();
                    } else
                    {
                        // Stop if not possible
                        speed.Value = 0.0f;
                    }

                }
                else 
                {
                    // Try to go to the direction and its no the opposite dir
                    if (cornerDirs.Contains(dirInput.Value) && !utils.IsOppositeDir(dir.Value, dirInput.Value))
                    {
                        dir.Value = dirInput.Value;
                        MoveStep();
                        speed.Value *= 0.5f;  // slow down
                    }
                    else
                    {
                        // If this not possible try to go straight
                        if (cornerDirs.Contains(dir.Value))
                        {
                            MoveStep();
                        }
                        else {
                            // Stop
                            speed.Value = 0.0f;
                        }
                    }
                }   

            }
            else
            {
                MoveStep();
            }
            
            UpdateSpeed();

            // SHOOTING
            shootCooldownCount.Value -= Time.deltaTime;
            if (shootInput.Value && shootCooldownCount.Value <= 0)
            {
                shootCooldownCount.Value = shootCooldown.Value;
                speed.Value -= 20.0f;
                gameManager.SpawnBullet(gameObject, pos.Value, dir.Value);
            }
        }


        //// GENERAL STUFF ////

        // Get new inputs to server but only for the player that this client owns
        if (IsOwner)
        {
            UpdateInputsServerRpc(input.GetDirInput(), input.GetShootInput());
        }

        // Set updated position
        transform.position = pos.Value;

        // Lerp orientation to the current dir
        UpdateOrientation();
    }

    // Gets triggert on collision with player and bullet
    void OnTriggerEnter(Collider other)
    {
        // Only do stuff on server
        if (IsServer)
        {
            // TODO Find better way to get what the other object is
            if (other.gameObject.name.StartsWith("Player"))
            {
                CollisionPlayer(other);
            }
            else if (other.gameObject.name.StartsWith("Bullet"))
            {
                CollisionBullet(other);
            }
        }

    }

    // Client gives Server its inputs
    // Only execute this on a player if this client is the owner
    [ServerRpc]
    void UpdateInputsServerRpc(int dirInput, bool shootInput)
    {   
        // Only accept the input if player controlled
        if (IsPlayerControlled.Value)
        {
            this.dirInput.Value = dirInput;
            this.shootInput.Value = shootInput;
        }
    }

    // When player object is controlled by the COMPUTER
    int GetComputerDirInput()
    {
        // TODO Some AI that is a BIT more sophisticated than this
        int r = Mathf.RoundToInt(Random.Range(0, 4));
        return r;
    }

    // When player object is controlled by the COMPUTER
    bool GetComputerShootInput()
    {
        // TODO Some AI that is a BIT more sophisticated than this
        return (Random.Range(0, 100) < 1);
        
    }

    // Execution server side
    void MoveStep()
    {
        Vector3 dirVec = new Vector3(0,0,0);
        if (dir.Value == 0) dirVec = new Vector3(1,0,0);
        else if (dir.Value == 1) dirVec = new Vector3(0,0,1);
        else if (dir.Value == 2) dirVec = new Vector3(-1,0,0);
        else if (dir.Value == 3) dirVec = new Vector3(0,0,-1);

        pos.Value = pos.Value + dirVec * speed.Value * Time.deltaTime;
        transform.position = pos.Value;
    }

    // Execution server side
    void UpdateSpeed()
    {
        if (speed.Value < targetSpeed) {
            speed.Value += acceleration;
        } else if (speed.Value > targetSpeed) {
            speed.Value -= acceleration;
        }
    }

    // Execution everywhere
    void UpdateOrientation()
    {

        Quaternion targetRot = transform.rotation;

        if (dir.Value == 0) targetRot.eulerAngles = new Vector3(0,0,0);
        else if (dir.Value == 1) targetRot.eulerAngles = new Vector3(0,-90,0);
        else if (dir.Value == 2) targetRot.eulerAngles = new Vector3(0,-180,0);
        else if (dir.Value == 3) targetRot.eulerAngles = new Vector3(0,-270,0);

        Quaternion currentRot = transform.rotation;
        transform.rotation = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime * rotSpeed * speed.Value);
    }

    // Execution server side
    void CollisionPlayer(Collider other)
    {
        // TODO
        // If at corner do nothing
        if (!atCorner)
        {
            // Turn other way of not at corner
            dir.Value = utils.ClampDir(dir.Value + 2);
            MoveStep(); // Move so no collision on next frame
            speed.Value = 0.0f;
        }
    }
    
    // Execution server side
    void CollisionBullet(Collider bulletCollider)
    {
        // Chekc if this is not the player that shoot the bullet
        if (bulletCollider.gameObject.GetComponent<Bullet>().GetShooterPlayer() != gameObject)
        {
            // Kill when direction is the same aka bullet comes from the back
            if (dir.Value == bulletCollider.gameObject.GetComponent<Bullet>().GetDir())
            {
                // TODO play kill animation

                // Set position after the camera clipping plane
                pos.Value = new Vector3(2000, 2000, 2000);

                alive.Value = false;
            }   

            // Destroy the bullet
            GameObject.Destroy(bulletCollider.gameObject);

        }
    }

    public void SetPosition(Vector3 position)
    {
        if (IsServer){
            transform.position = position;
            pos.Value = position;
        }
    }

    public bool GetPlayerControlled()
    {
        return IsPlayerControlled.Value;
    }

    public void SetPlayerControlled(bool playerControlled)
    {
        if (IsServer){
            IsPlayerControlled.Value = playerControlled;
        }
    }
}
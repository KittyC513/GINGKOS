using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SummoningCircle : MonoBehaviour
{
    [SerializeField]
    private PlayerController[] players;

    private PlayerController activePlayer;

    [SerializeField]
    private float radius = 1f;
    [SerializeField]
    private Transform origin;
    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private UnityEvent onSummon;
    [SerializeField]
    private UnityEvent onExit;

    private bool summoningActive = false;


    private void Start()
    {
        players = new PlayerController[4];
    }

    private void Update()
    {
        //detect the player
        //if the player is detected read its run input, if the run input is active we want to set the player to a hold button state
        DetectPlayer();
        //if we detect the player in our circle
        for (int i = 0; i < players.Length - 1; i++)
        {
            //and just a double check that we have a player script attached to our player
            if (players[i] != null)
            {
                //if the player presses the action button (run)
                if (players[i].ReadActionButton() && !summoningActive)
                {
                    //activate summoning for this script at the player script
                    summoningActive = true;
                    players[i].OnSummoningEnter(this.gameObject);
                    activePlayer = players[i];
                }

            }
            
        }


        if (activePlayer != null && summoningActive && !activePlayer.ReadActionButton())
        {
            //if summoning is active and we let go of the action button exit the summon
            summoningActive = false;
            activePlayer.OnSummoningExit();
            onExit.Invoke();
            activePlayer = null;
        }


        //if summoning is active run functions
        if (summoningActive)
        {
            onSummon.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin.position, radius);
    }

    private void DetectPlayer()
    {
        //check a circular area for a collider with the player layermask
        Collider[] playerCollider = Physics.OverlapSphere(origin.position, radius, playerMask);

        //if we detect a player grab our player object and script for use otherwise exit the player from their summoning state if they are in it and get rid of our player reference
        if (playerCollider.Length > 0)
        {
            Debug.Log(playerCollider.Length);
            for (int i = 0; i < playerCollider.Length - 1; i++)
            {
                GameObject playerObj = playerCollider[i].gameObject;
                players[i] = playerObj.GetComponent<PlayerController>();
                
            }
        }
        else
        {
            for (int i = 0; i < players.Length - 1; i++)
            {
                if (players[i] != null)
                {
                    players[i].OnSummoningExit();
                }
                players[i] = null;
            }
        }
       
        //check if we are still colliding with target player
        //if we aren't get rid of the reference
    }
}

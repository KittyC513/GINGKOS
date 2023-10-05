using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SummoningCircle : MonoBehaviour
{
    private PlayerController player;
    [SerializeField]
    private GameObject playerGameObject;

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
        
    }

    private void Update()
    {
        //detect the player
        //if the player is detected read its run input, if the run input is active we want to set the player to a hold button state
        DetectPlayer();
        //if we detect the player in our circle
        if (DetectPlayer() != null)
        {
            //and just a double check that we have a player script attached to our player
            if (player != null)
            {
                //if the player presses the action button (run)
                if (player.ReadActionButton())
                {
                    //activate summoning for this script at the player script
                    summoningActive = true;
                    player.OnSummoningEnter(this.gameObject);
                }
                
                if (summoningActive && !player.ReadActionButton())
                {
                    //if summoning is active and we let go of the action button exit the summon
                    summoningActive = false;
                    player.OnSummoningExit();
                    onExit.Invoke();
                }
            }
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

    private GameObject DetectPlayer()
    {
        //check a circular area for a collider with the player layermask
        Collider[] playerCollider = Physics.OverlapSphere(origin.position, radius, playerMask);

        //if we detect a player grab our player object and script for use otherwise exit the player from their summoning state if they are in it and get rid of our player reference
        if (playerCollider.Length > 0)
        {
            GameObject playerObj = playerCollider[0].gameObject;
            player = playerObj.GetComponent<PlayerController>();
            return playerObj;
        }
        else
        {
            if (player != null)
            {
                player.OnSummoningExit();
            }
            player = null;
            return null;
        }
       
    }
}

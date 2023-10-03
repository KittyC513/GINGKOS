using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SummoningCircle : MonoBehaviour
{
    private PlayerController player;
    private GameObject playerGameObject;

    [SerializeField]
    private float radius = 1f;
    [SerializeField]
    private Transform origin;
    [SerializeField]
    private LayerMask playerMask;

    [SerializeField]
    private UnityEvent onSummon;

    private bool summoningActive = false;

    //need to get the player's interact button
    //the player could get the script from this object to make it a little easier however it becomes dependant on that script
    //could check the range around itself for a player and then check for the button press from here
    //difference between having the logic in this script or in the player script
    //might have it cleaner to have it here however every summoning circle will be checking around it all the time for a player

    private void Start()
    {
        
    }

    private void Update()
    {
        //detect the player
        //if the player is detected read its run input, if the run input is active we want to set the player to a hold button state

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
                    player.OnSummoningEnter();
                }
                
                if (summoningActive && !player.ReadActionButton())
                {
                    //if summoning is active and we let go of the action button exit the summon
                    summoningActive = false;
                    player.OnSummoningExit();
                }
            }
        }

        if (summoningActive)
        {
            onSummon.Invoke();
        }
    }

    private GameObject DetectPlayer()
    {
        Collider[] playerCollider = Physics.OverlapSphere(origin.position, radius, playerMask);

        if (playerCollider.Length > 0)
        {
            GameObject playerObj = playerCollider[0].GetComponent<GameObject>();
            player = playerObj.GetComponent<PlayerController>();
            return playerObj;
        }
        else
        {
            player = null;
            return null;
        }
       
    }
}

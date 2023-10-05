using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private Transform player1Respawn;
    [SerializeField]
    private Transform player2Respawn;
    [SerializeField]
    private Vector3 checkpointRange;
    [SerializeField]
    private LayerMask playerLayer;

    private void DetectPlayer()
    {
        Collider[] players = Physics.OverlapBox(transform.position, checkpointRange, Quaternion.identity, playerLayer);
    }
}

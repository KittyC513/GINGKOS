using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class TestCube : MonoBehaviour
{
    Vector2 i_movement;
    float moveSpeed = 10f;
    [SerializeField]
    private List<LayerMask> playerLayers;
    private List<PlayerInput> players = new List<PlayerInput>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(i_movement.x, 0, i_movement.y) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    private void OnMove(InputValue value)
    {
        i_movement = value.Get<Vector2>();
        Debug.Log("Moving");
    }
    private void OnMoveDown()
    {
        transform.Translate(transform.up);
        Debug.Log("Moving");
    }
    private void OnMoveUp()
    {
        transform.Translate(-transform.up);
        Debug.Log("Moving");
    }

    void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        Transform playerParent = player.transform.parent;

        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        playerParent.GetComponentInChildren<CinemachineBrain>().gameObject.layer = layerToAdd;
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;


    }
}

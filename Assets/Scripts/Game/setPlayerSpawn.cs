using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setPlayerSpawn : MonoBehaviour
{

    public GameObject player;

    void Update()
    {

        if(player == null)
        {
            player = GameObject.Find("PlayerCharacter");
            player.transform.position = transform.position;
            Destroy(gameObject);
        }
        
    }
}

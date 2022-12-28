using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class followPlayer : MonoBehaviour
{

    private static GameObject Instance;
    public GameObject player;
    // Update is called once per frame

    void Start()
    {
        player = GameObject.Find ("PlayerCharacter");
    }
    void Update()
    {
        if(player == null)
            player = GameObject.Find ("PlayerCharacter");


        transform.position = player.transform.position + new Vector3(-1, 0, -5);

        if(SceneManager.GetActiveScene().buildIndex == 1)
            Destroy(gameObject);
        
    }
}

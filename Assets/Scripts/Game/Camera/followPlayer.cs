using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{

    private static GameObject Instance;
    public GameObject player;
    // Update is called once per frame

    void Start()
    {
        player = GameObject.Find ("PlayerCharacter");
        if(Instance == null)
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {

        transform.position = player.transform.position + new Vector3(-1, 0, -5);
        
    }
}

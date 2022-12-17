using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    
    public bool isInRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    public UnityEvent enterRangeAction;
    public UnityEvent exitRangeAction;

    // Update is called once per frame
    void Update()
    {
        if(isInRange)   // If in range to interact
        {

            if(Input.GetKeyDown(interactKey)) // And interact key is pressed
            {
                interactAction.Invoke(); //Fire Event
            }
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionGameObject = collision.transform.parent.gameObject;

        if(collisionGameObject.name == "PlayerCharacter")
        {
            isInRange = true;
            enterRangeAction.Invoke(); // Fire in range event (Object Highlight)
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collisionGameObject = collision.transform.parent.gameObject;
        
        if(collisionGameObject.name == "PlayerCharacter")
        {
            isInRange = false;
            exitRangeAction.Invoke(); // Fire in range event (Object Highlight)
        }
    }
}

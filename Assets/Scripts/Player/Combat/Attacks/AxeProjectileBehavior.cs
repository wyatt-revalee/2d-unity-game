using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeProjectileBehavior : MonoBehaviour
{

    public float Speed = 20f;

    void Update()
    {
        transform.position += transform.right *Time.deltaTime * Speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}

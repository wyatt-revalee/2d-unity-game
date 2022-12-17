using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start() {
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            bool isCriticalHit = Random.Range(0, 100) < 30;
            DamagePopup.Create(Vector3.zero, 300, isCriticalHit);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{

    // Create a damage popup
    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit) {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);

        return damagePopup;
    }



    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
   
    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
   }

    public void Setup(int damageAmount, bool isCriticalHit) {
        textMesh.SetText(damageAmount.ToString());
        if(!isCriticalHit) {
            // Normal hit
            textMesh.fontSize = 4;
            textColor = new Color(1f, 0.4f, 0.4f);

        }
        else {
            // Critical Hit
            textMesh.fontSize = 6;
            textColor = new Color(1f, 0f, 0f);
        }
        textMesh.color = textColor;
        disappearTimer = 1f;
    }

    private void Update() {
        float moveYspeed = 1f;
        transform.position += new Vector3(0, moveYspeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0) {
            // Start Disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0){
                Destroy(gameObject);
            }
        }
    }

}

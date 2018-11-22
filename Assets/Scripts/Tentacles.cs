using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacles : MonoBehaviour {

    //frame array to create a collider anim that fits the tentacles
    [SerializeField]
    private PolygonCollider2D[] centerColliders;
    private int centerIndex = 0;

    [SerializeField]
    private PolygonCollider2D[] sideColliders;
    private int sideIndex = 0;

    public void setCenterCollider(int spriteNum) {
        centerColliders[centerIndex].enabled = false;
        centerIndex = spriteNum;
        centerColliders[centerIndex].enabled = true;
    }

    public void setSideCollider(int spriteNum) {
        sideColliders[sideIndex].enabled = false;
        sideIndex = spriteNum;
        sideColliders[sideIndex].enabled = true;
    }
}

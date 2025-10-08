using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public float impact;

    private Renderer r;

    private void Awake() {
        r = GetComponent<Renderer>();
    }


    private void OnCollisionEnter(Collision collision) {

        Vector3 intersectionAnchor = collision.GetContact(0).point;

        if (collision.collider.GetType() == typeof(CapsuleCollider)) {
            float capsuleRadius = ((CapsuleCollider)collision.collider).radius;     
            r.material.SetInt("_IsIntersecting", 1);
            r.material.SetFloat("_CapsuleRadius", capsuleRadius);
            r.material.SetVector("_IntersectionAnchor", intersectionAnchor);
            r.material.SetFloat("_Impact", this.impact);
        }
    }

    private void OnCollisionExit(Collision collision) {
        r.material.SetInt("_IsIntersecting", 0);
    }

}

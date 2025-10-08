using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDropMe : MonoBehaviour
{
    /*
    Whenever an object with a rigidbody hits a collider tagged "Ground" it is reset to its start position and rotation.
    The rigidbody is put to sleep to prevent it from carrying its velocity.
    This ensures that objects dont drop to or fall through the floor.
    */

    private Vector3 startPos;
    private Quaternion startRot;

    private Rigidbody rb;

    [SerializeField] private new string tag = "Dropzone";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            transform.position = startPos;
            transform.rotation = startRot;
            rb.Sleep();
            Debug.Log("reset");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tag))
        {
            startPos = transform.position;
            startRot = transform.rotation;
            Debug.Log("new drop zone");
        }
    }
}
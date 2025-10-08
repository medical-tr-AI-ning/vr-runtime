using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Logger;
using MedicalTraining.Configuration;

public class NormalGravity : MonoBehaviour
{

    private enum States
    {
        onHand,
        normalGravity,
        goingToHand,
        inAnimation
    }


    [Header("Basics")]
    public Transform rayOrigin;
    public Transform handlePosition;
    public new string name;
    [Space(10)]

    [Header("While on Hand")]
    public float rayLength = 0.1f;
    [Space(10)]

    [Header("While on Surface")]
    public float surfaceDistance = .05f;
    public float maxHandDistance = .05f;
    public float handGravityMultiplier = 30f;
    public float gravityMultiplier = 20f;
    public float maxGravityForce = 20f;
    [Space(10)]

    [Header("Going back to Hand")]
    public float mPerSecond = 3f;
    public float degreePerSecond = 500f;
    [Space(10)]

    [Header("Temp")]
    private List<RaycastHit> hits;
    private Quaternion[] rotations;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Vector3 onSurfaceOffset;        
    private SimulationLogger logger;

    //bool changedToHand = true;
    States state = States.onHand;

    void Awake()
    {
        
        logger = ConfigurationContainer.Instance.GetLogger();
        Quaternion r = transform.parent.rotation;
        transform.parent.rotation = Quaternion.Euler(0, 0, 0);
        
        originalPosition = rayOrigin.localPosition - transform.localPosition;
        originalRotation = rayOrigin.localRotation;
        originalParent = transform.parent;

        transform.parent.rotation = r;

        hits = new List<RaycastHit>();
        rotations = new Quaternion[25]{
            Quaternion.Euler(0, -2.5f, 0),
            Quaternion.Euler(2.5f, -2.5f, 0),
            Quaternion.Euler(-2.5f, -2.5f, 0),
            Quaternion.Euler(0, 2.5f, 0),
            Quaternion.Euler(2.5f, 2.5f, 0),
            Quaternion.Euler(-2.5f, 2.5f, 0),
            Quaternion.Euler(2.5f, 0, 0),
            Quaternion.Euler(-2.5f, 0, 0),
            Quaternion.Euler(0, -5f, 0),
            Quaternion.Euler(5, -5f, 0),
            Quaternion.Euler(-5f, -5f, 0),
            Quaternion.Euler(0, 5f, 0),
            Quaternion.Euler(5f, 5f, 0),
            Quaternion.Euler(-5f, 5f, 0),
            Quaternion.Euler(5f, 0, 0),
            Quaternion.Euler(-5f, 0, 0),
            Quaternion.Euler(0, -45f, 0),
            Quaternion.Euler(45f, -45f, 0),
            Quaternion.Euler(-45f, -45f, 0),
            Quaternion.Euler(0, 45f, 0),
            Quaternion.Euler(45f, 45f, 0),
            Quaternion.Euler(-45f, 45f, 0),
            Quaternion.Euler(45f, 0, 0),
            Quaternion.Euler(-45f, 0, 0),
            Quaternion.Euler(0, 0, 0)
        };
    }

    void FixedUpdate()
    {
        //Debug.Log(state);
        fireRays(hits, rotations, rayOrigin);

        Vector3 average = new Vector3(0, 0, 0);
        float hitDistance = 0f;
        Vector3 averagePoint = new Vector3(0, 0, 0);

        foreach (RaycastHit hit in hits)
        {
            average += hit.normal;
            hitDistance += (hit.point - rayOrigin.position).magnitude;
            averagePoint += hit.point;
        }

        average = average / hits.Count;
        average = Vector3.Normalize(average);
        hitDistance = hitDistance / hits.Count;
        averagePoint = averagePoint / hits.Count;

        averagePoint = averagePoint + average * surfaceDistance;

        //Debug.DrawRay(rayOrigin.position, average);

        //Debug.DrawRay(rayOrigin.position, rayOrigin.forward);

        float handDistance = (rayOrigin.position - (transform.position + onSurfaceOffset)).magnitude;

        if (state == States.onHand && !float.IsNaN(hitDistance))
        {
            state = States.normalGravity;
            handlePosition.SetParent(originalParent);
            rayOrigin.GetComponent<Rigidbody>().isKinematic = false;
            onSurfaceOffset = rayOrigin.position - transform.position;
            if (logger != null)
            {
                logger.WriteEvent(name, "attached onto skin", "");
            }
        }
        else if (state == States.normalGravity && (handDistance >= maxHandDistance || float.IsNaN(hitDistance)))
        {
            state = States.goingToHand;
            if (logger != null)
            {
                logger.WriteEvent(name, "detached from skin", "");
            }
        }


        switch (state)
        {
            case States.normalGravity:
                addTorque(average, hitDistance, rayOrigin.GetComponent<Rigidbody>());
                addGravityTowardsPointHandGravityOnPlane(averagePoint, average, hitDistance, rayOrigin.GetComponent<Rigidbody>());
                break;
            case States.goingToHand:
                StartCoroutine("moveToHand");
                state = States.inAnimation;
                break;
            case States.onHand:
            
                rayOrigin.GetComponent<Rigidbody>().isKinematic = true;
                handlePosition.SetParent(transform);
                rayOrigin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |
                                                          RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                
                break;
            default:
                break;
        }
    }

    IEnumerator moveToHand()
    {
    rayOrigin.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        rayOrigin.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        rayOrigin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |
                                                          RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;


        handlePosition.position = rayOrigin.position - originalPosition;
        rayOrigin.position = handlePosition.position + handlePosition.rotation * originalPosition;
        rayOrigin.localRotation = originalRotation;

        while ((rayOrigin.position - (transform.position + handlePosition.rotation * originalPosition)).magnitude > .005f)
        {
            handlePosition.position = Vector3.MoveTowards(handlePosition.position, transform.position, Time.deltaTime * mPerSecond);
            handlePosition.rotation = Quaternion.RotateTowards(handlePosition.rotation, transform.rotation, Time.deltaTime * degreePerSecond);
            yield return 0;
        }

        // reset Position and Rotation to make sure everything is in Place
        rayOrigin.localRotation = originalRotation;
        handlePosition.position = transform.position;
        handlePosition.rotation = transform.rotation;
        rayOrigin.position = handlePosition.position + handlePosition.rotation * originalPosition;
        state = States.onHand;
        yield break;
    }

    IEnumerator moveToHandPhysics()
    {
        rayOrigin.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        rayOrigin.localRotation = originalRotation;
        rayOrigin.position = handlePosition.position + originalPosition;

        while ((rayOrigin.position - (transform.position + handlePosition.rotation * originalPosition)).magnitude > .005f)
        {
            Vector3 handGravity = (rayOrigin.position - (transform.position + handlePosition.rotation * originalPosition)) * (-1) * handGravityMultiplier;
            rayOrigin.GetComponent<Rigidbody>().AddForce(handGravity);
            yield return 0;
        }

        rayOrigin.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        rayOrigin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |
                                                            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;


        // reset Position and Rotation to make sure everything is in Place
        rayOrigin.localRotation = originalRotation;
        handlePosition.position = transform.position;
        handlePosition.rotation = transform.rotation;
        rayOrigin.position = handlePosition.position + handlePosition.rotation * originalPosition;
        rayOrigin.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        rayOrigin.GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
        state = States.onHand;
        yield break;
    }

    void fireRays(List<RaycastHit> hits, Quaternion[] rotations, Transform origin)
    {
        hits.Clear();
        int layerMask = 1 << LayerMask.NameToLayer("Agent");
        //layerMask = ~layerMask;

        foreach (Quaternion q in rotations)
        {
            Ray ray = new Ray(origin.position, q * (origin.forward));
            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData, rayLength, layerMask))
            {
                hits.Add(hitData);
            }
            Debug.DrawRay(origin.position, q * (origin.forward) * rayLength);
        }
    }

    void addNormalGravity(Vector3 average, float normalDistance, Rigidbody recipient)
    {
        Vector3 handGravity = (rayOrigin.position - (transform.position + handlePosition.rotation * originalPosition)) * (-1) * handGravityMultiplier;
        Vector3 normalGravity = -average * Mathf.Min((1 / normalDistance) * gravityMultiplier, maxGravityForce);
        recipient.AddForce(normalGravity + handGravity);
    }

    void addGravityTowardsPointHandGravityOnPlane(Vector3 point, Vector3 average, float normalDistance, Rigidbody recipient)
    {
        //Vector3 handGravity = (rayOrigin.position - (transform.position + handlePosition.rotation * originalPosition)) * (-1) * handGravityMultiplier * 0.05f;
        Vector3 pointGravity = (point - rayOrigin.position) * Mathf.Min((1 / normalDistance) * gravityMultiplier, maxGravityForce);

        Plane p = new Plane(-average, point);
        Vector3 pointOnPlane = p.ClosestPointOnPlane(transform.position + onSurfaceOffset);
        Vector3 handGravity = (pointOnPlane - point) * handGravityMultiplier;
        Debug.DrawLine(pointOnPlane, transform.position + onSurfaceOffset, Color.red, 0.5f, true);

        /*if((pointOnPlane - (transform.position + onSurfaceOffset)).magnitude < lesionMaxDistance && activeLesionGravity){
            recipient.AddForce(pointGravity + LesionGravity);
            Debug.DrawLine(pointOnPlane, nextLesion, Color.cyan, 0.0f, true);
        } else {
            recipient.AddForce(pointGravity + handGravity);
        }*/
        recipient.AddForce(pointGravity + handGravity);

    }

    void addTorque(Vector3 average, float normalDistance, Rigidbody recipient)
    {
        //https://stackoverflow.com/questions/58419942/stabilize-hovercraft-rigidbody-upright-using-torque/58420316#58420316
        Quaternion deltaQuat = Quaternion.FromToRotation(rayOrigin.forward, -average);
        Vector3 axis;
        float angle;
        deltaQuat.ToAngleAxis(out angle, out axis);


        //Vector3 normalTorque = Vector3.Cross(-average, Vector3.forward);
        //Vector3 handTorque = Vector3.Cross(-transform.up, vectorForward);
        //recipient.AddTorque(normalTorque + handTorque);
        recipient.constraints = RigidbodyConstraints.None;
        //recipient.AddTorque(normalTorque);
        //rayOrigin.rotation = Quaternion.LookRotation(-average, Vector3.up);
        float dampeningFactor = 0.8f; // this value requires tuning
        recipient.AddTorque(-recipient.angularVelocity * dampeningFactor, ForceMode.Acceleration);
        float adjustFactor = 0.5f; // this value requires tuning
        recipient.AddTorque(axis.normalized * angle * adjustFactor, ForceMode.Acceleration);
    }

    void OnEnable() {
        handlePosition.gameObject.SetActive(true);
        rayOrigin.localRotation = originalRotation;
        handlePosition.position = transform.position;
        handlePosition.rotation = transform.rotation;
        rayOrigin.position = handlePosition.position + handlePosition.rotation * originalPosition;
        state = States.onHand;
    }

    void OnDisable() {
        handlePosition.gameObject.SetActive(false);
    }
}

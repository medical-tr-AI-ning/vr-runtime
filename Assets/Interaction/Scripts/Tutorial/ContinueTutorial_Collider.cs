using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ContinueTutorial_Collider : MonoBehaviour
{

    [SerializeField] private string collisionTag;
    /*
    [SerializeField] private GameObject[] disableObjects;
    [SerializeField] private GameObject[] enableObjects;
    */
    public UnityEvent continueTutorial;


    private bool played = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(collisionTag) && !played)
        {
            /*
            foreach(GameObject obj in disableObjects)
            {
                obj.SetActive(false);
            }

            foreach(GameObject obj in disableObjects)
            {
                obj.SetActive(true);
            }
            */
            continueTutorial.Invoke();
            played = true;
        }
    }


}

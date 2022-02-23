using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardVisionCone : MonoBehaviour
{
    public GameObject Guard;
    GuardAI guardScript;
    [HideInInspector] public bool isInVision;
    // Start is called before the first frame update
    void Start()
    {
        guardScript = Guard.GetComponent<GuardAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInVision = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            guardScript.LastKnownPlayerPos = other.transform.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInVision = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject LookDirection;
    public bool IsIdleNode = false;
    [HideInInspector] public Vector3 PointToLookAt;
    // Start is called before the first frame update
    void Start()
    {
        PointToLookAt = LookDirection.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

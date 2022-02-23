using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//States Key
//0 = idle
//stare in a direction
//1 = patrol
//go from point to point in loop at certain points go into idle for x seconds
//2 = investigate
//Looks around for player and checks last seen props if present
//3 = chase
//Goes to point then enters investigate or attack if player is seen
//4 = attack
//shoot at the player
public class GuardAI : MonoBehaviour
{
    private GameObject player;
    [HideInInspector] public Vector3 LastKnownPlayerPos;
    private float DetectionTime = 3f;
    private float CurrentDetection;
    [HideInInspector] public bool isInVision = false;
    int State = 0;
    public float attackRadius;
    public float peripheralDetectionDivider = 1;
    public float mainDetectionDivider = 1;
    public float unDetectionDivider = 1;
    public GuardVisionCone PeripheralVisionCone;
    public GuardVisionCone MainVisionCone;
    public GameObject[] Nodes;
    int CurrentNodeNumber = 0;
    bool playerWasProp;
    [HideInInspector] public int LastSeenPropID = 0; // 0 = null prop
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<movement>().gameObject;
        LastKnownPlayerPos = this.transform.position;
        CurrentDetection = DetectionTime;
        Nodes[0].GetComponent<Node>().IsIdleNode = false; //idiot protection
    }
    void MoveToPoint(Vector3 posToMove)
    { 
        //make later lmao
        //move to point return when there
    }
    void LookAtPoint(Vector3 pointToLookAt)
    {
        //make later lmao
        //face direction of a vector3
    }
    void ShootAtPoint(Vector3 pointToShootAt)
    {
        //make later lmao
        //make shoot at point
    }
    void Update()
    {
        if (DetectionTime <= 0)
        {
            State = 4;//set to attack
        }
        //check peripheral vision
        if (PeripheralVisionCone.isInVision == true && MainVisionCone.isInVision == false)
        {
            CurrentDetection -= Time.deltaTime / peripheralDetectionDivider;
        }
        //check main vision
        else if (MainVisionCone.isInVision == true)
        {
            CurrentDetection -= Time.deltaTime / mainDetectionDivider;
        }
        //reset CurrentDetection slowly
        else
        {
            if (CurrentDetection < DetectionTime)
            {
                CurrentDetection += Time.deltaTime / unDetectionDivider;
            }
        }
        switch (State)
        {
            case 0://idle in patrol
                LookAtPoint(Nodes[CurrentNodeNumber - 1].GetComponent<Node>().PointToLookAt);//do not set node 0 to be idle
                //wait for x seconds
                State = 1;//go back to patrol
                break;
            case 1://patrol
                MoveToPoint(Nodes[CurrentNodeNumber].transform.position);
                if (Nodes[CurrentNodeNumber].GetComponent<Node>().IsIdleNode == true)
                {
                    if (CurrentNodeNumber + 1 > Nodes.Length)
                    {
                        CurrentNodeNumber += 1;
                        State = 0;
                    }
                }
                break;
            case 2://investigate
                //looks back and forth in a x degree area
                if (PeripheralVisionCone.isInVision == true || MainVisionCone.isInVision == true) //check for player in view
                {
                    State = 4;//set state to attack
                }
                //if player was a prop and prop is spotted attack prop
                if (playerWasProp == true)
                { 
                
                }
                //wait for x time

                //if nothing found return to patrol
                State = 1;
                break;
            case 3://chase
                MoveToPoint(LastKnownPlayerPos);
                if (PeripheralVisionCone.isInVision == true || MainVisionCone.isInVision == true) // check for player in view
                {
                    State = 4;//set state to attack
                }
                else
                    State = 2;// go to investigate if player isnt found
                break;
            case 4://attack
                LastKnownPlayerPos = player.transform.position;
                RaycastHit hit = new RaycastHit();
                Vector3 direction = player.transform.position - transform.position;
                Physics.Raycast(transform.position, direction, out hit);
                //attack if in certain radius
                if (Vector3.Distance(player.transform.position, transform.position) < attackRadius)
                    ShootAtPoint(LastKnownPlayerPos);
                else if (hit.collider.gameObject.tag != "Player")
                {
                    State = 3;//go to chase state because player is behind object
                }

            break;
        }
    }    
}

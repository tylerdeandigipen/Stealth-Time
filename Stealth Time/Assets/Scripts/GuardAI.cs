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
    [Tooltip("The points the guard will travel to")] public GameObject[] Nodes;
    int CurrentNodeNumber = 0;
    bool playerWasProp;
    [HideInInspector] public int LastSeenPropID = 0; // 0 = null prop
    [HideInInspector] public bool propSpotted;
    [HideInInspector] public GameObject prop;
    float attackDelay;
    [Tooltip("How accurate first shot is")] public float FirstAttackAccuracy;
    float attackAccuracy = .5f;
    [Tooltip("How long it takes to reset accuracy in seconds")] public float accuracyResetTime = 5;
    [Tooltip("How much more accurate each shot is(between 1-0, 0 is more accurate)")] public float accuracyGain;
    float timeSinceLastShot;
    float wait;
    [Tooltip("How long they will search in investigate state")] public float timeToWaitInvestigate;
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
        //move to point return when there
    }
    void LookAtPoint(Vector3 pointToLookAt)
    {
        //face direction of a vector3
    }
    void ShootAtPoint(Vector3 pointToShootAt, float accuracy)
    {
        attackAccuracy = accuracy;
        Random.Range(0, attackAccuracy);//have random number generated between 0 and attackAccuracy (use to modulate projectile trajectory)
        //spawn projectile and add the random number to the velociy on x or y or both

        //make each succesive shot more accurate
        if (attackAccuracy - accuracyGain < 0)
            attackAccuracy = 0;
        else
            attackAccuracy -= accuracyGain;
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
            LastKnownPlayerPos = player.transform.position;
            CurrentDetection -= Time.deltaTime / peripheralDetectionDivider;
        }
        //check main vision
        else if (MainVisionCone.isInVision == true)
        {
            LastKnownPlayerPos = player.transform.position;
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
        //make time since last shot go up slowly and reset attackaccuracy
        if (timeSinceLastShot < attackDelay)
        {
            timeSinceLastShot += Time.deltaTime;
            if (timeSinceLastShot > accuracyResetTime)
            {
                attackAccuracy = FirstAttackAccuracy;
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
                //looks back and forth in a x degree area (use a sine wave?)

                //if player was a prop and prop is spotted attack prop
                if (playerWasProp == true && propSpotted)
                {
                    ShootAtPoint(prop.transform.position, 0f);
                }
                //wait for x time
                if (wait > timeToWaitInvestigate)
                {
                    //if nothing found return to patrol
                    State = 1;
                }
                else
                    wait += Time.deltaTime;
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
                if (hit.collider.gameObject.tag != "Player")
                {
                    State = 3;//go to chase state because player is behind object
                }
                //attack if in certain radius
                else if (Vector3.Distance(player.transform.position, transform.position) < attackRadius && timeSinceLastShot >= attackDelay)
                {
                    ShootAtPoint(LastKnownPlayerPos, attackAccuracy);                    
                }
                break;
        }
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMyVision : MonoBehaviour
{
    //How sensitive we are about vision/line of sight?
    public enum enmSensitivity { HIGH, LOW};

    // Variable to check sensitivty 
    public enmSensitivity sensitity = enmSensitivity.HIGH;

    //Are we able to see the target right now?
    public bool targetInSight = false;

    //Field of Vision
    public float fieldofVision = 45f;

    // we need a refernce ot our target here as well
    private Transform target = null;

    //Reference to our eyes
    public Transform myEyes = null;
    
    //My transform componet ?
    public Transform npcTransform = null;
    
    // My sphere collider 
    private SphereCollider sphereCollider = null;

    // Last known sighting of object
    public Vector3 lastknownSighting = Vector3.zero;

    private void Awake()
    {
        npcTransform = GetComponent<Transform>();
        sphereCollider = GetComponent<SphereCollider>();
        lastknownSighting = npcTransform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); // Okay we shall tag this later
    }

    bool InMyFieldOfVision()
    {
        Vector3 dirToTarget = target.position - myEyes.position;

        // Get angle between forward and view direction

        float angle = Vector3.Angle(myEyes.forward, dirToTarget);

        // Let us check if within field of view
        if (angle <= fieldofVision)
            return true;
        else
            return false;

    }

    // We need a function to check line of sight
    bool ClearLineofSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(myEyes.position, (target.position - myEyes.position).normalized,
            out hit, sphereCollider.radius))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
            
                
        }
        return false;
    }

    void UpdateSight()
    {
        switch (sensitity)
        {
            case enmSensitivity.HIGH:
                targetInSight = InMyFieldOfVision() && ClearLineofSight();
                break;
            case enmSensitivity.LOW:
                targetInSight = InMyFieldOfVision() && ClearLineofSight();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateSight();
        //Update last known sight
        if (targetInSight)
            lastknownSighting = target.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        targetInSight = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

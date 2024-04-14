using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject leftHandTarget;
    public GameObject rightHandTarget;
    public GameObject leftFootTarget;
    public GameObject rightFootTarget;
    public GameObject hips;

    public Rigidbody rb;

    private FixedJoint leftHandJoint, rightHandJoint;
    public GameObject leftHand, rightHand, rightFoot, leftFoot;

    public Animator animator;

    public float grabDistance = 0.2f; // Maximum distance hands can reach to grab
    public float grabSpeed = 3.0f; // Speed at which hands and feet move to grab object

    private bool isLeftHandGrabbing = false;
    private bool isRightHandGrabbing = false;
    private bool isLeftFootGrabbing = false;
    private bool isRightFootGrabbing = false;

    private bool isHanging = false;

    private AudioSource audioListener;

    private bool isRagdoll = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioListener = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckForInput();
        TrackMouseMovement();
        audioListener = GetComponent<AudioSource>();

        if ((isLeftHandGrabbing || isRightHandGrabbing) && (!isRightFootGrabbing || !isLeftFootGrabbing))
        {
            //ToggleRagdoll(true);
            rb.isKinematic = false;
            animator.SetBool("isHanging", true);
        }
        else 
        {
            animator.SetBool("isHanging", false);
        }

    }

    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("pull");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (TryGrab(leftFoot, "Left Foot"))
            {
                isLeftFootGrabbing = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (TryGrab(rightFoot, "Right Foot"))
            {
                isRightFootGrabbing = true;
            }
        }
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            if (TryGrab(leftHand, "Left Hand"))
            {
                isLeftHandGrabbing = true;
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            if (TryGrab(rightHand, "Right Hand"))
            {
                isRightHandGrabbing = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isLeftHandGrabbing = false;
        }

        if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            isRightHandGrabbing = false;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            isLeftFootGrabbing = false;
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            isRightFootGrabbing = false;
        }
    }

    void TrackMouseMovement()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, Camera.main.WorldToScreenPoint(leftHandTarget.transform.position).z));
        
        hips.GetComponent<Rigidbody>().AddForce(new Vector3(newPosition.x, newPosition.y, newPosition.z) * 10);
        
        
        if (!isLeftHandGrabbing)
        {
            leftHandTarget.transform.position = newPosition;
        }

        if (!isRightHandGrabbing)
        {
            rightHandTarget.transform.position = newPosition;
        }

        if (!isLeftFootGrabbing && isRightFootGrabbing && (isRightHandGrabbing || isLeftHandGrabbing))
        {
            leftFootTarget.transform.position = newPosition;
        }

        if (!isRightFootGrabbing && isLeftFootGrabbing && (isRightHandGrabbing || isLeftHandGrabbing))
        {
            rightFootTarget.transform.position = newPosition;
        }
    }

    bool TryGrab(GameObject hand, string handName)
    {
        GameObject target = GetClosestObject(hand);

        if (target != null)
        {
            Debug.Log(handName + " hand is grabbing: " + target.name);
            hand.transform.position = Vector3.MoveTowards(hand.transform.position, target.transform.position,
                grabSpeed * Time.deltaTime);
            audioListener.Play();
            
            GrabHold(hand, target);
            return true;
        }

        return false;
    }

    void GrabHold(GameObject hand, GameObject hold)
    {
        hand.transform.position = hold.transform.position;
    }

    GameObject GetClosestObject(GameObject hand)
    {
        float minDistance = float.MaxValue;
        GameObject closestObject = null;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Hold");

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(hand.transform.position, obj.transform.position);
            if (distance < minDistance && distance <= grabDistance)
            {
                closestObject = obj;
                minDistance = distance;
            }
        }

        return closestObject;
    }

    public void ToggleRagdoll(bool state)
    {
        isRagdoll = state;
        animator.enabled = !state; // Disable the animator when the ragdoll is active

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !state; // Turn off kinematic to enable ragdoll physics
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = state; // Ensure colliders are enabled for ragdoll
        }
    }
}

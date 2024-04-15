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

    public float standUpSpeed = 3.0f; // Speed at which player stands up
    public float standUpDistance = 3f; // Maximum distance player can reach to stand up

    private bool isLeftHandGrabbing = false;
    private bool isRightHandGrabbing = false;
    private bool isLeftFootGrabbing = false;
    private bool isRightFootGrabbing = false;

    private bool isHanging = false;

    private AudioSource audioListener;


    void Start()
    { 
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
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
            // rb.isKinematic = false;
            animator.SetBool("isHanging", true);
        }
        else
        {
            animator.SetBool("isHanging", false);
        }
    }

    void StandUp()
    {
        Debug.Log("Standing up");
        Vector3 position = hips.transform.position;
        Vector3 targetPosition = new Vector3(position.x, position.y + standUpDistance, position.z);

        if (hips.GetComponent<Rigidbody>() != null && !hips.GetComponent<Rigidbody>().isKinematic)
        {
            Rigidbody rb = hips.GetComponent<Rigidbody>();
            rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, standUpSpeed * Time.deltaTime));
        }
        else
        {
            hips.transform.position = Vector3.MoveTowards(position, targetPosition, standUpSpeed * Time.deltaTime);
        }
    }

    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StandUp();
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


        // Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        //
        // // Adjust mouse position so that the screen center is (0, 0)
        // Vector2 adjustedMousePosition =
        //     new Vector2(Input.mousePosition.x - screenCenter.x, Input.mousePosition.y - screenCenter.y);
        //
        // Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(leftHandTarget.transform.position).z));


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
}
using UnityEngine;
using System.Collections.Generic;

public class MoveTest : MonoBehaviour
{
    [SerializeField] private GameObject gripper_system;
    private Rigidbody gripper_systemRB;
    [SerializeField] private GameObject conveyor;
    [SerializeField] private GameObject barrel1;
    private Rigidbody barrel1RB;
    private Barrel barrel1Collision;
    [SerializeField] private GameObject barrel2;
    private Rigidbody barrel2RB;
    private Barrel barrel2Collision;
    [SerializeField] private GameObject gripper;
    private Rigidbody gripperRB;
    [SerializeField] private GameObject clawLeft;
    private Claw clawLeftTrigger;
    [SerializeField] private GameObject clawRight;
    private Claw clawRightTrigger;
    [SerializeField] private GameObject handleLeft;
    private Handle handleLeftTrigger_l;
    private Handle handleLeftTrigger_r;
    [SerializeField] private GameObject handleRight;
    private Handle handleRightTrigger_l;
    private Handle handleRightTrigger_r;

    [SerializeField] private float conveyorSpeed = 0.7f;
    [SerializeField] private float gripperVerticalSpeed = 0.5f;
    [SerializeField] private float gripperHorizontalSpeed = 0.1f;
    [SerializeField] private float clawSpeed = 0.05f;
    [SerializeField] private float handleRotationSpeed = 3f;
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private bool pickup = false;
    private FixedJoint gripJoint;
    [SerializeField] private bool enableBarrel2 = false;

    void Start()
    {
        gripper_systemRB = gripper_system.GetComponent<Rigidbody>();
        if (gripper_systemRB == null)
        {
            Debug.LogError("Gripper System Rigidbody not found!");
        }
        gripperRB = gripper.GetComponent<Rigidbody>();
        if (gripperRB == null)
        {
            Debug.LogError("Gripper Rigidbody not found!");
        }
        barrel1RB = barrel1.GetComponent<Rigidbody>();
        if (barrel1RB == null)
        {
            Debug.LogError("Barrel1 Rigidbody not found!");
        }
        barrel2RB = barrel2.GetComponent<Rigidbody>();
        if (barrel2RB == null)
        {
            Debug.LogError("Barrel2 Rigidbody not found!");
        }
        clawLeftTrigger = clawLeft.transform.GetComponentInChildren<Claw>();
        if (clawLeftTrigger == null)
        {
            Debug.LogError("Claw Left Trigger not found!");
        }
        clawRightTrigger = clawRight.transform.GetComponentInChildren<Claw>();
        if (clawRightTrigger == null)
        {
            Debug.LogError("Claw Right Trigger not found!");
        }
        barrel1Collision = barrel1.GetComponent<Barrel>();
        if (barrel1Collision == null)
        {
            Debug.LogError("Barrel1 Collision not found!");
        }
        barrel2Collision = barrel2.GetComponent<Barrel>();
        if (barrel2Collision == null)
        {
            Debug.LogError("Barrel2 Collision not found!");
        }
        handleLeftTrigger_l = handleLeft.transform.GetChild(0).GetComponent<Handle>();
        if (handleLeftTrigger_l == null)
        {
            Debug.LogError("Handle Left Trigger not found!");
        }
        handleLeftTrigger_r = handleLeft.transform.GetChild(1).GetComponent<Handle>();
        if (handleLeftTrigger_r == null)
        {
            Debug.LogError("Handle Left Trigger not found!");
        }
        handleRightTrigger_l = handleRight.transform.GetChild(0).GetComponent<Handle>();
        if (handleRightTrigger_l == null)
        {
            Debug.LogError("Handle Right Trigger not found!");
        }
        handleRightTrigger_r = handleRight.transform.GetChild(1).GetComponent<Handle>();
        if (handleRightTrigger_r == null)
        {
            Debug.LogError("Handle Right Trigger not found!");
        }
        pickup = false;
        enableBarrel2 = false;
        barrel2.GetComponent<Collider>().enabled = false; // Disable barrel2 collider initially
        barrel2.GetComponent<MeshRenderer>().enabled = false; // Disable barrel2 mesh renderer initially
    }
    void Update()
    {
        // Manually enable barrel2 with space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enableBarrel2 = true;
            barrel2.GetComponent<Collider>().enabled = true; // Enable barrel2 collider
            barrel2.GetComponent<MeshRenderer>().enabled = true; // Enable barrel2 mesh renderer
            Debug.Log($"[Barrel2] Enabled: {enableBarrel2}");
        }
    }
    void FixedUpdate()
    {
        // Receive Hokuyo data
        /*
        FrameData data = OSCReceiver.Instance.GetLatestFrame();
        if (data != null)
        {
            foreach (var entity in data.Entities)
            {
                Debug.Log($"Entity {entity.ID}: X={entity.X}, Y={entity.Y}");
            }
        }
        */
        // Initialize
        gripper_systemRB.linearVelocity = Vector3.zero;
        if (!pickup && !barrel1Collision.isGrounded) // Consider gravity
        {
            barrel1RB.linearVelocity = new Vector3(0, barrel1RB.linearVelocity.y - gravity * Time.deltaTime, 0);
        }
        else
        {
            barrel1RB.linearVelocity = Vector3.zero;
        }
        if (!pickup && !barrel2Collision.isGrounded && enableBarrel2) // Consider gravity
        {
            barrel2RB.linearVelocity = new Vector3(0, barrel2RB.linearVelocity.y - gravity * Time.deltaTime, 0);
        }
        else
        {
            barrel2RB.linearVelocity = Vector3.zero;
        }
        gripperRB.linearVelocity = Vector3.zero;
        // Gripper system movement (x)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gripper_systemRB.linearVelocity += new Vector3(-gripperHorizontalSpeed, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            gripper_systemRB.linearVelocity += new Vector3(gripperHorizontalSpeed, 0, 0);
        }
        // Gripper movement (y)
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gripperRB.linearVelocity += new Vector3(0, -gripperVerticalSpeed, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            if (gripper.transform.localPosition.z <= -0.01599986f)
            {
                gripperRB.linearVelocity += new Vector3(0, gripperVerticalSpeed, 0);
            }
        }

        // Claw movement
        if (clawLeftTrigger.touched && clawRightTrigger.touched && clawLeftTrigger.contact == clawRightTrigger.contact)
        {
            pickup = true;
            if (gripJoint == null)
            {
                gripJoint = gripper.AddComponent<FixedJoint>();
                gripJoint.connectedBody = clawLeftTrigger.contact.GetComponent<Rigidbody>();
                Debug.Log("Grip joint created and connected to barrel1.");
            }
        }
        else
        {
            pickup = false;
            if (gripJoint != null)
            {
                Destroy(gripJoint);
                gripJoint = null;
                Debug.Log("Grip joint destroyed.");
            }
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (!pickup && clawLeft.transform.localPosition.x >= 0.0013f)
            {
                Debug.Log("Moving claw left");
                clawLeft.transform.Translate(Vector3.left * clawSpeed * Time.deltaTime);
                clawRight.transform.Translate(Vector3.right * clawSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log("Cannot move claw left, either pickup is true or claw is at the limit.");
            }
        }
        else if (Input.GetKey(KeyCode.E))
        {
            if (clawLeft.transform.localPosition.x <= 0.008f)
            {
                clawLeft.transform.Translate(Vector3.right * clawSpeed * Time.deltaTime);
                clawRight.transform.Translate(Vector3.left * clawSpeed * Time.deltaTime);
            }
        }
        // Barrel movement
        if (barrel1Collision.isOnConveyor)
        {
            if (Input.GetKey(KeyCode.A))
            {
                barrel1RB.linearVelocity += new Vector3(-conveyorSpeed, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                barrel1RB.linearVelocity += new Vector3(conveyorSpeed, 0, 0);
            }
        }
        if (barrel2Collision.isOnConveyor && enableBarrel2)
        {
            if (Input.GetKey(KeyCode.A))
            {
                barrel2RB.linearVelocity += new Vector3(-conveyorSpeed, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                barrel2RB.linearVelocity += new Vector3(conveyorSpeed, 0, 0);
            }
        }
        // handle rotation
        if (Input.GetKey(KeyCode.W))
        {
            bool handleLeftMovable = handleLeft.transform.localRotation.eulerAngles.y <= 90f && !handleLeftTrigger_r.touched;
            bool handleRightMovable = handleRight.transform.localRotation.eulerAngles.y >= 2f && !handleRightTrigger_l.touched;
            if (handleLeftMovable && handleRightMovable)
            {
                handleLeft.transform.localRotation = Quaternion.Euler(0, handleLeft.transform.localRotation.eulerAngles.y + handleRotationSpeed * Time.deltaTime, 0);
                handleRight.transform.localRotation = Quaternion.Euler(0, handleRight.transform.localRotation.eulerAngles.y - handleRotationSpeed * Time.deltaTime, 0);
            }
            else
            {
                Debug.Log("[Handle] Cannot rotate handles due to normal constraints.");
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            bool handleLeftMovable = handleLeft.transform.localRotation.eulerAngles.y >= 2f && !handleLeftTrigger_l.touched;
            bool handleRightMovable = handleRight.transform.localRotation.eulerAngles.y <= 90f && !handleRightTrigger_r.touched;
            if (handleLeftMovable && handleRightMovable)
            {
                handleLeft.transform.localRotation = Quaternion.Euler(0, handleLeft.transform.localRotation.eulerAngles.y - handleRotationSpeed * Time.deltaTime, 0);
                handleRight.transform.localRotation = Quaternion.Euler(0, handleRight.transform.localRotation.eulerAngles.y + handleRotationSpeed * Time.deltaTime, 0);
            }
            else
            {
                Debug.Log("[Handle] Cannot rotate handles due to normal constraints.");
            }
        }
    }
}

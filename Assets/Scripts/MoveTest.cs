using UnityEngine;
using System.Collections.Generic;

public class MoveTest : MonoBehaviour
{
    [SerializeField] private GameObject conveyor;
    [SerializeField] private GameObject barrel1;
    private Rigidbody barrel1RB;
    private Barrel barrel1Collision;
    [SerializeField] private GameObject barrel2;
    private Rigidbody barrel2RB;
    [SerializeField] private GameObject gripper;
    private Rigidbody gripperRB;
    [SerializeField] private GameObject clawLeft;
    private Claw clawLeftTrigger;
    [SerializeField] private GameObject clawRight;
    private Claw clawRightTrigger;
    [SerializeField] private GameObject handleLeft;
    [SerializeField] private GameObject handleRight;

    [SerializeField] private float conveyorSpeed = 0.7f;
    [SerializeField] private float gripperVerticalSpeed = 0.5f;
    [SerializeField] private float gripperHorizontalSpeed = 0.1f;
    [SerializeField] private float clawSpeed = 0.05f;
    [SerializeField] private float handleRotationSpeed = 3f;
    [SerializeField] private float gravity = 9.8f;

    private bool pickup = false;

    void Start()
    {
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
        pickup = false;
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
        if (!pickup && !barrel1Collision.isGrounded) // Consider gravity
        {
            barrel1RB.linearVelocity = new Vector3(0, barrel1RB.linearVelocity.y - gravity * Time.deltaTime, 0);
        }
        else
        {
            barrel1RB.linearVelocity = Vector3.zero;
        }
        barrel2RB.linearVelocity = Vector3.zero;
        gripperRB.linearVelocity = Vector3.zero;

        // Gripper movement
        if (Input.GetKey(KeyCode.D))
        {
            gripperRB.linearVelocity += new Vector3(0, -gripperVerticalSpeed, 0);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            gripperRB.linearVelocity += new Vector3(0, gripperVerticalSpeed, 0);
        }

        // Claw movement
        if (clawLeftTrigger.touched && clawRightTrigger.touched)
        {
            pickup = true;
        }
        else
        {
            pickup = false;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (!pickup && clawLeft.transform.localPosition.x >= 0.0013f)
            {
                clawLeft.transform.Translate(Vector3.left * clawSpeed * Time.deltaTime);
                clawRight.transform.Translate(Vector3.right * clawSpeed * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeyCode.W))
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
            else if (Input.GetKey(KeyCode.S))
            {
                barrel1RB.linearVelocity += new Vector3(conveyorSpeed, 0, 0);
            }
        }
    }
}

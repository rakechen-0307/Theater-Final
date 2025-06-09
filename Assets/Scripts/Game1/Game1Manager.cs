using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Game1Manager : MonoBehaviour
{
    // Character
    [SerializeField] GameObject character;
    [SerializeField] private float speedX = 1.5f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private int decay_frame = 15;
    private Rigidbody characterRB;
    private List<bool> grounded = new List<bool>();
    private bool jump = false;
    private float acceleration = 0f;

    // System
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

    private FixedJoint gripJoint;
    [SerializeField] private bool pickup = false;
    [SerializeField] private bool enableBarrel2 = false;

    // UI
    [SerializeField] private Button gripperLeftBT;
    [SerializeField] private Button gripperRightBT;
    [SerializeField] private Button gripperUpBT;
    [SerializeField] private Button gripperDownBT;
    [SerializeField] private Button valveRightBT;
    [SerializeField] private Button valveLeftBT;
    [SerializeField] private Button conveyorLeftBT;
    [SerializeField] private Button conveyorRightBT;
    [SerializeField] private Button clawGrabBT;
    [SerializeField] private Button clawReleaseBT;

    private bool isGripperLeftHeld = false;
    private bool isGripperRightHeld = false;
    private bool isGripperUpHeld = false;
    private bool isGripperDownHeld = false;
    private bool isConveyorLeftHeld = false;
    private bool isConveyorRightHeld = false;
    private bool isValveLeftHeld = false;
    private bool isValveRightHeld = false;
    private bool isClawGrabHeld = false;
    private bool isClawReleaseHeld = false;

    [SerializeField] private int uiWidth = 1920;
    [SerializeField] private int uiHeight = 1080;
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool isDebug = false;
    [SerializeField] private float goalX = 6f;
    [SerializeField] private float goalY = 1.6f;

    void Start()
    {
        // Character
        characterRB = character.GetComponent<Rigidbody>();
        jump = false;
        acceleration = 0f;

        // System
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

        CheckHitsTest();
    }

    void Update()
    {
        /*
        // Receive Hokuyo data
        FrameData data = OSCDataParser.Instance.GetLatestFrame();
        if (data != null)
        {
            if (isDebug) {
                foreach (var entity in data.Entities)
                {
                    Debug.Log($"Entity {entity.ID}: X={entity.X}, Y={entity.Y}");
                }
            }
            CheckHits(data);
        }
        */

        if (Input.GetKeyDown(KeyCode.Space))
        {
            enableBarrel2 = true;
            barrel2.GetComponent<Collider>().enabled = true; // Enable barrel2 collider
            barrel2.GetComponent<MeshRenderer>().enabled = true; // Enable barrel2 mesh renderer
            Debug.Log($"[Barrel2] Enabled: {enableBarrel2}");
        }

        if (character.transform.position.x >= goalX && character.transform.position.y >= goalY)
        {
            GoToNextScene();
            return;
        }
    }

    void FixedUpdate()
    {
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

        // Gripper
        if (isGripperLeftHeld)
            gripper_systemRB.linearVelocity += new Vector3(-gripperHorizontalSpeed, 0, 0);
        if (isGripperRightHeld)
            gripper_systemRB.linearVelocity += new Vector3(gripperHorizontalSpeed, 0, 0);
        if (isGripperUpHeld && gripper.transform.localPosition.z <= -0.01599986f)
            gripperRB.linearVelocity += new Vector3(0, gripperVerticalSpeed, 0);
        if (isGripperDownHeld)
            gripperRB.linearVelocity += new Vector3(0, -gripperVerticalSpeed, 0);

        // Conveyor
        if (isConveyorLeftHeld)
        {
            if (barrel1Collision.isOnConveyor)
                barrel1RB.linearVelocity += new Vector3(-conveyorSpeed, 0, 0);
            else if (barrel2Collision.isOnConveyor && enableBarrel2)
                barrel2RB.linearVelocity += new Vector3(-conveyorSpeed, 0, 0);
        }
        if (isConveyorRightHeld)
        {
            if (barrel1Collision.isOnConveyor)
                barrel1RB.linearVelocity += new Vector3(conveyorSpeed, 0, 0);
            else if (barrel2Collision.isOnConveyor && enableBarrel2)
                barrel2RB.linearVelocity += new Vector3(conveyorSpeed, 0, 0);
        }

        // Claw
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

        if (isClawGrabHeld && !pickup && clawLeft.transform.localPosition.x >= 0.0013f)
        {
            clawLeft.transform.Translate(Vector3.left * clawSpeed * Time.deltaTime);
            clawRight.transform.Translate(Vector3.right * clawSpeed * Time.deltaTime);
        }
        if (isClawReleaseHeld && clawLeft.transform.localPosition.x <= 0.008f)
        {
            clawLeft.transform.Translate(Vector3.right * clawSpeed * Time.deltaTime);
            clawRight.transform.Translate(Vector3.left * clawSpeed * Time.deltaTime);
        }

        // Valve
        if (isValveRightHeld)
        {
            bool handleLeftMovable = handleLeft.transform.localRotation.eulerAngles.y >= 2f && !handleLeftTrigger_l.touched;
            bool handleRightMovable = handleRight.transform.localRotation.eulerAngles.y <= 90f && !handleRightTrigger_r.touched;
            if (handleLeftMovable && handleRightMovable)
            {
                handleLeft.transform.localRotation = Quaternion.Euler(0, handleLeft.transform.localRotation.eulerAngles.y - handleRotationSpeed * Time.deltaTime, 0);
                handleRight.transform.localRotation = Quaternion.Euler(0, handleRight.transform.localRotation.eulerAngles.y + handleRotationSpeed * Time.deltaTime, 0);
            }
        }
        if (isValveLeftHeld)
        {
            bool handleLeftMovable = handleLeft.transform.localRotation.eulerAngles.y <= 90f && !handleLeftTrigger_r.touched;
            bool handleRightMovable = handleRight.transform.localRotation.eulerAngles.y >= 2f && !handleRightTrigger_l.touched;
            if (handleLeftMovable && handleRightMovable)
            {
                handleLeft.transform.localRotation = Quaternion.Euler(0, handleLeft.transform.localRotation.eulerAngles.y + handleRotationSpeed * Time.deltaTime, 0);
                handleRight.transform.localRotation = Quaternion.Euler(0, handleRight.transform.localRotation.eulerAngles.y - handleRotationSpeed * Time.deltaTime, 0);
            }
        }
    }

    void CheckHits(FrameData dataPoints)
    {
        foreach (var entity in dataPoints.Entities)
        {
            Vector2 point = new Vector2(entity.X, entity.Y);
            /*
            if (isDebug)
            {
                Debug.Log($"Sensor Point: {point}");
            }
            */

            // TODO: check whether the data's position is on the button
        }
    }

    void CheckHitsTest()
    {
        AddPressEvent(gripperLeftBT, () => isGripperLeftHeld = true, () => isGripperLeftHeld = false);
        AddPressEvent(gripperRightBT, () => isGripperRightHeld = true, () => isGripperRightHeld = false);
        AddPressEvent(gripperUpBT, () => isGripperUpHeld = true, () => isGripperUpHeld = false);
        AddPressEvent(gripperDownBT, () => isGripperDownHeld = true, () => isGripperDownHeld = false);
        AddPressEvent(conveyorLeftBT, () => isConveyorLeftHeld = true, () => isConveyorLeftHeld = false);
        AddPressEvent(conveyorRightBT, () => isConveyorRightHeld = true, () => isConveyorRightHeld = false);
        AddPressEvent(valveLeftBT, () => isValveLeftHeld = true, () => isValveLeftHeld = false);
        AddPressEvent(valveRightBT, () => isValveRightHeld = true, () => isValveRightHeld = false);
        AddPressEvent(clawGrabBT, () => isClawGrabHeld = true, () => isClawGrabHeld = false);
        AddPressEvent(clawReleaseBT, () => isClawReleaseHeld = true, () => isClawReleaseHeld = false);
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    void AddPressEvent(Button button, System.Action onDown, System.Action onUp)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        var entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entryDown.callback.AddListener((e) => onDown());
        trigger.triggers.Add(entryDown);

        var entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entryUp.callback.AddListener((e) => onUp());
        trigger.triggers.Add(entryUp);
    }
}

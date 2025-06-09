using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Game1Manager : MonoBehaviour
{
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

    [SerializeField] private float gravity = 9.8f;
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
    private bool isConveyorSoundPlaying = false;
    private bool isClawSoundPlaying = false;
    private bool isValveSoundPlaying = false;

    [SerializeField] private int uiWidth = 1920;
    [SerializeField] private int uiHeight = 1080;
    [SerializeField] private bool isDebug = false;

    void Start()
    {
        // BGM
        if (OSCSender.Instance != null)
        {
            OSCSender.Instance.PlaySound("game1", 1);
        }

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

        // TODO: disable this
        CheckHitsTest();
    }

    void Update()
    {
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
        if (OSCSender.Instance != null && (isGripperLeftHeld || isGripperRightHeld || isGripperUpHeld || isGripperDownHeld) && !isClawSoundPlaying)
        {
            OSCSender.Instance.PlaySound("arm", 1);
            isClawSoundPlaying = true;
        }
        else if (OSCSender.Instance != null && isClawSoundPlaying && !isGripperLeftHeld && !isGripperRightHeld && !isGripperUpHeld && !isGripperDownHeld)
        {
            OSCSender.Instance.PlaySound("arm", 0);
            isClawSoundPlaying = false;
        }

        // Conveyor
        if (isConveyorLeftHeld)
        {
            if (barrel1Collision.isOnConveyor)
                barrel1RB.linearVelocity += new Vector3(-conveyorSpeed, 0, 0);
            else if (barrel2Collision.isOnConveyor && enableBarrel2)
                barrel2RB.linearVelocity += new Vector3(-conveyorSpeed, 0, 0);
            if (OSCSender.Instance != null && !isConveyorSoundPlaying)
            {
                OSCSender.Instance.PlaySound("conveyor", 1);
                isConveyorSoundPlaying = true;
            }
        }
        else if (isConveyorSoundPlaying && !isConveyorRightHeld)
        {
            if (OSCSender.Instance != null)
            {
                OSCSender.Instance.PlaySound("conveyor", 0);
                isConveyorSoundPlaying = false;
            }
        }
        if (isConveyorRightHeld)
        {
            if (barrel1Collision.isOnConveyor)
                barrel1RB.linearVelocity += new Vector3(conveyorSpeed, 0, 0);
            else if (barrel2Collision.isOnConveyor && enableBarrel2)
                barrel2RB.linearVelocity += new Vector3(conveyorSpeed, 0, 0);
            if (OSCSender.Instance != null && !isConveyorSoundPlaying)
            {
                OSCSender.Instance.PlaySound("conveyor", 1);
                isConveyorSoundPlaying = true;
            }
        }
        else if (isConveyorSoundPlaying && !isConveyorLeftHeld)
        {
            if (OSCSender.Instance != null)
            {
                OSCSender.Instance.PlaySound("conveyor", 0);
                isConveyorSoundPlaying = false;
            }
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
            if (OSCSender.Instance != null && !isClawSoundPlaying)
            {
                OSCSender.Instance.PlaySound("arm", 1);
                isClawSoundPlaying = true;
            }
        }
        else if (isClawSoundPlaying && !isClawReleaseHeld)
        {
            if (OSCSender.Instance != null)
            {
                OSCSender.Instance.PlaySound("arm", 0);
                isClawSoundPlaying = false;
            }
        }
        if (isClawReleaseHeld && clawLeft.transform.localPosition.x <= 0.008f)
        {
            clawLeft.transform.Translate(Vector3.right * clawSpeed * Time.deltaTime);
            clawRight.transform.Translate(Vector3.left * clawSpeed * Time.deltaTime);
            if (OSCSender.Instance != null && !isClawSoundPlaying)
            {
                OSCSender.Instance.PlaySound("arm", 1);
                isClawSoundPlaying = true;
            }
        }
        else if (isClawSoundPlaying && !isClawGrabHeld)
        {
            if (OSCSender.Instance != null)
            {
                OSCSender.Instance.PlaySound("arm", 0);
                isClawSoundPlaying = false;
            }
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
        // Single Sound
        if(OSCSender.Instance != null && (isValveLeftHeld || isValveRightHeld))
        {
            OSCSender.Instance.PlaySound("gas", 1);
        }
        // // Loop sound
        // if (OSCSender.Instance != null && (isValveLeftHeld || isValveRightHeld) && !isValveSoundPlaying)
        // {
        //     OSCSender.Instance.PlaySound("gas", 1);
        //     isValveSoundPlaying = true;
        // }
        // else if (OSCSender.Instance != null && isValveSoundPlaying && !isValveLeftHeld && !isValveRightHeld)
        // {
        //     OSCSender.Instance.PlaySound("gas", 0);
        //     isValveSoundPlaying = false;
        // }
    }

    void CheckHits(FrameData dataPoints)
    {
        isGripperLeftHeld = false;
        isGripperRightHeld = false;
        isGripperUpHeld = false;
        isGripperDownHeld = false;
        isConveyorLeftHeld = false;
        isConveyorRightHeld = false;
        isValveLeftHeld = false;
        isValveRightHeld = false;
        isClawGrabHeld = false;
        isClawReleaseHeld = false;

        foreach (var entity in dataPoints.Entities)
        {
            Vector2 point = new Vector2(entity.X, entity.Y);

            // Check each button
            if (IsPointInButton(gripperLeftBT, point)) isGripperLeftHeld = true;
            if (IsPointInButton(gripperRightBT, point)) isGripperRightHeld = true;
            if (IsPointInButton(gripperUpBT, point)) isGripperUpHeld = true;
            if (IsPointInButton(gripperDownBT, point)) isGripperDownHeld = true;
            if (IsPointInButton(conveyorLeftBT, point)) isConveyorLeftHeld = true;
            if (IsPointInButton(conveyorRightBT, point)) isConveyorRightHeld = true;
            if (IsPointInButton(valveLeftBT, point)) isValveLeftHeld = true;
            if (IsPointInButton(valveRightBT, point)) isValveRightHeld = true;
            if (IsPointInButton(clawGrabBT, point)) isClawGrabHeld = true;
            if (IsPointInButton(clawReleaseBT, point)) isClawReleaseHeld = true;
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

    bool IsPointInButton(Button button, Vector2 point)
    {
        RectTransform rt = button.GetComponent<RectTransform>();
        Vector2 buttonCenter = rt.anchoredPosition;
        Vector2 buttonSize = rt.rect.size;

        float left = uiWidth / 2 + buttonCenter.x - buttonSize.x / 2;
        float right = uiWidth / 2 - buttonCenter.x + buttonSize.x / 2;
        float bottom = uiHeight / 2 + buttonCenter.y - buttonSize.y / 2;
        float top = uiHeight / 2 - buttonCenter.y + buttonSize.y / 2;

        return (point.x >= left && point.x <= right &&
                point.y >= bottom && point.y <= top);
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

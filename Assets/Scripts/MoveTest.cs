using UnityEngine;
using System.Collections.Generic;

public class MoveTest : MonoBehaviour
{
    [SerializeField] private GameObject conveyor;
    [SerializeField] private GameObject barrel1;
    [SerializeField] private GameObject barrel2;
    [SerializeField] private GameObject gripper;
    [SerializeField] private GameObject zstring;
    [SerializeField] private GameObject clawLeft;
    [SerializeField] private GameObject clawRight;
    [SerializeField] private GameObject handleLeft;
    [SerializeField] private GameObject handleRight;

    [SerializeField] private float conveyorSpeed = 0.7f;
    [SerializeField] private float gripperVerticalSpeed = 0.5f;
    [SerializeField] private float gripperHorizontalSpeed = 0.1f;
    [SerializeField] private float clawSpeed = 0.05f;
    [SerializeField] private float handleRotationSpeed = 3f;

    // Update is called once per frame
    void Update()
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

        if (Input.GetKey(KeyCode.A))
        {
            barrel1.transform.position = new Vector3(
                barrel1.transform.position.x + Time.deltaTime * conveyorSpeed,
                barrel1.transform.position.y,
                barrel1.transform.position.z
            );
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            barrel1.transform.position = new Vector3(
                barrel1.transform.position.x - Time.deltaTime * conveyorSpeed,
                barrel1.transform.position.y,
                barrel1.transform.position.z
            );
        }

        if (Input.GetKey(KeyCode.D))
        {
            gripper.transform.position = new Vector3(
                gripper.transform.position.x,
                gripper.transform.position.y - Time.deltaTime * gripperVerticalSpeed,
                gripper.transform.position.z
            );
            zstring.transform.position = new Vector3(
                zstring.transform.position.x,
                zstring.transform.position.y - Time.deltaTime * gripperVerticalSpeed,
                zstring.transform.position.z
            );
        }
    }
}

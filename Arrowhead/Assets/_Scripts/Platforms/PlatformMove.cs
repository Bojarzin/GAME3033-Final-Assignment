using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public GameObject platform;

    Vector3 positionOneVector;
    Vector3 positionTwoVector;
    public Transform positionTwo;

    Quaternion rotationOneQuaternion;
    Quaternion rotationTwoQuaternion;
    public Transform rotationTwo;

    public float movementSpeed;
    public float rotationSpeed;

    public bool rotate = false;

    // Start is called before the first frame update
    void Start()
    {
        positionOneVector = platform.transform.position;
        positionTwoVector = positionTwo.position;

        rotationOneQuaternion = platform.transform.rotation;
        if (rotate)
        {
            rotationTwoQuaternion = rotationTwo.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlatform();
        RotatePlatform();
    }

    void MovePlatform()
    {
        platform.transform.position = Vector3.Lerp(positionOneVector, positionTwoVector, Mathf.PingPong(Time.time * movementSpeed, 1.0f));
    }

    void RotatePlatform()
    {
        if (rotate)
        {
            transform.rotation = Quaternion.Lerp(rotationOneQuaternion, rotationTwoQuaternion, Mathf.PingPong(Time.time * rotationSpeed, 1.0f));
        }
    }
}

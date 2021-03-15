using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform lookAt;

    void Start()
    {
        
    }

    void Update()
    {
        LookAt();
    }

    void LookAt()
    {
        transform.LookAt(lookAt);
    }
}

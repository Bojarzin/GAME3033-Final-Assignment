using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;

    /// Movement
    [Header("Movement")]
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Transform arrowTip;
    [SerializeField] float moveSpeed;
    [SerializeField] float diveStrength;
    [SerializeField] float maxDiveStrength;
    [SerializeField] float rotationSpeed;
    [SerializeField] float diveTurnSpeed;
    [SerializeField] float diveRotationSpeed;
    [SerializeField] float jumpStrength;

    [Header("Constraints")]
    public bool isGrounded;
    public bool isMoving;
    public bool isDiving = false;
    public bool chargeDive = false;
    public bool isStuck = false;

    int divesLeft = 1;

    Vector2 inputDirection;
    Vector3 moveDirection;
    Quaternion rotationToMoveDirection;
    Quaternion rotationToCamera;

    /// Animation
    [Header("Animation")]
    [SerializeField] Animator animator;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Movement.Move.performed += context => inputDirection = context.ReadValue<Vector2>();
        controls.Movement.Move.canceled += context => inputDirection = context.ReadValue<Vector2>();

        controls.Movement.Jump.started += context => Jump();


        controls.Movement.ChargeDive.performed += controls => ChargeDive();
        controls.Movement.Dive.performed += context => Dive();

    }

    void OnEnable()
    {
        controls.Enable();    
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    { 

    }

    void Update()
    {
        IncreaseDiveStrength();
        ChangesWhenStuck();
    }

    void FixedUpdate()
    {
        Rotation();
        Movement();
    }

    void LateUpdate()
    {
        UpdateRunAndIdleAnimation();
    }

    public void OnJump(InputAction.CallbackContext _value)
    {
        if (_value.interaction is HoldInteraction)
        {
            Debug.Log("Jump");
        }
    }

    void Movement()
    {
        if (!isStuck && !isDiving && !chargeDive)
        {
            moveDirection = Vector3.forward * inputDirection.y + Vector3.right * inputDirection.x;

            Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
            rotationToCamera = Quaternion.LookRotation(cameraForward, Vector3.up);

            moveDirection = rotationToCamera * moveDirection;

            if (inputDirection.y != 0 || inputDirection.x != 0)
            {
                rigidbody.MovePosition(rigidbody.position + moveDirection * moveSpeed * Time.deltaTime);
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
    }

    void Rotation()
    {
        if (!isStuck && !chargeDive)
        {
            if (inputDirection.x != 0 || inputDirection.y != 0)
            {
                if (!isDiving)
                {
                    rotationToMoveDirection = Quaternion.LookRotation(moveDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToMoveDirection, rotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                if (isDiving)
                {
                    DiveRotation();
                }
            }
        }
    }

    void DiveRotation()
    {
        transform.LookAt(transform.position + rigidbody.velocity);
        transform.rotation *= Quaternion.Euler(90, 0, 0);
    }

    void Jump()
    {
        if (isStuck)
        {
            isStuck = false;
            rigidbody.AddForce(Vector3.up - transform.forward * jumpStrength, ForceMode.Impulse);
            animator.speed = 1;
            animator.SetBool("Jump", true);
        }
        if (isGrounded)
        {
            rigidbody.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            animator.speed = 1;
            animator.SetBool("Jump", true);
        }
    }

    void ChargeDive()
    {
        if (!isDiving && divesLeft > 0)
        {
            Physics.gravity = new Vector3(0.0f, -1.0f, 0.0f);

            chargeDive = true;
        }
    }

    void IncreaseDiveStrength()
    {
        if (chargeDive && diveStrength < maxDiveStrength)
        {
            diveStrength += 0.1f;
        }
        if (diveStrength > maxDiveStrength)
        {
            diveStrength = maxDiveStrength;
        }
    }

    void Dive()
    {
        if (divesLeft > 0)
        {
            isDiving = true;
            divesLeft -= 1;
            Physics.gravity = new Vector3(0.0f, -25.0f, 0.0f);
            rigidbody.AddForce(Camera.main.transform.forward * diveStrength, ForceMode.Impulse);
            //rigidbody.AddTorque(Camera.main.transform.forward + new Vector3(1, 0, 0) * diveStrength/500, ForceMode.Impulse);

            chargeDive = false;
            diveStrength = 0.0f;
            animator.SetBool("Dive", true);
        }
    }

    void ChangesWhenStuck()
    {
        if (isStuck)
        {
            Physics.gravity = new Vector3(0, 0, 0);
            isGrounded = true;
        }
        else
        {
            if (!chargeDive)
            {
                Physics.gravity = new Vector3(0, -25, 0);
            }
        }
    }

    void UpdateRunAndIdleAnimation()
    {
        float animationSpeed = isMoving ? 1.0f : 0.0f;
        animator.SetFloat("Speed", animationSpeed);
        if (!isMoving)
        {
            animator.speed = 1;
        }
        animator.speed = 2;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            rigidbody.velocity = new Vector3(0, 0, 0);

            animator.SetBool("Jump", false);
            animator.SetBool("Dive", false);

            isDiving = false;
            divesLeft = 1;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isDiving = false;
            divesLeft = 1;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stickable"))
        {
            if (isDiving)
            {
                rigidbody.velocity = new Vector3(0, 0, 0);
                isStuck = true;
                isDiving = false;
                divesLeft = 1;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Stickable"))
        { 
            isStuck = false;
            isGrounded = false;
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }
    }

    public float GetMaxDiveStrength()
    {
        return maxDiveStrength;
    }

    public float GetCurrentDiveStrength()
    {
        return diveStrength;
    }
}


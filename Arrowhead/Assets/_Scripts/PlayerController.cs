using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;

    /// Movement
    [Header("Movement")]
    [SerializeField] public Rigidbody rigidbody;
    [SerializeField] Transform arrowTip;
    public float moveSpeed;
    public float diveStrength;
    public float maxDiveStrength;
    public float rotationSpeed;
    public float diveTurnSpeed;
    public float diveRotationSpeed;
    public float jumpStrength;
    [Range(0,1)] public float airControlPercent;

    [Header("Constraints")]
    public Transform startingPosition;
    public bool isGrounded;
    public bool isMoving;
    public bool isDiving = false;
    public bool chargeDive = false;
    public bool isStuck = false;
    public bool isWet = false;

    [Header("Effects")]
    public ParticleSystem rocket;
    public AudioSource walkingAudioSource;
    public AudioSource flyingAudioSource;
    public AudioSource stuckAudioSource;
    public AudioClip[] audioClips;

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

        controls.Movement.Pause.performed += context => PauseGame();
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
        transform.position = startingPosition.position;
    }

    void Update()
    {
        IncreaseDiveStrength();
        ChangesWhenStuck();

        PlayAudio();
    }

    void FixedUpdate()
    {
        Movement();
        Rotation();
    }

    void LateUpdate()
    {
        UpdateRunAndIdleAnimation();
    }

    void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    void Movement()
    {
        if (!isStuck && !isDiving)
        {
            if (!isGrounded && chargeDive)
            {
                
            }
            else
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
    }

    Vector3 AirControlChange(Vector3 _vector)
    {
        if (isGrounded)
        {
            return _vector;
        }
        if (airControlPercent == 0)
        {
            return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        return _vector * airControlPercent;
    }

    void Rotation()
    {
        if (!isStuck)
        {
            if (inputDirection.x != 0 || inputDirection.y != 0)
            {
                if (!isDiving)
                {
                    rotationToMoveDirection = Quaternion.LookRotation(moveDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToMoveDirection, rotationSpeed * Time.deltaTime);
                }
                if (isDiving)
                {
                    DiveRotation();
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
        if (isStuck && !chargeDive)
        {
            isStuck = false;
            rigidbody.AddForce(Vector3.up - transform.forward * jumpStrength/200, ForceMode.Impulse);
            animator.speed = 1;
            animator.SetBool("Jump", true);
        }
        if (isGrounded && !chargeDive)
        {
            rigidbody.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            animator.speed = 1;
            animator.SetBool("Jump", true);
        }
    }

    void ChargeDive()
    {
        if (!isDiving && divesLeft > 0 && Time.timeScale != 0.0f)
        {
            if (!isGrounded)
            {
                Physics.gravity = new Vector3(0.0f, -3.0f, 0.0f);
            }
            chargeDive = true;
        }
    }

    void IncreaseDiveStrength()
    {
        if (chargeDive && diveStrength < maxDiveStrength)
        {
            diveStrength += 30.0f * Time.deltaTime;
            if (rigidbody.velocity != Vector3.zero)
            {
                rigidbody.AddForce(-rigidbody.velocity);
            }
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
            diveStrength = 10.0f;
            animator.SetBool("Dive", true);

            rocket.Play();
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

    void PlayAudio()
    {
        if (isMoving && isGrounded && !isStuck)
        {
            if (!walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Play();
            }
        }
        else
        {
            walkingAudioSource.Stop();
        }

        if (isDiving && !isWet)
        {
            if (!flyingAudioSource.isPlaying)
            {
                flyingAudioSource.Play();
            }
        }
        else
        {
            if (flyingAudioSource.isPlaying)
            {
                flyingAudioSource.volume -= 0.010f;

                if (flyingAudioSource.volume <= 0.0f)
                {
                    flyingAudioSource.Stop();
                    flyingAudioSource.volume = 0.5f;
                }
            }
        }

        if (Time.timeScale != 1.0f)
        {
            walkingAudioSource.Stop();
            flyingAudioSource.Stop();
        }
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

            rocket.Stop();
        }

        if (collision.gameObject.GetComponent<GroundPlatformComponent>())
        {
            isGrounded = true;
            transform.SetParent(collision.gameObject.transform);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        //if (collision.gameObject.GetComponent<GroundPlatformComponent>())
        //{
        //    isGrounded = true;
        //    transform.SetParent(collision.gameObject.transform);
        //}
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.GetComponent<GroundPlatformComponent>())
        {
            isGrounded = false;
            transform.SetParent(null);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<StickableComponent>())
        {
            if (isDiving)
            {
                rigidbody.velocity = new Vector3(0, 0, 0);
                isStuck = true;
                isDiving = false;
                divesLeft = 1;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                transform.SetParent(other.gameObject.transform);

                rocket.Stop();

                if (!stuckAudioSource.isPlaying)
                {
                    stuckAudioSource.Play();
                    stuckAudioSource.time = 0.05f;
                }
            }
        }

        if (other.gameObject.GetComponent<GroundPlatformComponent>())
        {
            transform.SetParent(other.gameObject.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<StickableComponent>())
        {
            isStuck = false;
            isGrounded = false;
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

            transform.SetParent(null);
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


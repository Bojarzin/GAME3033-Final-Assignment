using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WaterHazard : MonoBehaviour
{
    public Transform respawnTransform;
    public PlayerController player;
    public GameObject splash;

    public CinemachineFreeLook freeLookCamera;

    Transform initialPlayerTransform;
    float delayRespawn;
    bool startDelay = false;
    bool secondDelay = false;

    // Start is called before the first frame update
    void Start()
    {
        initialPlayerTransform = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (startDelay)
        {
            delayRespawn += Time.deltaTime;
            if (delayRespawn >= 0.02f)
            {
                player.transform.localScale = new Vector3(0, 0, 0);
                player.rigidbody.velocity = new Vector3(0, 0, 0);
                player.rigidbody.useGravity = false;
                freeLookCamera.m_XAxis.m_MaxSpeed = 0;
                freeLookCamera.m_YAxis.m_MaxSpeed = 0;
                delayRespawn = 0.0f;
                startDelay = false;
                secondDelay = true;
            }
        }
        if (secondDelay)
        {
            delayRespawn += Time.deltaTime;
            if (delayRespawn >= 2.0f)
            {
                player.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f); ;
                player.rigidbody.useGravity = true;
                player.isWet = false;
                player.transform.position = GameManager.Instance().activeRespawn.position;
                player.transform.rotation = GameManager.Instance().activeRespawn.rotation;
                freeLookCamera.m_XAxis.m_MaxSpeed = 250;
                freeLookCamera.m_YAxis.m_MaxSpeed = 2;
                delayRespawn = 0.0f;
                secondDelay = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!player.isWet)
            {
                player.isWet = true;
                startDelay = true;

                Instantiate(splash, player.transform.position + Vector3.up, Quaternion.identity);
            }
        }
    }
}

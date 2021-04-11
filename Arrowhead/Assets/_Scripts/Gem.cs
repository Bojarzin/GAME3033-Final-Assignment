using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gem : MonoBehaviour
{
    public bool isGem;
    public string gemName;
    public Image sprite;
    public AudioSource audioSource;

    public enum GemType
    {
        FOREST,
        CITY,
        WATER
    };

    public GemType gemType;

    private void Start()
    {
        isGem = true;
        sprite.color = new Vector4(1.0f, 1.0f, 1.0f, 0.25f);
    }

    IEnumerator KillSelf()
    {
        audioSource.Play();

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            GameManager.Instance().gemsCollected.Add(this);
            sprite.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            StartCoroutine(KillSelf());
        }
    }
}
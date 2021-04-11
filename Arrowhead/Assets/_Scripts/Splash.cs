using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(KillSelf());

        audioSource.Play();
        audioSource.time = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator KillSelf()
    {
        yield return new WaitForSeconds(2.0f);

        Destroy(gameObject);
    }
}

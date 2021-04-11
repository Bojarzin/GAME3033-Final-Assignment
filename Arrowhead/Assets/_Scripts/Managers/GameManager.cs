using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance()
    {
        return instance;
    }

    public List<Gem> gemsCollected;
    public Transform activeRespawn;

    public bool viewMouse;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        viewMouse = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseVisibility();
    }

    void MouseVisibility()
    {
        Cursor.lockState = viewMouse == true ? CursorLockMode.None : CursorLockMode.Confined;
        
        Cursor.visible = viewMouse;
    }

    void WinGame()
    {
        if (gemsCollected.Count >= 3)
        {

        }
    }
}

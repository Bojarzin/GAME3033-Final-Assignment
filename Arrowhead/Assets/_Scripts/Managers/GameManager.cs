using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance()
    {
        return instance;
    }

    public List<Gem> gemsCollected;
    public Transform activeRespawn;

    public Canvas gameCanvas;
    public Canvas pauseCanvas;
    public Canvas winCanvas;
    public TMP_Text boardText;

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
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        MouseVisibility();
        UpdateCanvas();
        WinGame();
    }

    void MouseVisibility()
    {
        Cursor.lockState = CursorLockMode.Confined;
        
        Cursor.visible = viewMouse;

        if (Time.timeScale == 0.0f)
        {
            viewMouse = true;
        }
        else
        {
            viewMouse = false;
        }
    }

    void WinGame()
    {
        if (gemsCollected.Count >= 3)
        {
            boardText.text = "Thanks for finding my gems! I guess I don't really have a prize. But I hope you had fun! Feel free to roam the world.";
        }
    }

    void UpdateCanvas()
    {
        if (Time.timeScale == 0.0f)
        {
            pauseCanvas.enabled = true;
            gameCanvas.enabled = false;
        }
        else
        {
            pauseCanvas.enabled = false;
            gameCanvas.enabled = true;
        }
    }
}

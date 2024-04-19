using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public enum GameMode
    {
        Build,
        Climb
    }
    
    public GameMode gameMode = GameMode.Build;
    public GameObject player;
    public GameObject buildModeUI;
    public GameObject climbModeUI;
    public GameObject buildModeCamera;
    public GameObject climbModeCamera;

    public GameObject HoldPlacer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
    }
    
    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Switching to build mode.");
            gameMode = GameMode.Build;
            player.GameObject().SetActive(false);
            HoldPlacer.GameObject().SetActive(true);
            buildModeUI.GameObject().SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Switching to climb mode.");
            gameMode = GameMode.Climb;
            player.GameObject().SetActive(true);
            HoldPlacer.GameObject().SetActive(false);
            buildModeUI.GameObject().SetActive(false);
        }
    }
    
    
}

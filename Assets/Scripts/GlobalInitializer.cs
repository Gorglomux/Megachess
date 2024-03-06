using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInitializer : MonoBehaviour
{

    public GlobalVariables global;
    public Material paletteMaterial;
    public Material unitMaterial;
    public GameObject unitPrefab;
    public IndicatorManager indicatorManager;
    public InputManager inputManager;
    public UIManager uiManager;
    public CameraMovementManager cameraMovement;
    public GameManager gameManager;
    public GameObject playerPrefab;

    public BloodSplatManager bloodSplatManager;
    private void Awake()
    {
        GlobalHelper.GlobalVariables = global;
        global.paletteMaterial = paletteMaterial;
        global.unitMaterial = unitMaterial;
        global.unitPrefab = unitPrefab;
        global.indicatorManager = indicatorManager;
        global.inputManager = inputManager;
        global.UIManager = uiManager;
        global.cameraMovement = cameraMovement;
        global.gameManager = gameManager;
        GlobalHelper.LoadGame();
        global.bloodSplatManager = bloodSplatManager;
        global.player = GameObject.Instantiate(playerPrefab).GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

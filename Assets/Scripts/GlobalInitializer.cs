using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInitializer : MonoBehaviour
{

    public GlobalVariables global;
    public Material paletteMaterial;
    public GameObject unitPrefab;
    public IndicatorManager indicatorManager;
    public InputManager inputManager;
    public UIManager uiManager;

    public GameObject playerPrefab;
    private void Awake()
    {
        GlobalHelper.GlobalVariables = global;
        global.paletteMaterial = paletteMaterial;
        global.unitPrefab = unitPrefab;
        global.indicatorManager = indicatorManager;
        global.inputManager = inputManager;
        global.UIManager = uiManager;
        GlobalHelper.LoadGame();

        global.player = GameObject.Instantiate(playerPrefab).GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

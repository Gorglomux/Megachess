using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInitializer : MonoBehaviour
{

    public GlobalVariables global;
    public Material paletteMaterial;
    public GameObject unitPrefab;
    public IndicatorManager indicatorManager;
    private void Awake()
    {
        GlobalHelper.GlobalVariables = global;
        global.paletteMaterial = paletteMaterial;
        global.unitPrefab = unitPrefab;
        global.indicatorManager = indicatorManager;
        GlobalHelper.LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

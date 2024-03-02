using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInitializer : MonoBehaviour
{

    public GlobalVariables global;
    private void Awake()
    {
        GlobalHelper.GlobalVariables = global;
        GlobalHelper.LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

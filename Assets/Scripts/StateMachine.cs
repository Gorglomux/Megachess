using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IState
{
    void OnEntry(GameManager gm);

    void OnUpdate(GameManager gm);

    void OnExit(GameManager gm);
}

public class UnitPlaceState : IState
{
    public void OnEntry(GameManager gm)
    {
        Debug.Log("Starting unit place phase");
    }

    public void OnExit(GameManager gm)
    {
        Debug.Log("Ending unit place phase");
    }

    public void OnUpdate(GameManager gm)
    {
        throw new System.NotImplementedException();
    }
}
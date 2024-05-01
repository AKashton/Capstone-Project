using MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRootMenu : MonoBehaviour
{
    [SerializeField] SolverHandler solverHandler;

    public void ToggleSolverUpdate()
    {
        solverHandler.UpdateSolvers = !solverHandler.UpdateSolvers;
    }
}

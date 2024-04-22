using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    TileManager tileManager;
    Vector2 startLocation;

    void Awake()
    {
        tileManager = GameObject.FindWithTag("TileManager").GetComponent<TileManager>();
    }

    void OnMouseDown()
    {
        if (!tileManager.CheckPuzzleCompletion())
            AttemptMove(true);
    }

    public void AssignStartLocation(Vector2 input)
    {
        startLocation = input;
    }

    public void AttemptMove(bool puzzleStarted = false)
    {
        if (Vector2.Distance(transform.position, tileManager.GetEmptyLocation()) > 1)
            Debug.Log("Too far");
        else
        {
            Vector2 tempVector = transform.position;
            transform.position = tileManager.GetEmptyLocation();
            tileManager.SetEmptyLocation(tempVector);

            if (puzzleStarted)
                tileManager.CheckFinished();
        }
    }

    public float GetDistanceFromStart()
    {
        return Vector2.Distance(transform.position, startLocation);
    }
}

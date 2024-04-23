using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] SpriteRenderer borderImage;

    TileManager tileManager;
    Vector3 startLocation;

    void Awake()
    {
        tileManager = GameObject.FindWithTag("TileManager").GetComponent<TileManager>();
    }

    /*
    void OnMouseDown()
    {
        if (!tileManager.CheckPuzzleCompletion())
            AttemptMove(true);
    }
    */

    public void ClickTile()
    {
        tileManager.CallTileSwap(gameObject);

        /*
        Collider[] possibleTiles = Physics.OverlapSphere(transform.position, 1);
        List<GameObject> closeTiles = new List<GameObject>();

        for (int i = 0; i < possibleTiles.Length; i++)
        {
            if (Vector3.Distance(possibleTiles[i].gameObject.transform.position, transform.position) <= 1)
                closeTiles.Add(possibleTiles[i].gameObject);
        }

        Debug.Log(possibleTiles.Length);
        Debug.Log(closeTiles.Count);
        */
    }

    public void AssignStartLocation()
    {
        startLocation = transform.localPosition;
    }

    public void AttemptMove(bool puzzleStarted = false)
    {
        if (Vector3.Distance(transform.position, tileManager.GetEmptyLocation()) > 1)
            Debug.Log("Too far");
        else
        {
            Vector3 tempVector = transform.position;
            transform.position = tileManager.GetEmptyLocation();
            tileManager.SetEmptyLocation(tempVector);

            if (puzzleStarted)
                tileManager.CheckFinished();
        }
    }

    public float GetDistanceFromStart()
    {
        return Vector3.Distance(transform.localPosition, startLocation);
    }

    public void FadeBorder()
    {
        StartCoroutine("FadeBorderImage", 255);
    }

    IEnumerator FadeBorderImage(int imageAlpha)
    {
        imageAlpha -= 11;

        Color tempColor = borderImage.color;
        tempColor.a = (float)imageAlpha / 255;
        borderImage.color = tempColor;

        yield return new WaitForSeconds(0.1f);

        if (imageAlpha > 0)
            StartCoroutine("FadeBorderImage", imageAlpha);
    }

    public void FadeWhite()
    {
        StartCoroutine("FadeWhiteImage", 0.1f);
    }

    IEnumerator FadeWhiteImage(float fadeTimer)
    {
        yield return new WaitForSeconds(0.232f);
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.black, Color.white, fadeTimer);

        if (fadeTimer < 1)
            StartCoroutine("FadeWhiteImage", 0.1f + fadeTimer);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] GameObject tile;
    [SerializeField] int dimension = 4;
    [SerializeField] List<Sprite> tileSprites;
    [SerializeField] SpriteRenderer finishedImage;
    [SerializeField] Transform emptyTransform, startTransform;

    List<GameObject> tiles = new List<GameObject>();
    Vector3 emptyLocation, lowRightBounds;
    bool puzzleComplete = false;

    void Awake()
    {
        emptyLocation = emptyTransform.position;
        //emptyLocation = new Vector2(dimension - 1, -(dimension - 1));
        lowRightBounds = emptyLocation;

        for (int i = 0; i < dimension * dimension; i++)
        {
            GameObject newTile = Instantiate(tile, startTransform.position + new Vector3((float)(i % dimension) / 10, -(float)(i / dimension) / 10, 0), Quaternion.identity, transform);
            tiles.Add(newTile);
            //newTile.GetComponent<Tile>().AssignStartLocation(new Vector2(i % dimension, -i / 2));

            if (i == dimension * dimension - 1)
                newTile.GetComponent<SpriteRenderer>().color = Color.black;
            else
                newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[i];
        }

        //ScrambleTiles(dimension * dimension * 1.25f);
        //ScrambleTiles(2);
    }

    public Vector2 GetEmptyLocation()
    {
        return emptyLocation;
    }

    public void SetEmptyLocation(Vector2 input)
    {
        emptyLocation = input;
        tiles[tiles.Count - 1].transform.position = input;
    }

    void ScrambleTiles(float minDisplacement = 10)
    {
        ScrambleTiles(0, minDisplacement, 0);
    }

    void ScrambleTiles(float displacement, float minDisplacement, int counter)
    {
        if (displacement > minDisplacement)
        {
            Debug.Log(displacement);
            return;
        }

        if (counter > 100)
        {
            if (displacement > 0.9f * minDisplacement)
            {
                Debug.Log(displacement + " Early escape");
                return;
            }    
        }
        else if (counter > 200)
        {
            Debug.Log(displacement + " Escaped");
            return;
        }

        Collider[] possibleTiles = Physics.OverlapSphere(emptyLocation, 1);
        List<GameObject> closeTiles = new List<GameObject>();

        for (int i = 0; i < possibleTiles.Length; i++)
        {
            if (Vector2.Distance(possibleTiles[i].gameObject.transform.position, emptyLocation) <= 1)
                closeTiles.Add(possibleTiles[i].gameObject);
        }
        /*
        int randomNum = Random.Range(0, closeTiles.Count);
        closeTiles[randomNum].gameObject.GetComponent<Tile>().AttemptMove();
        counter++;

        ScrambleTiles(CountTotalDisplacement(), minDisplacement, counter);
        */

    }

    public float CountTotalDisplacement()
    {
        float totalDisplacement = 0;

        for (int i = 0; i < tiles.Count; i++)
            totalDisplacement += tiles[i].GetComponent<Tile>().GetDistanceFromStart();

        return totalDisplacement;
    }

    public void CheckFinished()
    {
        if (CountTotalDisplacement() == 0)
        {
            puzzleComplete = true;
            Debug.Log("You finished the puzzle!");
            StartCoroutine("DisplayFinishedImage", 0);
        }
    }

    public bool CheckPuzzleCompletion()
    {
        return puzzleComplete;
    }

    IEnumerator DisplayFinishedImage(int imageAlpha)
    {
        imageAlpha += 11;

        Color tempColor = finishedImage.color;
        tempColor.a = (float)imageAlpha / 255;
        finishedImage.color = tempColor;

        yield return new WaitForSeconds(0.1f);

        if (imageAlpha <= 255)
            StartCoroutine("DisplayFinishedImage", imageAlpha);
    }

    public void DisplayMessage()
    {
        Debug.Log("Hello");
    }
}
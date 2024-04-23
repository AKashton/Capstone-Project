using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] GameObject tile, tileHolder, replaySlate;
    [SerializeField] int dimension = 4;
    [SerializeField] List<Sprite> tileSprites;
    [SerializeField] SpriteRenderer finishedImage;
    [SerializeField] Transform emptyTransform, startTransform;
    [SerializeField] float castDistance;

    List<GameObject> tiles = new List<GameObject>();
    List<int> possibleMoveIndex = new List<int>();
    Vector3 emptyLocation;
    bool puzzleComplete = false;
    int emptyIndex = 15;
    int counter = 0;
    float minDisplacement = 6f;
    bool gameStarted = false;
    GameObject holder;
    SpriteRenderer lastTileSprite;

    void OnEnable()
    {
        emptyIndex = dimension * dimension - 1;
        holder = Instantiate(tileHolder, gameObject.transform.position, Quaternion.identity, transform);

        for (int i = 0; i < dimension * dimension; i++)
        {
            Vector3 tileLocation = startTransform.position + new Vector3((float)(i % dimension) / 5, -(float)(i / dimension) / 5, -0.02f);
            GameObject newTile = Instantiate(tile, tileLocation, Quaternion.identity, holder.transform);

            tiles.Add(newTile);
            newTile.GetComponent<Tile>().AssignStartLocation();

            newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[i];
        }

        emptyLocation = tiles[dimension * dimension - 1].transform.position;
        lastTileSprite = tiles[dimension * dimension - 1].GetComponent<SpriteRenderer>();
        lastTileSprite.color = Color.black;

        for (int i = 0; i < 2; i++)
        {
            if (!ScrambleTiles())
                break;
        }

        Debug.Log(CountTotalDisplacement());
        gameStarted = true;
    }

    public Vector3 GetEmptyLocation()
    {
        return emptyLocation;
    }

    public void SetEmptyLocation(Vector3 input)
    {
        emptyLocation = input;
        tiles[tiles.Count - 1].transform.position = input;
    }

    bool ScrambleTiles()
    {
        float displacement = CountTotalDisplacement();

        if (displacement > minDisplacement)
        {
            Debug.Log(displacement);
            return false;
        }

        if (counter > 100)
        {
            if (displacement > 0.9f * minDisplacement)
            {
                Debug.Log(displacement + " Early escape");
                return false;
            }
        }
        else if (counter > 200)
        {
            Debug.Log(displacement + " Escaped");
            return false;
        }

        possibleMoveIndex.Clear();

        if (emptyIndex >= dimension) // Empty tile is below top row
            possibleMoveIndex.Add(emptyIndex - 4);

        if (emptyIndex < (dimension - 1) * dimension) // Empty tile is above bottom row
            possibleMoveIndex.Add(emptyIndex + 4);

        if (emptyIndex % dimension > 0) // Empty tile is to right of leftmost column
            possibleMoveIndex.Add(emptyIndex - 1);

        if (emptyIndex % dimension < dimension - 1) // Empty tile is to left of rightmost column
            possibleMoveIndex.Add(emptyIndex + 1);

        int randomNeighborIndex = Random.Range(0, possibleMoveIndex.Count);

        SwapTiles(possibleMoveIndex[randomNeighborIndex], emptyIndex);

        return true;
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

        possibleMoveIndex.Clear();

        if (emptyIndex >= dimension) // Empty tile is below top row
            possibleMoveIndex.Add(emptyIndex - 4);

        if (emptyIndex < (dimension - 1) * dimension) // Empty tile is above bottom row
            possibleMoveIndex.Add(emptyIndex + 4);

        if (emptyIndex % dimension > 0) // Empty tile is to right of leftmost column
            possibleMoveIndex.Add(emptyIndex - 1);

        if (emptyIndex % dimension < dimension - 1) // Empty tile is to left of rightmost column
            possibleMoveIndex.Add(emptyIndex + 1);

        int randomNeighborIndex = Random.Range(0, possibleMoveIndex.Count);

        SwapTiles(possibleMoveIndex[randomNeighborIndex], emptyIndex);
        //Debug.Log(tiles[emptyIndex].GetComponent<Tile>().GetDistanceFromStart());

        counter++;

        ScrambleTiles(CountTotalDisplacement(), minDisplacement, counter);
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
        if (CountTotalDisplacement() < 0.001f)
        {
            gameStarted = false;
            //puzzleComplete = true;
            //lastTileSprite.color = Color.white;
            //Debug.Log("You finished the puzzle!");
            //StartCoroutine("DisplayFinishedImage", 0);

            lastTileSprite.gameObject.GetComponent<Tile>().FadeWhite();

            for (int i = 0; i < tiles.Count; i++)
                tiles[i].GetComponent<Tile>().FadeBorder();

            Invoke("OpenReplayScreen", 4);
        }
        else
            Debug.Log(CountTotalDisplacement());
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

    public void SwapTiles(int index1, int index2)
    {
        GameObject tempTile = tiles[index1];
        tiles[index1] = tiles[index2];
        tiles[index2] = tempTile;

        Vector3 tempPosition = tiles[index1].transform.position;
        tiles[index1].transform.position = tiles[index2].transform.position;
        tiles[index2].transform.position = tempPosition;

        emptyIndex = index1;

        if (gameStarted)
            CheckFinished();
    }

    public void CallTileSwap(GameObject inputObject)
    {
        if (!gameStarted)
            return;

        int calledTileIndex = tiles.IndexOf(inputObject);
        //Debug.Log($"Swapping {calledTileIndex} and {calledTileIndex + 1}");

        if (CheckEmptyNearby(calledTileIndex))
            SwapTiles(calledTileIndex, emptyIndex);
    }

    bool CheckEmptyNearby(int inputIndex)
    {
        if (inputIndex >= dimension) // Clicked tile is below top row
        {
            if (emptyIndex == inputIndex - dimension) // Empty is above clicked tile
                return true;
        }
        
        if (inputIndex < (dimension - 1) * dimension) // Clicked tile is above bottom row
        {
            if (emptyIndex == inputIndex + dimension) // Empty is below clicked tile
                return true;
        }

        if (inputIndex % dimension > 0) // Clicked tile is to right of leftmost column
        {
            if (emptyIndex == inputIndex - 1) // Empty tile is to left of clicked tile
                return true;
        }

        if (inputIndex % dimension < dimension - 1) // Clicked tile is to left of rightmost column
        {
            if (emptyIndex == inputIndex + 1) // Empty tile is to right of leftmost column
                return true;
        }

        return false;
    }

    void OpenReplayScreen()
    {
        Destroy(holder);
        replaySlate.SetActive(true);
        Invoke("CloseGame", 0.25f);
    }

    void CloseGame()
    {
        gameObject.SetActive(false);
    }
}
using MRTK.Tutorials.AzureCloudServices.Scripts.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PageManager : MonoBehaviour
{
    [System.Serializable]
    public class PointOfInterest
    {
        public enum nodeTypes { IMAGE, TEXT, GAME1, GAME2, GAME3, CONNECTING }
        public nodeTypes nodeType;
        public List<Sprite> nodeSprites;
        public string nextpointNumber;
        public float verticalOffset;
    }

    [System.Serializable]
    public class Location
    {
        public string mapLocation;
        public List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();
    }

    public static string MapLocation = "ADSAIL1";

    [SerializeField] List<Location> locations = new List<Location>();
    [SerializeField] List<Sprite> gameSprites;
    [SerializeField] GameObject objectController;
    [SerializeField] GameObject wolfGazeGame, puzzleGame;
    [SerializeField] GameObject rootMenu;
    [SerializeField] TMP_Dropdown locationDropdown;

    GameObject instantiatedGame;
    Vector3 spawnedLocation;

    public void ActivateController(bool input)
    {
        objectController.SetActive(input);
    }

    public Sprite GetGameSprite(int index)
    {
        return gameSprites[index];
    }

    public void EnableGazeGame()
    {
        rootMenu.SetActive(false);
        wolfGazeGame.SetActive(true);
    }

    public void EnablePuzzleGame(Vector3 spawnLocation)
    {
        spawnedLocation = spawnLocation;
        rootMenu.SetActive(false);
        instantiatedGame = Instantiate(puzzleGame, spawnLocation, Quaternion.identity);
    }

    public void RenewPuzzleGame()
    {
        Destroy(instantiatedGame);
        EnablePuzzleGame(spawnedLocation);
    }

    public void QuitPuzzleGame()
    {
        Destroy(instantiatedGame);
    }

    public MRTK.Tutorials.AzureCloudServices.Scripts.Domain.TrackedObject GetTrackedObject(int index)
    {
        GameObject[] tempObjects = GameObject.FindGameObjectsWithTag("Node");

        for (int i = 0; i < tempObjects.Length; i++)
        {
            MRTK.Tutorials.AzureCloudServices.Scripts.Domain.TrackedObject tempObject;
            tempObject = tempObjects[i].GetComponent<AnchorPosition>().CheckAnchorIntName(index);

            if (tempObject != null)
                return tempObject;
        }

        return null;
    }

    public void UpdateLocation()
    {
        MapLocation = locationDropdown.options[locationDropdown.value].text;
        Debug.Log(MapLocation);
    }

    public Location GetLocation()
    {
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].mapLocation == MapLocation)
                return locations[i];
        }

        return null;
    }
}

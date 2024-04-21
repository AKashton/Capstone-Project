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
    }

    [System.Serializable]
    public class Location
    {
        public string mapLocation;
        public List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();
    }

    public static string MapLocation = "ADSAIL1";

    [SerializeField] List<Location> locations = new List<Location>();
    [SerializeField] List<Sprite> wolfPages, wolfCircle, posterPages;
    [SerializeField] GameObject objectController;
    [SerializeField] GameObject wolfGazeGame;
    [SerializeField] GameObject rootMenu;
    [SerializeField] TMP_Dropdown locationDropdown;
    //[SerializeField] MRTK.Tutorials.AzureCloudServices.Scripts.Controller.ObjectCardViewController viewController;

    /*string s1 = "hellothere", s2 = "hello";
    private void Awake()
    {
        int stringIndex = s1.IndexOf(s2);
        s1 = s1.Remove(stringIndex, s2.Length);
        Debug.Log(s1);
    }*/

    /*
    public int GetNumberWolfPages()
    {
        return wolfPages.Count;
    }

    public int GetNumberPosterPages()
    {
        return posterPages.Count;
    }

    public Sprite GetWolfPage(int index)
    {
        return wolfPages[index];
    }

    public Sprite GetWolfCircle()
    {
        return wolfCircle[0];
    }

    public Sprite GetPosterPage(int index)
    {
        return posterPages[index];
    }
    */

    public void ActivateController(bool input)
    {
        objectController.SetActive(input);
    }

    public void EnableGazeGame()
    {
        rootMenu.SetActive(false);
        wolfGazeGame.SetActive(true);
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

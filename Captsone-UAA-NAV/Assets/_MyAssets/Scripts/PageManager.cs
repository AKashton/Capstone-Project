using MRTK.Tutorials.AzureCloudServices.Scripts.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.EditorUtilities;

public class PageManager : MonoBehaviour
{
    public static string MapLocation = "ADSAIL1";

    [SerializeField] List<Sprite> wolfPages, wolfCircle, posterPages;
    [SerializeField] GameObject objectController;
    [SerializeField] GameObject wolfGazeGame;
    [SerializeField] GameObject rootMenu;
    [SerializeField] TMP_Dropdown locationDropdown;
    //[SerializeField] MRTK.Tutorials.AzureCloudServices.Scripts.Controller.ObjectCardViewController viewController;

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
}

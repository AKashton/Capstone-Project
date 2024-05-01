// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT License.

using System.Threading.Tasks;
using MixedReality.Toolkit;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MRTK.Tutorials.AzureCloudServices.Scripts.Controller
{
    public class ObjectEntryController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] SceneController sceneController;
        [SerializeField] GameObject searchObjectPanel = default;
        [SerializeField] ObjectEditController objectEditPanel = default;
        [SerializeField] ObjectCardViewController objectCardPrefab = default;
        [Header("UI Elements")]
        [SerializeField] TMP_Text submitButtonLabel = default;
        [SerializeField] TMP_Text hintLabel = default;
        [SerializeField] Text inputField = default;
        [SerializeField] string loadingText = "Please wait...";
        [SerializeField] StatefulInteractable[] buttons = default;
        
        bool isInSearchMode;
        
        void Awake()
        {
            if (sceneController == null)
                sceneController = FindObjectOfType<SceneController>();
        }

        void OnEnable()
        {
            inputField.text = string.Empty;
        }

        public void SetSearchMode(bool searchModeActive)
        {
            isInSearchMode = searchModeActive;
            submitButtonLabel.SetText(isInSearchMode ? "Search Object" : "Set Object");
        }

        public async void FindAllAnchors()
        {
            SetButtonsInteractiveState(false);

            var project0 = FindObject(PageManager.MapLocation + "0");

            /*
            if (project0 != null)
            {
                searchObjectPanel.SetActive(false);
                var objectCard = Instantiate(objectCardPrefab, transform.position, transform.rotation);
                objectCard.InitAndFind(project0);
            }
            */

            var project1 = FindObject(PageManager.MapLocation + "1");

            /*
            if (project1 != null)
            {
                searchObjectPanel.SetActive(false);
                var objectCard = Instantiate(objectCardPrefab, transform.position, transform.rotation);
                objectCard.InitAndFind(project1);
            }
            */

            var project2 = FindObject(PageManager.MapLocation + "2");

            /*
            if (project2 != null)
            {
                searchObjectPanel.SetActive(false);
                var objectCard = Instantiate(objectCardPrefab, transform.position, transform.rotation);
                objectCard.InitAndFind(project2);
            }
            */

            searchObjectPanel.SetActive(false);
            await Task.WhenAll(project0, project1, project2);

            var objectCard0 = Instantiate(objectCardPrefab, transform.position, transform.rotation);
            objectCard0.InitAndFind(project0.Result);

            var objectCard1 = Instantiate(objectCardPrefab, transform.position, transform.rotation);
            objectCard1.InitAndFind(project1.Result);

            var objectCard2 = Instantiate(objectCardPrefab, transform.position, transform.rotation);
            objectCard2.InitAndFind(project2.Result);

            SetButtonsInteractiveState(true);
        }

        public async void SubmitQuery()
        {
            if (string.IsNullOrWhiteSpace(inputField.text))
            {
                hintLabel.SetText("Please type in a name.");
                hintLabel.gameObject.SetActive(true);
                return;
            }

            if (!sceneController.DataManager.IsReady)
            {
                hintLabel.SetText("No connection to the database!");
                hintLabel.gameObject.SetActive(true);
                return;
            }

            SetButtonsInteractiveState(false);

            if (isInSearchMode)
            {
                var project = await FindObject(PageManager.MapLocation + inputField.text);

                if (project != null)
                {
                    searchObjectPanel.SetActive(false);
                    var objectCard = Instantiate(objectCardPrefab, transform.position, transform.rotation);
                    objectCard.Init(project);
                }
            }
            else
            {
                var project = await CreateObject(PageManager.MapLocation + inputField.text);

                if (project != null)
                {
                    searchObjectPanel.SetActive(false);
                    objectEditPanel.gameObject.SetActive(true);
                    objectEditPanel.Init(project);
                }
            }

            SetButtonsInteractiveState(true);
        }

        public async void SubmitQuery(int pageID)
        {
            var project = await FindObject(pageID.ToString());

            if (project != null)
            {
                var objectCard = Instantiate(objectCardPrefab, transform.position, transform.rotation);
                objectCard.Init(project);

                objectCard.StartFindLocation();
            }
        }

        async Task<TrackedObject> FindObject(string searchName)
        {
            hintLabel.SetText(loadingText);
            hintLabel.gameObject.SetActive(true);

            var projectFromDb = await sceneController.DataManager.FindTrackedObjectByName(searchName);

            if (projectFromDb == null)
            {
                hintLabel.SetText($"No object found with the name '{searchName}'.");
                return null;
            }

            hintLabel.gameObject.SetActive(false);
            return projectFromDb;
        }

        async Task<TrackedObject> CreateObject(string searchName)
        {
            hintLabel.SetText(loadingText);
            hintLabel.gameObject.SetActive(true);

            var trackedObject = await sceneController.DataManager.FindTrackedObjectByName(searchName);

            if (trackedObject == null)
            {
                trackedObject = new TrackedObject(searchName);
                var success = await sceneController.DataManager.UploadOrUpdate(trackedObject);

                if (!success)
                    return null;
            }

            hintLabel.gameObject.SetActive(false);
            return trackedObject;
        }
        
        void SetButtonsInteractiveState(bool state)
        {
            foreach (var interactable in buttons)
                interactable.enabled = state;
        }
    }
}

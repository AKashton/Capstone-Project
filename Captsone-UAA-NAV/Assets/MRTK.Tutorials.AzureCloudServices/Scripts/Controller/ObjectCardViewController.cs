// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using MixedReality.Toolkit;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MRTK.Tutorials.AzureCloudServices.Scripts.Controller
{
    public class ObjectCardViewController : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private SceneController sceneController;
        [Header("UI")]
        [SerializeField] private TMP_Text objectNameLabel = default;
        [SerializeField] private TMP_Text descriptionLabel = default;
        [SerializeField] private TMP_Text messageLabel = default;
        [SerializeField] private Image thumbnailImage = default;
        [SerializeField] private Sprite thumbnailPlaceHolderImage = default;
        [SerializeField] private StatefulInteractable[] buttons = default;
        
        private TrackedObject trackedObject;
        
        private void Awake()
        {
            if (sceneController == null)
                sceneController = FindObjectOfType<SceneController>();
        }
        
        private void OnDisable()
        {
            sceneController.OpenMainMenu();
        }

        public void InitAndFind(TrackedObject source)
        {
            if (sceneController == null)
                sceneController = FindObjectOfType<SceneController>();

            trackedObject = source;
            objectNameLabel.SetText(trackedObject.Name);
            descriptionLabel.text = trackedObject.Description;

            sceneController.StopCamera();
            sceneController.AnchorManager.OnFindAnchorSucceeded += HandleOnAnchorFound;
            sceneController.AnchorManager.FindAnchor(trackedObject);

        }

        public async void Init(TrackedObject source)
        {
            if (sceneController == null)
                sceneController = FindObjectOfType<SceneController>();
            
            trackedObject = source;
            objectNameLabel.SetText(trackedObject.Name);
            descriptionLabel.text = trackedObject.Description;
            
            if (!string.IsNullOrEmpty(trackedObject.ThumbnailBlobName))
                thumbnailImage.sprite = await LoadThumbnailImage();
            else
                thumbnailImage.sprite = thumbnailPlaceHolderImage;
        }

        public void StartFindLocation()
        {
            if (string.IsNullOrEmpty(trackedObject.SpatialAnchorId))
            {
                messageLabel.text = "No spatial anchor has been specified for this object.";
                return;
            }

            if (sceneController.AnchorManager.CheckIsAnchorActiveForTrackedObject(trackedObject.SpatialAnchorId))
            {
                messageLabel.text = "The spatial anchor for this object is already spawned.";
                sceneController.AnchorManager.GuideToAnchor(trackedObject.SpatialAnchorId);
                return;
            }
            
            sceneController.StopCamera();
            sceneController.AnchorManager.OnFindAnchorSucceeded += HandleOnAnchorFound;
            sceneController.AnchorManager.FindAnchor(trackedObject);
        }
        /*
        public void StartFindLocation(int pageID)
        {
            if (sceneController.AnchorManager.CheckIsAnchorActiveForTrackedObject(trackedObject.SpatialAnchorId))
            {
                messageLabel.text = "The spatial anchor for this object is already spawned.";
                sceneController.AnchorManager.GuideToAnchor(trackedObject.SpatialAnchorId);
                return;
            }

            sceneController.StopCamera();
            sceneController.AnchorManager.OnFindAnchorSucceeded += HandleOnAnchorFound;
            sceneController.AnchorManager.FindAnchor(trackedObject);
        }
        */
        private void HandleOnAnchorFound(object sender, EventArgs e)
        {
            Debug.Log("ObjectCardViewController.HandleOnAnchorFound");
            sceneController.AnchorManager.OnFindAnchorSucceeded -= HandleOnAnchorFound;
            SetButtonsInteractiveState(true);
        }

        public void CloseCard()
        {
            messageLabel.text = string.Empty;
            sceneController.OpenMainMenu();
            Destroy(gameObject);
        }

        private async Task<Sprite> LoadThumbnailImage()
        {
            var imageData = await sceneController.DataManager.DownloadBlob(trackedObject.ThumbnailBlobName);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        private void SetButtonsInteractiveState(bool state)
        {
            foreach (var interactable in buttons)
                interactable.enabled = state;
        }
    }
}

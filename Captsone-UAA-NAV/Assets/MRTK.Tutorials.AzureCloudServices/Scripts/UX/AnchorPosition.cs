// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT License.

using System.Collections;
using MRTK.Tutorials.AzureCloudServices.Scripts.Controller;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace MRTK.Tutorials.AzureCloudServices.Scripts.UX
{
    /// <summary>
    /// Handles the anchor position visual.
    /// </summary>
    public class AnchorPosition : MonoBehaviour
    {
        public TrackedObject TrackedObject => trackedObject;

        [SerializeField] private ObjectCardViewController objectCard = default;
        [SerializeField] SpriteRenderer spriteRenderer;

        private TrackedObject trackedObject;
        PageManager pageManager;
        int pictureIndex = 0;

        void Awake()
        {
            pageManager = GameObject.FindWithTag("PageManager").GetComponent<PageManager>();
        }

        public void Init(TrackedObject source)
        {
            trackedObject = source;
            //Workaround because TextMeshPro label is not ready until next frame
            StartCoroutine(DelayedInitCoroutine());
        }
        
        private IEnumerator DelayedInitCoroutine()
        {
            yield return null;

            if (trackedObject != null)
            {
                if (int.Parse(trackedObject.Name) == 0)
                    spriteRenderer.sprite = pageManager.GetWolfPage(0);
                else if (int.Parse(trackedObject.Name) == 1)
                    spriteRenderer.sprite = pageManager.GetWolfCircle();
                else if (int.Parse(trackedObject.Name) == 2)
                    spriteRenderer.sprite = pageManager.GetPosterPage(0);

                objectCard.Init(trackedObject);
            }
        }

        public void EngagePosition()
        {
            if (int.Parse(trackedObject.Name) == 0)
            {
                pictureIndex = (pictureIndex + 1) % pageManager.GetNumberWolfPages();
                spriteRenderer.sprite = pageManager.GetWolfPage(pictureIndex);
            }
            else if (int.Parse(trackedObject.Name) == 1)
                pageManager.EnableGazeGame();
            else
            {
                pictureIndex = (pictureIndex + 1) % pageManager.GetNumberPosterPages();
                spriteRenderer.sprite = pageManager.GetPosterPage(pictureIndex);
            }
        }

        public void FindNextPosition()
        {
            TrackedObject nextObject;

            if (int.Parse(trackedObject.Name) == 0)
                nextObject = pageManager.GetTrackedObject(1);
            else if (int.Parse(trackedObject.Name) == 1)
                nextObject = pageManager.GetTrackedObject(2);
            else
                nextObject = pageManager.GetTrackedObject(0);

            if (nextObject == null)
                return;

            AnchorManager anchorManager = GameObject.FindWithTag("AnchorManager").GetComponent<AnchorManager>();
            anchorManager.GuideToAnchor(nextObject.SpatialAnchorId);
            //anchorManager.FindAnchor(nextObject);

            //pageManager.ActivateController(true);
            //FindObjectOfType<ObjectEntryController>().SubmitQuery((int.Parse(trackedObject.Name) + 1) % 3 + 3); // 3 nodes for demo
        }

        public TrackedObject CheckAnchorIntName(int input)
        {
            if (int.Parse(trackedObject.Name) == input)
                return trackedObject;
            else
                return null;
        }
    }
}

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

        [SerializeField] ObjectCardViewController objectCard = default;
        [SerializeField] SpriteRenderer spriteRenderer;

        TrackedObject trackedObject;
        PageManager pageManager;
        PageManager.PointOfInterest pointOfInterest;
        int pictureIndex = 0;
        string objectLocation;
        public string anchorID { get; set; }

        void Awake()
        {
            pageManager = GameObject.FindWithTag("PageManager").GetComponent<PageManager>();
        }

        public async void GetTrackedSource()
        {
            trackedObject = await GameObject.FindWithTag("DataManager").GetComponent<DataManager>().FindTrackedObjectById(anchorID);
            StartCoroutine(DelayedInitCoroutine());
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
                string s1 = trackedObject.Name, s2 = PageManager.MapLocation;
                int stringIndex = s1.IndexOf(s2);
                s1 = s1.Remove(stringIndex, s2.Length);

                pointOfInterest = pageManager.GetLocation().pointsOfInterest[int.Parse(s1)];

                if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.TEXT)
                {
                    spriteRenderer.enabled = false;
                    // Enable text panel object
                    // Set default text
                }
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.CONNECTING)
                    spriteRenderer.enabled = false;
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME1)
                    spriteRenderer.sprite = pageManager.GetGameSprite(0);
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME2)
                    spriteRenderer.sprite = pageManager.GetGameSprite(1);
                else
                    spriteRenderer.sprite = pointOfInterest.nodeSprites[0];

                objectCard.Init(trackedObject);
            }
        }

        public void EngagePosition()
        {
            if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.IMAGE)
            {
                pictureIndex = (pictureIndex + 1) % pointOfInterest.nodeSprites.Count;
                spriteRenderer.sprite = pointOfInterest.nodeSprites[pictureIndex];
            }
            else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME1)
                pageManager.EnableGazeGame();
            else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME2)
                pageManager.EnablePuzzleGame();
        }

        public void FindNextPosition()
        {
            TrackedObject nextObject;

            nextObject = pageManager.GetTrackedObject(int.Parse(pointOfInterest.nextpointNumber));

            if (nextObject == null)
                return;

            AnchorManager anchorManager = GameObject.FindWithTag("AnchorManager").GetComponent<AnchorManager>();
            anchorManager.GuideToAnchor(nextObject.SpatialAnchorId);
        }

        public TrackedObject CheckAnchorIntName(int input)
        {
            if (trackedObject.Name == PageManager.MapLocation + input.ToString())
                return trackedObject;
            else
                return null;
        }
    }
}

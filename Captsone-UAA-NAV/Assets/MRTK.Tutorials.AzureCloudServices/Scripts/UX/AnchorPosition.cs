﻿// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT License.

using System.Collections;
using MRTK.Tutorials.AzureCloudServices.Scripts.Controller;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;

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
        [SerializeField] MeshRenderer orbIndicator;
        [SerializeField] GameObject engageButton, advanceButton;

        TrackedObject trackedObject;
        PageManager pageManager;
        PageManager.PointOfInterest pointOfInterest;
        int pictureIndex = 0;
        public string anchorID { get; set; }

        void Awake()
        {
            pageManager = GameObject.FindWithTag("PageManager").GetComponent<PageManager>();
        }

        public async void GetTrackedSource()
        {
            trackedObject = await GameObject.FindWithTag("DataManager").GetComponent<DataManager>().FindTrackedObjectBySpatialID(anchorID);
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

                if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.IMAGE)
                {
                    spriteRenderer.sprite = pointOfInterest.nodeSprites[0];
                    spriteRenderer.gameObject.transform.position += pointOfInterest.verticalOffset * Vector3.up;
                }
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.TEXT)
                {
                    spriteRenderer.enabled = false;
                    // Enable text panel object
                    // Set default text
                }
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.CONNECTING)
                {
                    spriteRenderer.enabled = false;
                    engageButton.SetActive(false);
                }
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME1)
                    spriteRenderer.sprite = pageManager.GetGameSprite(0);
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME2)
                    spriteRenderer.sprite = pageManager.GetGameSprite(1);
                else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.NULL)
                    gameObject.SetActive(false);
                else
                    spriteRenderer.sprite = pageManager.GetGameSprite(0);

                objectCard.Init(trackedObject);
            }
        }

        public void EngagePosition()
        {
            //GameObject.FindWithTag("AnchorManager").GetComponent<AnchorManager>().DisplayAssignedID(anchorID);

            
            if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.IMAGE)
            {
                pictureIndex = (pictureIndex + 1) % pointOfInterest.nodeSprites.Count;
                spriteRenderer.sprite = pointOfInterest.nodeSprites[pictureIndex];
            }
            else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME1)
                pageManager.EnableGazeGame();
            else if (pointOfInterest.nodeType == PageManager.PointOfInterest.nodeTypes.GAME2)
                pageManager.EnablePuzzleGame(transform.position);
            
        }

        public void FindNextPosition()
        {
            TrackedObject nextObject = pageManager.GetTrackedObject(int.Parse(pointOfInterest.nextpointNumber));

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

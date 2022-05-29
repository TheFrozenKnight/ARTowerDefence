using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    private ARRaycastManager arOrigin;
    private ARPlaneManager m_ARPlaneManager;

    Camera arCam;
    List<ARRaycastHit> m_hits = new List<ARRaycastHit>();
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool placed = false;

    void Start()
    {
        arOrigin = GetComponent<ARRaycastManager>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    void Update()
    {
        /*
        if (placed == true)
        {
            if (Input.touchCount == 0)
                return;

            RaycastHit hit;
            Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

            if (arOrigin.Raycast(Input.GetTouch(0).position, m_hits))
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if(Physics.Raycast(ray, out hit))
                    {
                        if(hit.collider.gameObject.CompareTag("node"))
                        {
                            hit.collider.gameObject.GetComponent<Node>().clicked();
                        }
                    }
                }
            }
        }
        */

        if (placed == false)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();

            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                placed = true;

                foreach (var plane in m_ARPlaneManager.trackables)
                    plane.gameObject.SetActive(false);
                m_ARPlaneManager.enabled = false;
                PlaceObject();
            }
        }

    }
    private void PlaceObject()
    {
        placementIndicator.gameObject.SetActive(false);
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (!placed)
        {
            if (placementPoseIsValid)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            }
            else
            {
                placementIndicator.SetActive(false);
            }
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; //Needed to access AR Foundation functionalities.
//using UnityEngine.Experimental.XR; //You won't need this (in case you followed the youtube tutorial)


public class ARHoldToDraw : MonoBehaviour
{
    private Camera mainCam;

    //AR
    private ARSessionOrigin aRSessionOrigin;
    private ARRaycastManager raycastManager;
    private bool placementPoseIsValid = false; //whether an object can be placed in a certain spot
    private Pose placementPose; //this stores a Pose


    //Objects
    public GameObject placementIndicator; //Don't forget to set this in the inspector
    public GameObject objectToPaintWith; //Don't forget to set this in the inspector
    private GameObject objectToPaintWithVisualizer;

    //Controlls variables
    private float distanceFromFloor = 1f;
    //public Slider sliderDistanceFromFloor;
    private float painterSphereSize = 1f;
    //public Slider sliderPainterSphereSize;
    private Vector3 newScaleForPaintObject; 



    void Start()
    {
        mainCam = FindObjectOfType<Camera>(); //You should only have one camera in the scene for this test
        aRSessionOrigin = FindObjectOfType<ARSessionOrigin>(); // creating a reference to ARSessionOrigin
        raycastManager = aRSessionOrigin.GetComponent<ARRaycastManager>(); //need this to perform raycasts to any "AR trackable". It should be in the same game object as ARSessionOrigin

        objectToPaintWithVisualizer = Instantiate(objectToPaintWith, Vector3.zero, Quaternion.identity); //will use this to visualize the brush were are using

    }

    void Update()
    {
        newScaleForPaintObject = Vector3.one * painterSphereSize;
        objectToPaintWithVisualizer.transform.localScale = newScaleForPaintObject;

        UpdatePlacementPose();
        UpdatePlacementIndicator();

        //If we have a valid pose and the user is touching the screen and the touch just began
        //then paint/place object in the scene
        //if you want to hold to paint, remove " && Input.GetTouch(0).phase == TouchPhase.Began" from the if statment
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    //Send a ray from the center of the screen in the camera direction
    //and see if it hits any Trackable Planes. If it does:
    //set the placementIsValid to true and
    //set the placementPose to the first pose in the hit list
    private void UpdatePlacementPose()
    {
        Vector3 screenCenter = mainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }

    //If the pose is valid, turn on the visualizer and the brush
    //and place them at the pose
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid) 
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

            objectToPaintWithVisualizer.SetActive(true);
            objectToPaintWithVisualizer.transform.SetPositionAndRotation(placementPose.position + placementPose.up * distanceFromFloor, placementPose.rotation);
        }
        else
        {
            objectToPaintWithVisualizer.SetActive(false);
            placementIndicator.SetActive(false);
        }
    }

    ////This will be called from a UI Button later on
    //public void ClickToPaint()
    //{
    //    if (placementPoseIsValid)
    //        PlaceObject();
    //}

    private void PlaceObject()
    {
        GameObject placedPaintObject = Instantiate(objectToPaintWith, 
            placementPose.position + placementPose.up * distanceFromFloor, 
            placementPose.rotation);
        placedPaintObject.transform.localScale = newScaleForPaintObject;
    }
}

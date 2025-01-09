using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[Serializable]
public struct CameraPositionDetails 
{
    //set position name, not used programmatically but allows
    //for easier identifying when looking at string
    public string PositionName;

    //target gameobject to move towards
    //usign gameobject instead of transform allows the camera to follow an object
    //instead of being locked to set transforms
    public GameObject targetPosition;

    //number of second to move to this set position
    public float transitionTimeInSeconds;

    //animation curve used to add in animation effects
    public AnimationCurve lerpAnimationCurve;
};


public class DynamicMenuManager : MonoBehaviour
{
    [Header("Camera Animation & Positions")]
    //list of animations for the camera to move between
    [SerializeField] private CameraPositionDetails[] cameraAnimationDetails;

    [Header("References")]
    //Main Camera
    [SerializeField] private Camera menuCamera;

    [Header("Values")]
    //Set the first position camera will move to
    [SerializeField] private int defaultMainScreenIndex = 0;
    //set bool on or off to allow updating camera position every frame regardless of journey status in animation
    //set to true will allow camera to follow object, set to false when only static and save some resources
    [SerializeField] private bool updateCameraPositionAfterAnimationCompleted = true;



    //Private variables
    //not a const but kept same convention as it is intended not to change
    private AnimationCurve DEFAULT_ANIMATION_CURVE = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    private float journey = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Safety checks to ensure the user doesn't accidentally cause errors without knowing why
        /////////
        //check if camera is assigned, if not assign main camera
        if (menuCamera == null) menuCamera = Camera.main;

        //reset camera starting position if outside of array bounds
        if (defaultMainScreenIndex >= cameraAnimationDetails.Length)
        {
            Debug.LogWarning("Default camera index outside of camera animation details array bounds, Reverting to position 0");
            defaultMainScreenIndex = 0;
        }

        //Show warning if there is no camera positions set
        if (cameraAnimationDetails.Length == 0)
        {
            Debug.LogError("Camera has no set animation positions or details");
            //set active to false to stop any code from running that may cause more errors
            gameObject.SetActive(false);
            return;
        }

        ////////

        //set default animation curve
        DEFAULT_ANIMATION_CURVE = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        //set camera in starting position
        menuCamera.transform.position = cameraAnimationDetails[0].targetPosition.transform.position;
        menuCamera.transform.rotation = cameraAnimationDetails[0].targetPosition.transform.rotation;
    }



    private void LateUpdate()
    {
        
    }



    public void ReturnCameraToMain()
    {

    }



    public void MoveCameraToPosition(int cameraPosition)
    {

    }









    //animate to position using camera position details
    public void AnimateToPosition(CameraPositionDetails positionDetails)
    {

    }



    
    //Allow custom animation to position using animation struct


    public void AnimateToPosition(GameObject inTargetPosition, float inTransitionTimeInSeconds, AnimationCurve inLerpAnimationCurve)
    {
        //create camera details from input information
        CameraPositionDetails newTargetPosition = new CameraPositionDetails
        {
            targetPosition = inTargetPosition,
            transitionTimeInSeconds = inTransitionTimeInSeconds,
            lerpAnimationCurve = inLerpAnimationCurve
        };

        //animate camera to position
        AnimateToPosition(newTargetPosition);
    }

    public void AnimateToPosition(GameObject inTargetPosition, float inTransitionTimeInSeconds)
    {
        //create camera details from input information
        CameraPositionDetails newTargetPosition = new CameraPositionDetails
        {
            targetPosition = inTargetPosition,
            transitionTimeInSeconds = inTransitionTimeInSeconds,
            lerpAnimationCurve = DEFAULT_ANIMATION_CURVE
        };

        //animate camera to position
        AnimateToPosition(newTargetPosition);
    }




    //Allow custom animation to position using a transform, move this object and use it as the target


    public void AnimateToPosition(Transform inTargetPosition, float inTransitionTimeInSeconds, AnimationCurve inLerpAnimationCurve)
    {
        //move this game object to input transform
        this.transform.SetPositionAndRotation(inTargetPosition.position, inTargetPosition.rotation);

        //create animation detail using this as the target game object
        CameraPositionDetails newTargetPosition = new CameraPositionDetails
        {
            targetPosition = gameObject,
            transitionTimeInSeconds = inTransitionTimeInSeconds,
            lerpAnimationCurve = inLerpAnimationCurve
        };

        //animate to position
        AnimateToPosition(newTargetPosition);
    }

    public void AnimateToPosition(Transform inTargetPosition, float inTransitionTimeInSeconds)
    {
        //move this game object to input transform
        this.transform.SetPositionAndRotation(inTargetPosition.position, inTargetPosition.rotation);

        //create animation detail using this as the target game object
        CameraPositionDetails newTargetPosition = new CameraPositionDetails
        {
            targetPosition = gameObject,
            transitionTimeInSeconds = inTransitionTimeInSeconds,
            lerpAnimationCurve = DEFAULT_ANIMATION_CURVE
        };
        
        //animte camera to position
        AnimateToPosition(newTargetPosition);
    }
}

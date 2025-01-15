using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Events;

[Serializable]



public class DynamicMenuManager : MonoBehaviour
{
    public static DynamicMenuManager _instance;

    //create events that are called on begin and on finished of camera animation
    [HideInInspector]
    public UnityEvent<DynamicMenuCameraTarget> Event_CameraAnimationStarted;
    [HideInInspector]
    public UnityEvent<DynamicMenuCameraTarget> Event_CameraAnimationMidCall;
    [HideInInspector]
    public UnityEvent<DynamicMenuCameraTarget> Event_CameraAnimationFinished;


    [Header("References")]
    //Main Camera
    [SerializeField] private Camera menuCamera;
    [SerializeField] private DynamicMenuCameraTarget defaultMainScreenPosition;

    [Header("Values")]
    //set bool on or off to allow updating camera position every frame regardless of journey status in animation
    //set to true will allow camera to follow object, set to false when only static and save some resources
    [SerializeField] private bool updateCameraPositionAfterAnimationCompleted = true;

    //Private variables
    //animation details
    //journey of animation between 0 and 1
    private float animationJourney = 1;

    //starting position of animation
    private Vector3 currentStartingPosition;
    private Quaternion currentStartingRotation;

    //ending position of animation
    private DynamicMenuCameraTarget currentTargetPosition;

    //bool to say if animation is completed
    private bool animationCompleted;
    private bool midAnimationEventCompleted;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //Safety checks to ensure the user doesn't accidentally cause errors without knowing why
        /////////
        //check if camera is assigned, if not assign main camera
        if (DynamicMenuManager._instance != null && DynamicMenuManager._instance != this)
        {
            Destroy(gameObject);
        }
        _instance = this;

        if (menuCamera == null) menuCamera = Camera.main;

        //reset camera starting position if outside of array bounds
        if (defaultMainScreenPosition == null)
        {
            Debug.LogWarning("Starting camera position is not set, camera will start in its current placed position");
        }
        else
        {
            //set camera in starting position
            menuCamera.transform.position = defaultMainScreenPosition.transform.position;
            menuCamera.transform.rotation = defaultMainScreenPosition.transform.rotation;
        }

        //testing remove
        //AnimateToPosition(1);
    }

    //update after physics and other updates to keep the camera movement smooth
    private void LateUpdate()
    {
        //check if the camera should update every frame
        if (!updateCameraPositionAfterAnimationCompleted && animationJourney == 1 || currentTargetPosition == null) return;
        
        //increment time of lerp based on time
        animationJourney = Mathf.Clamp(animationJourney + Time.deltaTime/currentTargetPosition.transitionTimeInSeconds, 0, 1);        

        //move camera to position
        //menuCamera.transform.position = Vector3.Lerp(currentStartingPosition.position, currentTargetPosition.targetPosition.transform.position, updatedJourneyFromAnimationCurve);
        menuCamera.transform.position = Vector3.LerpUnclamped(currentStartingPosition, currentTargetPosition.transform.position, currentTargetPosition.GetUpdatedJourneyFromCurve(animationJourney));
        menuCamera.transform.rotation = Quaternion.LerpUnclamped(currentStartingRotation, currentTargetPosition.transform.rotation, currentTargetPosition.GetUpdatedRotationJourneyFromCurve(animationJourney));

        //complete the animation journey
        if (animationJourney >= 1 && !animationCompleted)
        {
            animationJourney = 1;
            animationCompleted = true;
            Event_CameraAnimationFinished.Invoke(currentTargetPosition);
        }

        //call the mid animation event based on the targets set time
        if (animationJourney >= currentTargetPosition.MiddleAnimationEventTime && !midAnimationEventCompleted)
        {
            Event_CameraAnimationMidCall.Invoke(currentTargetPosition);
            midAnimationEventCompleted = true;
        }
    }

    /// <summary>
    /// returns the camera to the set starting/main position
    /// </summary>
    public void ReturnCameraToMain()
    {
        AnimateToPosition(defaultMainScreenPosition);
    }

    /// <summary>
    /// Animate the camera to the passed in position details
    /// </summary>
    /// <param name="positionDetails"></param>
    public void AnimateToPosition(DynamicMenuCameraTarget positionDetails)
    {
        currentTargetPosition = positionDetails;
        //set up event data
        Event_CameraAnimationStarted.Invoke(currentTargetPosition);

        //reset journey to 0
        animationJourney = 0;
        SetStartingTransform(menuCamera.transform);
        animationCompleted = false;
        midAnimationEventCompleted = false;
    }

    /// <summary>
    /// sets starting position and rotation from input transform
    /// </summary>
    /// <param name="newTransform"></param>
    public void SetStartingTransform(Transform newTransform)
    {
        currentStartingPosition = newTransform.position;
        currentStartingRotation = newTransform.rotation;
    }
}

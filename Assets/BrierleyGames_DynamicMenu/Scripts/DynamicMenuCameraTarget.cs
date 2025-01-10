using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class DynamicMenuCameraTarget : MonoBehaviour
{
    public enum soundType
    {
        Open,
        Close,
        Other,
        Custom
    }
    //number of second to move to this set position
    public float transitionTimeInSeconds;

    [Range(0.0f, 1.0f)]
    public float MiddleAnimationEventTime = 0.5f;

    //Select sound enum to create simple consistent audio
    [Header("Sounds")]
    public soundType selectSoundType;

    //or choose custom and add sounds below
    [Header("Add sounds if set as custom above")]
    public AudioClip[] sounds;

    ////Hide below as it is shown with custom GUI later
    [HideInInspector]
    //animation curve used to add in animation effects
    public AnimationCurve lerpAnimationCurve;
    [HideInInspector]
    //bool to allow seperate RotationCurve
    public bool seperateRotationCurve;
    [HideInInspector]
    //animation curve used to add in animation effects
    public AnimationCurve lerpRotationCurve;
    //////////
    ///


    [HideInInspector]
    public UnityEvent Event_AnimationStart;
    [HideInInspector]
    public UnityEvent Event_AnimationMid;
    [HideInInspector]
    public UnityEvent Event_AnimationEnd;
    [HideInInspector]
    public UnityEvent Event_HideCanvasPanel;

    /// <summary>
    /// Call on start, subscribe to all animation events
    /// </summary>
    private void Start()
    {
        DynamicMenuManager._instance.Event_CameraAnimationStarted.AddListener(AnimationStartedEventReader);
        DynamicMenuManager._instance.Event_CameraAnimationMidCall.AddListener(AnimationMiddleEventReader);
        DynamicMenuManager._instance.Event_CameraAnimationFinished.AddListener(AnimationFinishedEventReader);
    }

    /// <summary>
    /// Call on destroy, unsubscribe to all animation events
    /// </summary>
    private void OnDestroy()
    {
        DynamicMenuManager._instance.Event_CameraAnimationStarted.RemoveListener(AnimationStartedEventReader);
        DynamicMenuManager._instance.Event_CameraAnimationMidCall.RemoveListener(AnimationMiddleEventReader);
        DynamicMenuManager._instance.Event_CameraAnimationFinished.RemoveListener(AnimationFinishedEventReader);
    }


    /// <summary>
    /// calls the dynamic menu manager to move the camera to this objects position
    /// </summary>
    public void AnimateCameraToThisPoint()
    {
        DynamicMenuManager._instance.AnimateToPosition(this);
    }

    /// <summary>
    /// Called when the camera movement animation starts
    /// </summary>
    /// <param name="completedTarget">target camera position</param>
    public void AnimationStartedEventReader(DynamicMenuCameraTarget animationTarget)
    {
        if (animationTarget != this)
        {
            //close listening canvas panel(s)
            Event_HideCanvasPanel.Invoke();
            return;
        }
        //call animation start
        Event_AnimationStart.Invoke();
    }

    /// <summary>
    /// Called when the camera movement animation hits the specified midPoint
    /// </summary>
    /// <param name="completedTarget">target camera position</param>
    public void AnimationMiddleEventReader(DynamicMenuCameraTarget completedTarget)
    {
        if (completedTarget != this) return;

        //call animation mid
        Event_AnimationMid.Invoke();
    }

    /// <summary>
    /// Called when the camera movement animation finishes
    /// </summary>
    /// <param name="completedTarget">target camera position</param>
    public void AnimationFinishedEventReader(DynamicMenuCameraTarget completedTarget)
    {
        if (completedTarget != this) return;

        //call animation end
        Event_AnimationEnd.Invoke();
    }

    /// <summary>
    /// returns the updated journey percentage based on the curve set
    /// </summary>
    /// <param name="currentJourney"></param>
    /// <returns></returns>
    public float GetUpdatedJourneyFromCurve(float currentJourney)
    {
        return lerpAnimationCurve.Evaluate(currentJourney);
    }

    /// <summary>
    /// returns the updated journey percentage based on the curve set
    /// </summary>
    /// <param name="currentJourney"></param>
    /// <returns></returns>
    public float GetUpdatedRotationJourneyFromCurve(float currentJourney)
    {
        if (seperateRotationCurve) return lerpRotationCurve.Evaluate(currentJourney);

        return GetUpdatedJourneyFromCurve(currentJourney);
    }

    //Custom editor settings to show and hide options
    [CustomEditor(typeof(DynamicMenuCameraTarget))]
    public class CameraTagetEditor : Editor
    {
        bool showAnimationSettings = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            showAnimationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAnimationSettings, "Animation Curve");

            if (showAnimationSettings)
            {
                //set this class as editor script
                DynamicMenuCameraTarget cameraTargetEditor = target as DynamicMenuCameraTarget;

                if (cameraTargetEditor.seperateRotationCurve)
                {
                    cameraTargetEditor.lerpAnimationCurve = EditorGUILayout.CurveField("Lerp Movement Animation", cameraTargetEditor.lerpAnimationCurve);
                }
                else
                {
                    cameraTargetEditor.lerpAnimationCurve = EditorGUILayout.CurveField("Lerp Animation", cameraTargetEditor.lerpAnimationCurve);
                }

                cameraTargetEditor.seperateRotationCurve = EditorGUILayout.Toggle("Seperate Rotation Curve?", cameraTargetEditor.seperateRotationCurve);

                if (cameraTargetEditor.seperateRotationCurve)
                {
                    cameraTargetEditor.lerpRotationCurve = EditorGUILayout.CurveField("Lerp Rotation Animation", cameraTargetEditor.lerpRotationCurve);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
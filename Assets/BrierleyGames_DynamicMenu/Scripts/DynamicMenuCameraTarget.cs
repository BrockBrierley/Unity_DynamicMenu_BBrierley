using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class DynamicMenuCameraTarget : MonoBehaviour
{
    public enum SoundType
    {
        MoveTo,
        MoveBack,
        Silent,
        Custom
    }
    //number of second to move to this set position
    public float transitionTimeInSeconds;

    [Range(0.0f, 1.0f)]
    public float MiddleAnimationEventTime = 0.5f;

    //Select sound enum to create simple consistent audio
    [HideInInspector]
    [SerializeField] private SoundType selectSoundType;
    [HideInInspector]
    [SerializeField] private AudioClip[] customSounds;

    ////Animation Selections
    [HideInInspector]
    //animation curve used to add in animation effects
    [SerializeField] private AnimationCurve lerpAnimationCurve;
    [HideInInspector]
    //bool to allow seperate RotationCurve
    [SerializeField] private bool seperateRotationCurve;
    [HideInInspector]
    //animation curve used to add in animation effects
    [SerializeField] private AnimationCurve lerpRotationCurve;

    //Events
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
        //verify manager is in the scene before attempting to access
        if (DynamicMenuManager._instance == null)
        {
            Debug.LogError("No DynamicMenuManager in the scene, DynamicMenu objects will fail to work properly until it has been added");
            return;
        }
        DynamicMenuManager._instance.AnimateToPosition(this);


        //no audio necessary
        if (selectSoundType == SoundType.Silent) return;

        //verify audio manager is in the scene before attempting to access
        if (DynamicMenuAudioManager._instance == null)
        {
            Debug.LogWarning("No DynamicMenuAudioManager in scene, please add it or change sound type to silent to remove this warning");
            return;
        }
        switch(selectSoundType)
        {
            //play move to sound
            case SoundType.MoveTo:
                DynamicMenuAudioManager._instance.PlayMoveToAudio();
                break;
            //play move back sound
            case SoundType.MoveBack:
                DynamicMenuAudioManager._instance.PlayMoveBackAudio();
                break;
            //play custom sound
            case SoundType.Custom:
                DynamicMenuAudioManager._instance.PlayAudio(customSounds);
                break;
            //do not play a sound
            case SoundType.Silent:
            default:
                break;
        }
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
        bool showAnimationSettings = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            DynamicMenuCameraTarget cameraTargetEditor = (DynamicMenuCameraTarget)target;
            serializedObject.Update();

            serializedObject.FindProperty("selectSoundType").enumValueIndex = (int)(SoundType)EditorGUILayout.EnumPopup("Select Sound Mode", cameraTargetEditor.selectSoundType);

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (cameraTargetEditor.selectSoundType == SoundType.Custom)
            {

                SerializedProperty soundsProperty = serializedObject.FindProperty("customSounds");

                EditorGUILayout.PropertyField(soundsProperty, true);
            }






            showAnimationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAnimationSettings, "Animation Curve");

            if (showAnimationSettings)
            {

                SerializedProperty lerpAnimationCurve = serializedObject.FindProperty("lerpAnimationCurve");
                EditorGUILayout.PropertyField (lerpAnimationCurve, true);


                SerializedProperty seperateRotationCurve = serializedObject.FindProperty("seperateRotationCurve");
                EditorGUILayout.PropertyField(seperateRotationCurve, true);

                if (cameraTargetEditor.seperateRotationCurve)
                {
                    SerializedProperty lerpRotationCurve = serializedObject.FindProperty("lerpRotationCurve");
                    EditorGUILayout.PropertyField(lerpRotationCurve, true);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            serializedObject.ApplyModifiedProperties();
        }
    }
}
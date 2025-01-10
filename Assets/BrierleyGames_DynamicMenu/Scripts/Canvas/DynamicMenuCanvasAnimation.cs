using UnityEditor;
using UnityEngine;

public class DynamicMenuCanvasAnimation : MonoBehaviour
{
    //Enums to make customising easier for the user
    public enum AnimationStartPoint
    {
        CameraAnimationStart,
        CameraAnimationMid,
        CameraAnimationEnd
    }

    public enum AnimationDirection
    {
        NoAnimation,
        Left,
        Up,
        Right,
        Down
    }
    public enum FadeType
    {
        Fade,
        Instant,
        NoFade
    }
    public enum GrowType
    {
        ConstantSize,
        Grow,
        Shrink
    }
    [Header("References")]
    [SerializeField] private DynamicMenuCameraTarget connectCameraTargetPosition;
    [SerializeField] private float animationTimeInSeconds;

    [Header("Set Up")]
    [SerializeField] private AnimationStartPoint animStartPoint;


    //Animation Direction variables (unclamped)
    [HideInInspector]
    [SerializeField] private AnimationDirection animateDirection;
    [HideInInspector]
    [SerializeField] private bool seperateAnimateOutDirection;
    [HideInInspector]
    [SerializeField] private AnimationDirection animateOutDirection;
    [HideInInspector]
    [SerializeField] private AnimationCurve directionCurve;

    //Fade Type variables (clamp bewteen 0 and 1)
    [HideInInspector]
    [SerializeField] private FadeType animateFadeType;
    [HideInInspector]
    [SerializeField] private bool seperateFadeOutDirection;
    [HideInInspector]
    [SerializeField] private FadeType animateOutFadeType;
    [HideInInspector]
    [SerializeField] private AnimationCurve fadeCurve;

    //Grow type variables (clamp to 0+)
    [HideInInspector]
    [SerializeField] private GrowType animateGrowType;
    [HideInInspector]
    [SerializeField] private bool seperateGrowOutDirection;
    [HideInInspector]
    [SerializeField] private GrowType animateOutGrowType;
    [HideInInspector]
    [SerializeField] private AnimationCurve growCurve;

    //private variables
    public float animationJourney = 1;

    private Vector3 targetPosition;
    private float targetFade;
    private Vector3 targetScale;


    private void Start()
    {
        //Set animation start points
        switch(animStartPoint)
        {
            //set animation start point in the middle of camera animation
            case AnimationStartPoint.CameraAnimationMid:
                connectCameraTargetPosition.Event_AnimationMid.AddListener(AnimateIn);
                break;
            //set animation start point at the end of camera animation
            case AnimationStartPoint.CameraAnimationEnd:
                connectCameraTargetPosition.Event_AnimationEnd.AddListener(AnimateIn);
                break;
            //Set animation start point at the start of the camera animation
            case AnimationStartPoint.CameraAnimationStart:
            default:
                connectCameraTargetPosition.Event_AnimationStart.AddListener(AnimateIn);
                break;
        }

        connectCameraTargetPosition.Event_HideCanvasPanel.AddListener(AnimateOut);
    }

    private void Update()
    {
        if (animationJourney >= 1) return;
    }

    private void OnDestroy()
    {
        //remove listeners on destroy
        connectCameraTargetPosition.Event_AnimationStart.RemoveListener(AnimateIn);
        connectCameraTargetPosition.Event_AnimationMid.RemoveListener(AnimateIn);
        connectCameraTargetPosition.Event_AnimationEnd.RemoveListener(AnimateIn);
        connectCameraTargetPosition.Event_HideCanvasPanel.RemoveListener(AnimateOut);
    }


    public void AnimateIn()
    {
        
        BeginAnimation();
    }

    public void AnimateOut()
    {
        BeginAnimation();
    }

    private void BeginAnimation()
    {
        animationJourney = 0;
    }

    //Show and Hide editor variables depending on what is selected
    //need to do for all variables to show them in the order prefered
    [CustomEditor(typeof(DynamicMenuCanvasAnimation))]
    public class CanvasAnimationEditor : Editor
    {
        bool showAnimationSettings = false;
        bool showFadeSettings = false;
        bool showGrowSettings = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DynamicMenuCanvasAnimation dynamicMenuCanvasAnimationEditor = target as DynamicMenuCanvasAnimation;

            // Show animation direction variables
            showAnimationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAnimationSettings, "Movement Animation Settings");

            if (showAnimationSettings)
            {
                dynamicMenuCanvasAnimationEditor.animateDirection = (AnimationDirection)EditorGUILayout.EnumPopup("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.animateDirection);
                dynamicMenuCanvasAnimationEditor.seperateAnimateOutDirection = EditorGUILayout.Toggle("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.seperateAnimateOutDirection);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateAnimateOutDirection)
                {
                    dynamicMenuCanvasAnimationEditor.animateOutDirection = (AnimationDirection)EditorGUILayout.EnumPopup("Animate Out Direction", dynamicMenuCanvasAnimationEditor.animateOutDirection);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Show Fade Settings in inspector
            showFadeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showFadeSettings, "Fade Animation Settings");

            if (showFadeSettings)
            {
                dynamicMenuCanvasAnimationEditor.animateFadeType = (FadeType)EditorGUILayout.EnumPopup("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.animateFadeType);
                dynamicMenuCanvasAnimationEditor.seperateFadeOutDirection = EditorGUILayout.Toggle("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.seperateFadeOutDirection);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateFadeOutDirection)
                {
                    dynamicMenuCanvasAnimationEditor.animateOutFadeType = (FadeType)EditorGUILayout.EnumPopup("Animate Out Direction", dynamicMenuCanvasAnimationEditor.animateOutFadeType);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Show Grow Settings in inspector
            showGrowSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGrowSettings, "Grow Animation Settings");

            if (showGrowSettings)
            {
                dynamicMenuCanvasAnimationEditor.animateGrowType = (GrowType)EditorGUILayout.EnumPopup("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.animateGrowType);
                dynamicMenuCanvasAnimationEditor.seperateGrowOutDirection = EditorGUILayout.Toggle("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.seperateGrowOutDirection);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateGrowOutDirection)
                {
                    dynamicMenuCanvasAnimationEditor.animateOutGrowType = (GrowType)EditorGUILayout.EnumPopup("Animate Out Direction", dynamicMenuCanvasAnimationEditor.animateOutGrowType);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}




/*
//movement
animation distance in
animation distance out

//size
Larger limit
smaller limit

//opacity
//can use animation curve but clamped


universal animation time in seconds to keep it simpler

animate in on call
animate out on call
 
 
 */
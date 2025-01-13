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
        Down,
        Forward,
        Backward
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
    [SerializeField] private CanvasGroup canvasObject;
    [SerializeField] private float animationTimeInSeconds;

    [Header("Set Up")]
    [SerializeField] private AnimationStartPoint animStartPoint;
    [SerializeField] private bool startInactive;


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
    [SerializeField] private bool seperateFadeOutType;
    [HideInInspector]
    [SerializeField] private FadeType animateOutFadeType;
    [HideInInspector]
    [SerializeField] private AnimationCurve fadeCurve;

    //Grow type variables (clamp to 0+)
    [HideInInspector]
    [SerializeField] private GrowType animateGrowType;
    [HideInInspector]
    [SerializeField] private bool seperateGrowOutType;
    [HideInInspector]
    [SerializeField] private GrowType animateOutGrowType;
    [HideInInspector]
    [SerializeField] private AnimationCurve growCurve;

    //private variables
    public float animationJourney = 1;

    //origin starting variables
    private Vector3 originPosition;
    private Vector3 originScale;


    //target position
    private Vector3 targetPosition;
    private float targetFade;
    private Vector3 targetScale;

    //animationStartingPosition;
    private Vector3 animStartingPosition;
    private float animStartingFade;
    private Vector3 animStartingScale;

    //bool to determine the direction of animating canvas
    private bool animatingIntoPosition = false;

    //variables to be applied to animation
    private float positionOffset = 10;
    private float scaleOffset = 10;

    //
    private float minFade = 0;
    private float maxFade = 1;

    private void Start()
    {
        if (canvasObject == null)
        {
            canvasObject = GetComponent<CanvasGroup>();
        }


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

        originPosition = transform.localPosition;
        originScale = transform.localScale;

        //set up starting position, scale and fade
        //do last in start to not override any variables
        if(startInactive)
        {
            transform.localPosition = GetInactivePosition();
            transform.localScale = GetInactiveScale();
            canvasObject.alpha = GetInactiveFade();
        }
    }

    private void Update()
    {
        if (animationJourney >= 1) return;
    }

    private void OnDestroy()
    {
        //remove listeners on destroy
        connectCameraTargetPosition?.Event_AnimationStart.RemoveListener(AnimateIn);
        connectCameraTargetPosition?.Event_AnimationMid.RemoveListener(AnimateIn);
        connectCameraTargetPosition?.Event_AnimationEnd.RemoveListener(AnimateIn);
        connectCameraTargetPosition?.Event_HideCanvasPanel.RemoveListener(AnimateOut);
    }


    public void AnimateIn()
    {
        //set start and end position
        animStartingPosition = GetInactivePosition();
        targetPosition = GetActivePosition();

        //set start and end fade
        animStartingFade  = GetInactiveFade();
        targetFade = GetActiveFade();

        //set start and end scale
        animStartingScale = GetInactiveScale();
        targetScale = GetActiveScale();

        animatingIntoPosition = true;

        BeginAnimation();
    }

    public void AnimateOut()
    {
        targetPosition = GetInactivePosition();
        animStartingPosition = GetActivePosition();

        animatingIntoPosition = false;

        BeginAnimation();
    }

    //Get animate in and out values

    private Vector3 GetActivePosition()
    {
        return originPosition;
    }

    private Vector3 GetInactivePosition()
    {
        //get if animation is seperate or not
        AnimationDirection direction;
        if (seperateAnimateOutDirection)
        {
            direction = animateOutDirection;
        }
        else
        {
            direction = animateDirection;
        }

        switch (direction)
        {
            //up down
            case AnimationDirection.Up:
                return originPosition + transform.up * positionOffset;
            case AnimationDirection.Down:
                return originPosition - transform.up * positionOffset;
            
            //Left Right
            case AnimationDirection.Right:
                return originPosition + transform.right * positionOffset;
            case AnimationDirection.Left:
                return originPosition - transform.right * positionOffset;

            //Forward Backward
            case AnimationDirection.Forward:
                return originPosition + transform.forward * positionOffset;
            case AnimationDirection.Backward:
                return originPosition - transform.forward * positionOffset;

            //no animation
            case AnimationDirection.NoAnimation:
            default:
                break;
        }

        return originPosition;
    }

    //Get fade in and out values

    private float GetActiveFade()
    {
        return maxFade;
    }

    private float GetInactiveFade()
    { 
        FadeType fadeType;
        if (seperateFadeOutType)
        {
            fadeType = animateOutFadeType;
        }
        else
        {
            fadeType = animateFadeType;
        }

        //get fade out alpha value
        switch(fadeType)
        {
            case FadeType.NoFade:
                return maxFade;
            case FadeType.Instant:
            case FadeType.Fade:
            default:
                break;
        }
        return minFade;
    }

   
    //get scale in and out values
    private Vector3 GetActiveScale()
    {
        return originScale;
    }

    private Vector3 GetInactiveScale()
    {
        GrowType growType;
        if(seperateGrowOutType)
        {
            growType = animateOutGrowType;
        }
        else
        {
            growType = animateGrowType;
        }

        switch (growType)
        {
            case GrowType.Grow:
                return originScale + new Vector3(scaleOffset, scaleOffset, scaleOffset);
            case GrowType.Shrink:
                return Vector3.zero;
            case GrowType.ConstantSize:
            default:
                break;
        }

        return originScale;
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
                dynamicMenuCanvasAnimationEditor.seperateFadeOutType = EditorGUILayout.Toggle("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.seperateFadeOutType);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateFadeOutType)
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
                dynamicMenuCanvasAnimationEditor.seperateGrowOutType = EditorGUILayout.Toggle("Seperate Animate Out Direction", dynamicMenuCanvasAnimationEditor.seperateGrowOutType);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateGrowOutType)
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
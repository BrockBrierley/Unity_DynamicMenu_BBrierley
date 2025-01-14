using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
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
    [SerializeField] private DynamicMenuCameraTarget connectedCameraTargetPosition;
    [SerializeField] private CanvasGroup canvasObject;


    [Header("Set Up")]
    [SerializeField] private float animationTimeInSeconds;
    [SerializeField] private AnimationStartPoint animStartPoint;
    [SerializeField] private bool startInactive;


    //Animation Direction variables (unclamped)
    [HideInInspector]
    [SerializeField] private AnimationDirection animateDirection;
    [HideInInspector]
    [SerializeField] private bool seperateAnimateOutDirection;
    [HideInInspector]
    [SerializeField] private AnimationDirection animateOutDirection;
    //direction Animation curve
    [HideInInspector]
    [SerializeField] private AnimationCurve directionCurve;
    [HideInInspector]
    [SerializeField] private bool seperateAnimateOutDirectionCurve;
    [HideInInspector]
    [SerializeField] private AnimationCurve directionOutCurve;

    //Fade Type variables (clamp bewteen 0 and 1)
    [HideInInspector]
    [SerializeField] private FadeType animateFadeType;
    [HideInInspector]
    [SerializeField] private bool seperateFadeOutType;
    [HideInInspector]
    [SerializeField] private FadeType animateOutFadeType;
    //fade animation curve
    [HideInInspector]
    [SerializeField] private AnimationCurve fadeCurve;
    [HideInInspector]
    [SerializeField] private bool seperateFadeCurve;
    [HideInInspector]
    [SerializeField] private AnimationCurve fadeOutCurve;

    //Grow type variables (clamp to 0+)
    [HideInInspector]
    [SerializeField] private GrowType animateGrowType;
    [HideInInspector]
    [SerializeField] private bool seperateGrowOutType;
    [HideInInspector]
    [SerializeField] private GrowType animateOutGrowType;
    //Grow animation Curve
    [HideInInspector]
    [SerializeField] private AnimationCurve growCurve;
    [HideInInspector]
    [SerializeField] private bool seperateGrowCurve;
    [HideInInspector]
    [SerializeField] private AnimationCurve growOutCurve;

    //private variables
    private float animationJourney = 1;

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
    private float positionOffset = 2;
    private float scaleOffset = 2;

    //min and max fade alphas
    private float minFade = 0;
    private float maxFade = 1;

    //calculatedCurves used in update
    private AnimationCurve movementCurve;
    private AnimationCurve alphaCurve;
    private AnimationCurve sizeCurve;

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
                connectedCameraTargetPosition.Event_AnimationMid.AddListener(AnimateIn);
                break;
            //set animation start point at the end of camera animation
            case AnimationStartPoint.CameraAnimationEnd:
                connectedCameraTargetPosition.Event_AnimationEnd.AddListener(AnimateIn);
                break;
            //Set animation start point at the start of the camera animation
            case AnimationStartPoint.CameraAnimationStart:
            default:
                connectedCameraTargetPosition.Event_AnimationStart.AddListener(AnimateIn);
                break;
        }

        connectedCameraTargetPosition.Event_HideCanvasPanel.AddListener(AnimateOut);

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
        animationJourney += Time.deltaTime/animationTimeInSeconds;

        //Lerp this canvas
        //unclamped
        transform.localPosition = Vector3.Lerp(animStartingPosition, targetPosition, movementCurve.Evaluate(animationJourney));
        //clamped > 0
        transform.localScale = Vector3.Max (Vector3.Lerp(animStartingScale, targetScale, sizeCurve.Evaluate(animationJourney)), Vector3.zero);
        //clamped between 0 and 1
        canvasObject.alpha = Mathf.Clamp(animStartingFade + (targetFade - animStartingFade) * alphaCurve.Evaluate(animationJourney), minFade, maxFade);
    }

    private void BeginAnimation()
    {
        SetUpCurves();
        animationJourney = 0;
    }

    private void SetUpCurves()
    {
        //Movement Curve
        if (!animatingIntoPosition)
        {
            if (seperateAnimateOutDirectionCurve)
            {
                movementCurve = directionOutCurve;
            }
            else
            {
                movementCurve = directionCurve;
            }
            //Fade Curve
            if (seperateFadeCurve)
            {
                alphaCurve = fadeOutCurve;
            }
            else
            {
                alphaCurve = fadeCurve;
            }
            //Scale Curve
            if (seperateGrowCurve)
            {
                sizeCurve = growOutCurve;
            }
            else
            {
                sizeCurve = growCurve;
            }
        }
        else
        {
            movementCurve = directionCurve;
            alphaCurve = fadeCurve;
            sizeCurve = growCurve;
        }
    }

    private void OnDestroy()
    {
        //remove listeners on destroy
        connectedCameraTargetPosition?.Event_AnimationStart.RemoveListener(AnimateIn);
        connectedCameraTargetPosition?.Event_AnimationMid.RemoveListener(AnimateIn);
        connectedCameraTargetPosition?.Event_AnimationEnd.RemoveListener(AnimateIn);
        connectedCameraTargetPosition?.Event_HideCanvasPanel.RemoveListener(AnimateOut);
    }


    public void AnimateIn()
    {

        animatingIntoPosition = true;

        //set start and end position
        animStartingPosition = GetInactivePosition();
        targetPosition = GetActivePosition();

        //set start and end fade
        animStartingFade  = GetInactiveFade();
        targetFade = GetActiveFade();

        //set start and end scale
        animStartingScale = GetInactiveScale();
        targetScale = GetActiveScale();

        BeginAnimation();
    }

    public void AnimateOut()
    {

        animatingIntoPosition = false;

        //set start and end position
        animStartingPosition = GetActivePosition();
        targetPosition = GetInactivePosition();

        //set start and end fade
        animStartingFade  = GetActiveFade();
        targetFade = GetInactiveFade();

        //set start and end scale
        animStartingScale = GetActiveScale();
        targetScale = GetInactiveScale();

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
        if (seperateAnimateOutDirection && !animatingIntoPosition)
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
        if (seperateFadeOutType && !animatingIntoPosition)
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
        if(seperateGrowOutType && !animatingIntoPosition)
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
                return originScale * scaleOffset;
            case GrowType.Shrink:
                return Vector3.zero;
            case GrowType.ConstantSize:
            default:
                break;
        }

        return originScale;
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

            DynamicMenuCanvasAnimation dynamicMenuCanvasAnimationEditor = (DynamicMenuCanvasAnimation)target;// as DynamicMenuCanvasAnimation;
            serializedObject.Update();




            // Show animation direction variables
            showAnimationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAnimationSettings, "Movement Animation Settings");

            if (showAnimationSettings)
            {
                //directionStyle
                serializedObject.FindProperty("animateDirection").enumValueIndex = (int)(AnimationDirection)EditorGUILayout.EnumPopup("Animation Direction", dynamicMenuCanvasAnimationEditor.animateDirection);
                serializedObject.FindProperty("seperateAnimateOutDirection").boolValue = EditorGUILayout.Toggle("Seperate Animate Out Direction?", dynamicMenuCanvasAnimationEditor.seperateAnimateOutDirection);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateAnimateOutDirection)
                {
                    serializedObject.FindProperty("animateOutDirection").enumValueIndex = (int)(AnimationDirection)EditorGUILayout.EnumPopup("Animate Out Direction", dynamicMenuCanvasAnimationEditor.animateOutDirection);
                }

                //curve settings
                serializedObject.FindProperty("directionCurve").animationCurveValue = EditorGUILayout.CurveField("Animation Curve", dynamicMenuCanvasAnimationEditor.directionCurve);
                serializedObject.FindProperty("seperateAnimateOutDirectionCurve").boolValue = EditorGUILayout.Toggle("Seperate Animate Out Curve?", dynamicMenuCanvasAnimationEditor.seperateAnimateOutDirectionCurve);
                if (dynamicMenuCanvasAnimationEditor.seperateAnimateOutDirectionCurve)
                {
                    serializedObject.FindProperty("directionOutCurve").animationCurveValue = EditorGUILayout.CurveField("Animate out Curve", dynamicMenuCanvasAnimationEditor.directionOutCurve);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();



            //Show Fade Settings in inspector
            showFadeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showFadeSettings, "Fade Animation Settings");

            if (showFadeSettings)
            {
                serializedObject.FindProperty("animateFadeType").enumValueIndex = (int)(FadeType)EditorGUILayout.EnumPopup("Fade Type", dynamicMenuCanvasAnimationEditor.animateFadeType);
                serializedObject.FindProperty("seperateFadeOutType").boolValue = EditorGUILayout.Toggle("Seperate Fade Out Type", dynamicMenuCanvasAnimationEditor.seperateFadeOutType);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateFadeOutType)
                {
                    serializedObject.FindProperty("animateOutFadeType").enumValueIndex = (int)(FadeType)EditorGUILayout.EnumPopup("Fade Out Type", dynamicMenuCanvasAnimationEditor.animateOutFadeType);
                }

                //curve settings
                serializedObject.FindProperty("fadeCurve").animationCurveValue = EditorGUILayout.CurveField("Fade Curve", dynamicMenuCanvasAnimationEditor.fadeCurve);
                serializedObject.FindProperty("seperateFadeCurve").boolValue = EditorGUILayout.Toggle("Seperate Fade Out Curve?", dynamicMenuCanvasAnimationEditor.seperateFadeCurve);
                if (dynamicMenuCanvasAnimationEditor.seperateFadeCurve)
                {
                    serializedObject.FindProperty("fadeOutCurve").animationCurveValue = EditorGUILayout.CurveField("Fade out Curve", dynamicMenuCanvasAnimationEditor.fadeOutCurve);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();





            //Show Grow Settings in inspector
            showGrowSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGrowSettings, "Grow Animation Settings");

            if (showGrowSettings)
            {
                serializedObject.FindProperty("animateGrowType").enumValueIndex = (int)(GrowType)EditorGUILayout.EnumPopup("Grow Type", dynamicMenuCanvasAnimationEditor.animateGrowType);
                serializedObject.FindProperty("seperateGrowOutType").boolValue = EditorGUILayout.Toggle("Seperate Grow Out Type?", dynamicMenuCanvasAnimationEditor.seperateGrowOutType);
                //only show if above variable is selected
                if (dynamicMenuCanvasAnimationEditor.seperateGrowOutType)
                {
                    serializedObject.FindProperty("animateOutGrowType").enumValueIndex = (int)(GrowType)EditorGUILayout.EnumPopup("Grow Out Direction", dynamicMenuCanvasAnimationEditor.animateOutGrowType);
                }

                //curve settings
                serializedObject.FindProperty("growCurve").animationCurveValue = EditorGUILayout.CurveField("Grow Curve", dynamicMenuCanvasAnimationEditor.growCurve);
                serializedObject.FindProperty("seperateGrowCurve").boolValue = EditorGUILayout.Toggle("Seperate Grow Out Curve?", dynamicMenuCanvasAnimationEditor.seperateGrowCurve);
                if (dynamicMenuCanvasAnimationEditor.seperateGrowCurve)
                {
                    serializedObject.FindProperty("growOutCurve").animationCurveValue = EditorGUILayout.CurveField("Grow out Curve", dynamicMenuCanvasAnimationEditor.growOutCurve);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
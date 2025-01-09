using UnityEditor;
using UnityEngine;

public class DynamicMenuCanvasAnimation : MonoBehaviour
{
    //Enums to make customising easier for the user
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

    //Animation Direction variables
    [HideInInspector]
    [SerializeField] private AnimationDirection animateDirection;
    [HideInInspector]
    [SerializeField] private bool seperateAnimateOutDirection;
    [HideInInspector]
    [SerializeField] private AnimationDirection animateOutDirection;

    //Fade Type variables
    [HideInInspector]
    [SerializeField] private FadeType animateFadeType;
    [HideInInspector]
    [SerializeField] private bool seperateFadeOutDirection;
    [HideInInspector]
    [SerializeField] private FadeType animateOutFadeType;

    //Grow type variables
    [HideInInspector]
    [SerializeField] private GrowType animateGrowType;
    [HideInInspector]
    [SerializeField] private bool seperateGrowOutDirection;
    [HideInInspector]
    [SerializeField] private GrowType animateOutGrowType;


    public void AnimateIn()
    {

    }

    public void AnimateOut()
    {

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
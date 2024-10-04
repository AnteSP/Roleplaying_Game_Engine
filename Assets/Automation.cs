using UnityEditor;
using UnityEngine;
using System.IO; // For file saving
using UnityEditor.Animations;
using System.Linq;
using System;

public class Automation_png : EditorWindow
{
    private Texture2D selectedImage;
    [MenuItem("TOOLS THAT MY BITCH ASS MADE AHAHAAHAHA/PNG transparency")]
    public static void ShowWindow_png()
    {
        Automation_png window = GetWindow<Automation_png>("PNG bylat");

        window.minSize = new Vector2(400, 100);
        window.maxSize = new Vector2(400, 100);
    }

    bool pressed = false;

    private void OnGUI()
    {
        selectedImage = (Texture2D)EditorGUILayout.ObjectField("Insert PNG Image", selectedImage, typeof(Texture2D), false);

        if (selectedImage != null && pressed) pressed = false;

        if (GUILayout.Button("Make pure white in a png transparent"))
        {
            Debug.Log("Button pressed! Running code...");
            MakeWhitePixelsTransparent(selectedImage);
            pressed = true;
        }

        if (pressed)
        {
            GUILayout.Label("Done", EditorStyles.boldLabel);
            selectedImage = null;
        }
    }

    private void MakeWhitePixelsTransparent(Texture2D image)
    {
        Color[] pixels = image.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r == 1f && pixels[i].g == 1f && pixels[i].b == 1f)
            {
                pixels[i] = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                pixels[i] = new Color(pixels[i].r, pixels[i].g, pixels[i].b, 1f);
            }
        }

        image.SetPixels(pixels);
        image.Apply();

        SaveTextureAsPNG(image, AssetDatabase.GetAssetPath(selectedImage));
        Debug.Log("Modified image saved as " + AssetDatabase.GetAssetPath(selectedImage));
    }

    private void SaveTextureAsPNG(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
    }
}

public class Automation_anim : EditorWindow
{
    bool pressed = false;

    [MenuItem("TOOLS THAT MY BITCH ASS MADE AHAHAAHAHA/Generate Anim")]
    public static void ShowWindow_anim()
    {
        GetWindow<Automation_anim>("Easy animator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Make sure:\n1: read/write is enabled on sprite asset \n2: the sprite sheet is sliced properly (4x5)");
        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Insert PNG sprite sheet", spriteSheet, typeof(Texture2D), false);

        if (spriteSheet != null && pressed) pressed = false;

        if (GUILayout.Button("Generate Animation"))
        {
            Debug.Log("Button pressed! Running code...");
            GenerateAnimator();
            pressed = true;
        }

        if (pressed)
        {
            GUILayout.Label("Done", EditorStyles.boldLabel);
            spriteSheet = null;
        }
    }

    public Texture2D spriteSheet; // Input sprite sheet
    public int columns = 5; // Number of columns in the sprite sheet
    public int rows = 4; // Number of rows in the sprite sheet
    public float frameRate = 60f; // Frame rate for the animations
    public string outputFolder = "Assets/Animations"; // Folder to store the generated assets
    Sprite[] sprites = new Sprite[0];

    public void GenerateAnimator()
    {
        outputFolder = "Assets/Animations";
        string path = AssetDatabase.GetAssetPath(spriteSheet);
        string char_name = Path.GetFileName(Path.GetDirectoryName(path));
        
        outputFolder = $"{outputFolder}/{char_name}";
        Directory.CreateDirectory(outputFolder);
        Debug.Log("saving to " + outputFolder);

        sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath($"{outputFolder}/{char_name}_Walking.controller");

        AnimationClip walkDownClip = CreateAnimationClip(sprites, rows, columns, 0, "Walk_Down");
        AnimationClip walkLeftClip = CreateAnimationClip(sprites, rows, columns, 1, "Walk_Left");
        AnimationClip walkRightClip = CreateAnimationClip(sprites, rows, columns, 2, "Walk_Right");
        AnimationClip walkUpClip = CreateAnimationClip(sprites, rows, columns, 3, "Walk_Up");

        animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Int);
        animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Int);
        animatorController.AddParameter("Speed", AnimatorControllerParameterType.Float);

        // Add states and transitions
        AddStateWithTransitions(animatorController, walkDownClip, 0, "Walk_Down", "Vertical", -1, "Speed");
        AddStateWithTransitions(animatorController, walkUpClip,3, "Walk_Up", "Vertical", 1, "Speed");
        AddStateWithTransitions(animatorController, walkLeftClip,1, "Walk_Left", "Horizontal", -1, "Speed");
        AddStateWithTransitions(animatorController, walkRightClip,2, "Walk_Right", "Horizontal", 1, "Speed");

        Debug.Log("Animator generated successfully.");
    }

    // Create animation clips for each direction
    private AnimationClip CreateAnimationClip(Sprite[] sprites, int rows, int columns, int rowIndex, string clipName)
    {
        AnimationClip clip = new AnimationClip { frameRate = frameRate };

        EditorCurveBinding spriteBinding = new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" };

        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[columns];

        for (int i = 0; i < columns; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i / frameRate,
                value = sprites[rowIndex * columns + i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);

        AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
        clipSettings.loopTime = true; // Enable looping
        AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

        // Save the animation clip
        AssetDatabase.CreateAsset(clip, $"{outputFolder}/{clipName}.anim");
        AssetDatabase.SaveAssets();

        return clip;
    }

    // Add states and transitions for the Animator Controller
    private void AddStateWithTransitions(AnimatorController controller, AnimationClip clip, int i,string stateName, string parameterName, int parameterValue, string speedParameter)
    {
        var rootStateMachine = controller.layers[0].stateMachine;

        // Add transition to idle state
        var idleState = rootStateMachine.AddState("Idle_" + stateName);
        //gets every 5th element from sprites (including first)
        var idleClip = CreateAnimationClip(sprites.Where((element, index) => index % 5 == 0).ToArray(), rows, 1, i, "Idle_" + stateName);
        idleState.motion = idleClip;




        // Add state
        var state = rootStateMachine.AddState(stateName);
        state.speedParameter = speedParameter;
        state.speedParameterActive = true;
        state.motion = clip;

        AnimatorStateTransition transitionFromAny = rootStateMachine.AddAnyStateTransition(state);
        transitionFromAny.hasExitTime = false;
        transitionFromAny.canTransitionToSelf = false;
        transitionFromAny.duration = 0;

        transitionFromAny.AddCondition((parameterValue > 0) ? AnimatorConditionMode.Greater : AnimatorConditionMode.Less, 0, parameterName);



        var transitionToIdle = state.AddTransition(idleState);
        transitionToIdle.hasExitTime = false;
        //transitionToIdle.duration = 0;

        transitionToIdle.AddCondition(AnimatorConditionMode.Equals, 0, parameterName);


    }
}
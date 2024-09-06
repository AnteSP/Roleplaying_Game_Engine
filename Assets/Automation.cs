using UnityEditor;
using UnityEngine;
using System.IO; // For file saving

public class Automation : EditorWindow
{
    private Texture2D selectedImage;
    [MenuItem("TOOLS THAT MY BITCH ASS MADE AHAHAAHAHA/PNG transparency")]
    public static void ShowWindow()
    {
        Automation window = GetWindow<Automation>("My Editor Tool");

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
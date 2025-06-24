using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TilemapExporter : MonoBehaviour
{
    [MenuItem("Tools/Export Tilemap View to PNG")]
    static void ExportTilemapToPNG()
    {
        Camera cam = Camera.main;
        int width = Screen.width;
        int height = Screen.height;

        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = EditorUtility.SaveFilePanel("Save Tilemap PNG", "", "tilemap_capture.png", "png");
        File.WriteAllBytes(filename, bytes);

        Debug.Log("Saved Tilemap view to: " + filename);
    }
}

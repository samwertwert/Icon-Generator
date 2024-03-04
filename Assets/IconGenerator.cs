using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




public class IconGenerator : MonoBehaviour
{
    Camera cam;
    public string pathFolder;

    public List<GameObject> sceneObjects;

    public SizeOption screenshotSize = SizeOption._512;

    public enum SizeOption
    {
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048
    };
    private void Awake()
    {
        cam = Camera.main;
    }

    [ContextMenu("Screenshot")]
    private void ProcessScreenshots()
    {
        StartCoroutine(Screenshot());
    }

    private IEnumerator Screenshot()
    {
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            GameObject obj = sceneObjects[i];

            obj.gameObject.SetActive(true);
            yield return null;

            TakeShot($"{Application.dataPath}/{pathFolder}/{obj.name}_Icon.png");

            yield return null;
            obj.gameObject.SetActive(false);

            yield return null;
        }
    }
    public void TakeShot(string fullPath)
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        RenderTexture rt = new RenderTexture((int)screenshotSize, (int)screenshotSize, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D((int)screenshotSize, (int)screenshotSize, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, (int)screenshotSize, (int)screenshotSize), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

    }
}

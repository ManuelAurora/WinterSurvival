using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class IntroAnimationController : MonoBehaviour
{
    public Material material;
    public Texture2D texture;
    public float scaleSpeed = 1.0f;

    private float targetScale = 1.0f;
    private float currentScale = 1.0f;
    public bool isOpening = false;
    public bool isClosing = false;

    void Start()
    {
        if (material != null && texture != null)
        {
            material.SetTexture("_MainTex", texture);
        }
    }

    void Update()
    {
        if (isOpening)
        {
            targetScale = 2.0f;
        }
        else if (isClosing)
        {
            targetScale = 0.0f;
        }

        if (Mathf.Abs(currentScale - targetScale) > 0.01f)
        {
            currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * scaleSpeed);
            material.SetFloat("_Scale", currentScale);
        }
    }

    public void Open()
    {
        isOpening = true;
        isClosing = false;
    }

    public void Close()
    {
        isClosing = true;
        isOpening = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class FinalPhotoCapture : MonoBehaviour
{
    public RawImage photoPreviewImage;   // RawImage da prévia

    // =============================================================
    // 🚀 CAPTURA APENAS A ÁREA DA PRÉVIA (COM ÍCONES EM OUTRO CANVAS)
    // =============================================================
    public Texture2D CapturePreviewWithIcons()
    {
        // 1) Screenshot de toda a tela
        Texture2D full = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        full.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        full.Apply();

        // 2) Pega os cantos exatos do RawImage
        RectTransform rt = photoPreviewImage.rectTransform;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        // 3) Proteção contra valores inválidos
        int ix = Mathf.Clamp(Mathf.RoundToInt(x), 0, Screen.width);
        int iy = Mathf.Clamp(Mathf.RoundToInt(y), 0, Screen.height);
        int iw = Mathf.Clamp(Mathf.RoundToInt(width), 1, Screen.width - ix);
        int ih = Mathf.Clamp(Mathf.RoundToInt(height), 1, Screen.height - iy);

        // 4) Recorta somente a área da prévia com os ícones
        Texture2D cropped = new Texture2D(iw, ih, TextureFormat.RGB24, false);
        Color[] pixels = full.GetPixels(ix, iy, iw, ih);
        cropped.SetPixels(pixels);
        cropped.Apply();

        return cropped;
    }

    // =============================================================
    // 🚀 FOTO CLEAN = só a textura original da RawImage (sem ícones)
    // =============================================================
    public Texture2D CaptureClean()
    {
        Texture src = photoPreviewImage.texture;

        RenderTexture rt = RenderTexture.GetTemporary(src.width, src.height, 0);
        Graphics.Blit(src, rt);

        Texture2D output = new Texture2D(src.width, src.height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        output.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0);
        output.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return output;
    }
}

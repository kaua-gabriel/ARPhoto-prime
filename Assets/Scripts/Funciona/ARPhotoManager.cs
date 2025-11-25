using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class ARPhotoManager : MonoBehaviour
{
    [Header("UI Principal")]
    public GameObject canvasCameraUI;
    public GameObject canvasPreviewUI;
    public GameObject iconStampPanel;

    [Header("Tutorial")]
    public GameObject canvasTutorialHand;
    public Button tutorialOkButton;

    [Header("Preview da Foto")]
    public RawImage photoPreviewImage;
    public Button buttonConfirm;
    public Button buttonBack;

    [Header("Feedback")]
    public TMP_Text feedbackText;

    [Header("Configurações")]
    public int maxPhotosPerSession = 5;
    public int maxPhotoSizeMB = 5;

    private Texture2D capturedTexture;
    private int photosSent = 0;
    private bool isCapturing = false;

    [Header("Tela Final")]
    public GameObject finalScreenCanvas;

    private void Start()
    {
        canvasCameraUI.SetActive(true);
        canvasPreviewUI.SetActive(false);
        feedbackText.gameObject.SetActive(false);

        if (iconStampPanel != null)
            iconStampPanel.SetActive(false);

        if (canvasTutorialHand != null)
            canvasTutorialHand.SetActive(false);

        buttonConfirm.onClick.AddListener(OnConfirmClicked);
        buttonBack.onClick.AddListener(OnBackClicked);
    }

    // ================================================================
    // CAPTURA A FOTO ORIGINAL EM ALTA RESOLUÇÃO
    // ================================================================
    public void TakePhoto()
    {
        if (isCapturing) return;

        if (photosSent >= maxPhotosPerSession)
        {
            ShowFeedback("Limite de 5 fotos atingido!");
            return;
        }

        StartCoroutine(CapturePhotoFlow());
    }

    private IEnumerator CapturePhotoFlow()
    {
        isCapturing = true;

        canvasCameraUI.SetActive(false);
        feedbackText.text = "Capturando...";
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        int scaleFactor = 2;
        int width = Screen.width * scaleFactor;
        int height = Screen.height * scaleFactor;

        RenderTexture rt = new RenderTexture(width, height, 24);
        Camera mainCamera = Camera.main;

        mainCamera.targetTexture = rt;
        mainCamera.Render();

        RenderTexture.active = rt;

        capturedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Preview leve
        Texture2D previewTexture = ResizeTexture(capturedTexture, 0.25f);
        photoPreviewImage.texture = previewTexture;

        canvasPreviewUI.SetActive(true);
        feedbackText.gameObject.SetActive(false);

        if (iconStampPanel != null)
            iconStampPanel.SetActive(true);

        TryShowTutorial();
        isCapturing = false;
    }

    // ================================================================
    // TUTORIAL
    // ================================================================
    private void TryShowTutorial()
    {
        canvasTutorialHand.SetActive(true);

        tutorialOkButton.onClick.RemoveAllListeners();
        tutorialOkButton.onClick.AddListener(() =>
        {
            canvasTutorialHand.SetActive(false);
        });
    }

    // ================================================================
    // CONFIRMAR FOTO → SALVAR 2 IMAGENS
    // ================================================================
    private void OnConfirmClicked()
    {
        if (capturedTexture == null || isCapturing) return;

        buttonConfirm.interactable = false;
        buttonBack.interactable = false;

        ShowFeedback("📤 Preparando foto...");

        StartCoroutine(SendBothPhotos());
    }

    private IEnumerator SendBothPhotos()
    {
        // ===================== FOTO CLEAN =====================
        byte[] cleanJpg = ResizeTexture(capturedTexture, 0.5f).EncodeToJPG(85);

        // ===================== FOTO COM ÍCONES =====================
        Texture2D composedTexture = null;
        yield return StartCoroutine(CapturePreviewCoroutine(tex => composedTexture = tex));

        byte[] composedJpg = ResizeTexture(composedTexture, 0.5f).EncodeToJPG(85);

        // ===================== ENVIO =====================
        string baseName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        WWWForm form = new WWWForm();
        form.AddBinaryData("photo_clean", cleanJpg, "foto_" + baseName + "_clean.jpg", "image/jpeg");
        form.AddBinaryData("photo_composed", composedJpg, "foto_" + baseName + "_icons.jpg", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post("https://webhook.site/5b60b0fa-104e-483e-b0a8-b075b8972cd9", form))
        {
            www.timeout = 15;
            var op = www.SendWebRequest();

            while (!op.isDone)
            {
                ShowFeedback(" Enviando... " + (int)(www.uploadProgress * 100) + "%");
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                ShowFeedback(" Fotos enviadas com sucesso!");
                photosSent++;
            }
            else
            {
                ShowFeedback(" Erro ao enviar: " + www.error);
            }

            finalScreenCanvas.SetActive(true);

            buttonConfirm.interactable = true;
            buttonBack.interactable = true;
        }

    }

    // ================================================================
    // VOLTAR
    // ================================================================
    private void OnBackClicked()
    {
        canvasPreviewUI.SetActive(false);
        canvasCameraUI.SetActive(true);

        if (iconStampPanel != null)
            iconStampPanel.SetActive(false);

        ShowFeedback("Foto descartada");
    }

    // ================================================================
    // HELPERS
    // ================================================================
    private Texture2D ResizeTexture(Texture2D source, float scale)
    {
        int width = Mathf.RoundToInt(source.width * scale);
        int height = Mathf.RoundToInt(source.height * scale);

        RenderTexture rt = new RenderTexture(width, height, 24);
        Graphics.Blit(source, rt);

        Texture2D tex = new Texture2D(width, height, source.format, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        rt.Release();
        return tex;
    }

    private void ShowFeedback(string msg)
    {
        feedbackText.text = msg;
        feedbackText.gameObject.SetActive(true);
        StartCoroutine(HideFeedback());
    }



    private IEnumerator HideFeedback()
    {
        yield return new WaitForSeconds(2.2f);
        feedbackText.gameObject.SetActive(false);
    }


    private IEnumerator CapturePreviewCoroutine(System.Action<Texture2D> callback)
    {
        // Aguarda o próximo frame ser renderizado
        yield return new WaitForEndOfFrame();

        // 1) Screenshot da tela inteira
        Texture2D full = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        full.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        full.Apply();

        // 2) Pega os limites da RawImage em coordenadas de tela
        RectTransform rt = photoPreviewImage.rectTransform;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        // 3) Converte para inteiro válido
        int ix = Mathf.Clamp(Mathf.RoundToInt(x), 0, Screen.width);
        int iy = Mathf.Clamp(Mathf.RoundToInt(y), 0, Screen.height);
        int iw = Mathf.Clamp(Mathf.RoundToInt(width), 1, Screen.width - ix);
        int ih = Mathf.Clamp(Mathf.RoundToInt(height), 1, Screen.height - iy);

        // 4) Recorta
        Texture2D cropped = new Texture2D(iw, ih, TextureFormat.RGB24, false);
        Color[] pixels = full.GetPixels(ix, iy, iw, ih);
        cropped.SetPixels(pixels);
        cropped.Apply();

        callback?.Invoke(cropped);
    }


}

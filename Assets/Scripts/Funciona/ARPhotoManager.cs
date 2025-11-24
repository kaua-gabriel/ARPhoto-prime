using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class ARPhotoManager : MonoBehaviour
{
    [Header("UI Principal (Cena da Câmera)")]
    public GameObject canvasCameraUI;
    public GameObject canvasPreviewUI;
    public GameObject iconStampPanel;

    [Header("Tutorial")]
    public GameObject canvasTutorialHand;
    public Button tutorialOkButton; // <-- botão arrastado no inspector

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

    private const string TutorialShownKey = "TutorialShown";

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
        PlayerPrefs.DeleteKey("TutorialShown");

    }

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
        feedbackText.text = "📸 Capturando...";
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        int scaleFactor = 2;
        int width = Screen.width * scaleFactor;
        int height = Screen.height * scaleFactor;

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        Camera mainCamera = Camera.main;
        mainCamera.targetTexture = renderTexture;

        Texture2D capturedTextureHD = new Texture2D(width, height, TextureFormat.RGB24, false);
        mainCamera.Render();
        RenderTexture.active = renderTexture;
        capturedTextureHD.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTextureHD.Apply();

        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        capturedTexture = capturedTextureHD;

        // 🔹 Cria uma cópia reduzida só para exibir (evita travamento)
        Texture2D previewTexture = ResizeTexture(capturedTextureHD, 0.25f);
        photoPreviewImage.texture = previewTexture;

        feedbackText.gameObject.SetActive(false);
        canvasPreviewUI.SetActive(true);

        if (iconStampPanel != null)
            iconStampPanel.SetActive(true);

        // 🔹 Mostra tutorial apenas se for a primeira vez
        TryShowTutorial();

        isCapturing = false;
    }

    private void TryShowTutorial()
    {
        if (canvasTutorialHand == null || tutorialOkButton == null)
        {
            Debug.LogError("Tutorial: Canvas ou Botão não atribuídos!");
            return;
        }

        // Sempre mostrar
        canvasTutorialHand.SetActive(true);

        // Remove listeners antigos
        tutorialOkButton.onClick.RemoveAllListeners();

        // Fecha apenas quando o usuário clicar
        tutorialOkButton.onClick.AddListener(() =>
        {
            canvasTutorialHand.SetActive(false);
        });
    }




    private void OnConfirmClicked()
    {
        if (capturedTexture == null || isCapturing) return;

        ShowFeedback("📤 Preparando foto...");
        buttonConfirm.interactable = false;
        buttonBack.interactable = false;

        // Cria cópia reduzida e envia em background
        Texture2D resized = ResizeTexture(capturedTexture, 0.5f);
        byte[] jpgData = resized.EncodeToJPG(85);

        StartCoroutine(SendPhotoToServer(jpgData));

        ResetPhotoState(); // Usuário pode continuar
    }

    private void OnBackClicked()
    {
        if (isCapturing) return;

        if (capturedTexture != null)
            Destroy(capturedTexture);

        capturedTexture = null;
        photoPreviewImage.texture = null;

        canvasPreviewUI.SetActive(false);
        canvasCameraUI.SetActive(true);

        if (iconStampPanel != null)
            iconStampPanel.SetActive(false);

        ShowFeedback("Foto descartada");
    }

    private IEnumerator SendPhotoToServer(byte[] cleanImage)
    {
        // Aqui vamos capturar também a imagem com ícones
        Texture2D composedTexture = CapturePreviewWithIcons();
        Texture2D resizedComposed = ResizeTexture(composedTexture, 0.5f);
        byte[] composedImage = resizedComposed.EncodeToJPG(85);

        string baseName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        WWWForm form = new WWWForm();

        form.AddBinaryData("photo_clean", cleanImage, "foto_" + baseName + "_clean.jpg", "image/jpeg");
        form.AddBinaryData("photo_composed", composedImage, "foto_" + baseName + "_icons.jpg", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post("https://webhook.site/a7d15bed-2931-4ddb-8777-a4077b9a5b52", form))
        {
            www.timeout = 15;
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                ShowFeedback($"📤 Enviando... {(int)(www.uploadProgress * 100)}%");
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                photosSent++;
                ShowFeedback("✅ Fotos enviadas com sucesso!");
            }
            else
            {
                ShowFeedback("❌ Erro ao enviar: " + www.error);
            }
        }

        buttonConfirm.interactable = true;
        buttonBack.interactable = true;
    }

    private Texture2D ResizeTexture(Texture2D source, float scale)
    {
        int width = Mathf.RoundToInt(source.width * scale);
        int height = Mathf.RoundToInt(source.height * scale);
        Texture2D result = new Texture2D(width, height, source.format, false);
        RenderTexture rt = new RenderTexture(width, height, 24);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        rt.Release();
        return result;
    }

    private void ResetPhotoState()
    {
        if (capturedTexture != null)
            Destroy(capturedTexture);

        capturedTexture = null;
        photoPreviewImage.texture = null;

        canvasPreviewUI.SetActive(false);
        canvasCameraUI.SetActive(true);

        if (iconStampPanel != null)
            iconStampPanel.SetActive(false);
    }

    private void ShowFeedback(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        StopCoroutine(HideFeedback());
        StartCoroutine(HideFeedback());
    }

    private IEnumerator HideFeedback()
    {
        yield return new WaitForSeconds(2.5f);
        feedbackText.gameObject.SetActive(false);
    }

    private Texture2D CapturePreviewWithIcons()
    {
        // se não houver preview, volta a original
        if (canvasPreviewUI == null || photoPreviewImage == null)
            return capturedTexture;

        // tamanho exato da área do RawImage
        RectTransform rt = photoPreviewImage.rectTransform;

        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        // canto inferior esquerdo
        float x = corners[0].x;
        float y = corners[0].y;

        // largura e altura exatas do preview na tela
        float width = rt.rect.width;
        float height = rt.rect.height;

        // cria screenshot da tela completa
        Texture2D fullScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        fullScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        fullScreenshot.Apply();

        // cria a textura recortada no tamanho do preview
        Texture2D cropped = new Texture2D(Mathf.RoundToInt(width), Mathf.RoundToInt(height), TextureFormat.RGB24, false);

        // COPIA pixel a pixel recortando certinho o RawImage com os ícones
        Color[] px = fullScreenshot.GetPixels(
            Mathf.RoundToInt(x),
            Mathf.RoundToInt(y),
            Mathf.RoundToInt(width),
            Mathf.RoundToInt(height)
        );

        cropped.SetPixels(px);
        cropped.Apply();

        return cropped;
    }

}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System.IO;
using TMPro;

public class TutorialSelector : MonoBehaviour
{
    [System.Serializable]
    public class SpriteTutorial
    {
        public Sprite mainSprite;
        public List<Sprite> tutorialSteps;
    }

    [Header("UI References")]
    public List<Image> tutorialImages;
    private Image displayImage;
    public Button nextButton;
    public Button previousButton;
    public Button selectButton;
    public Button captureButton;

    public TextMeshProUGUI resultText;

    public GameObject Frame;

    [Header("Main Sprites with Tutorials")]
    public List<SpriteTutorial> spriteTutorials;

    [Header("AR Camera Capture Reference")]
    public ARCameraCapture cameraCapture;

    private int currentMainIndex = 0;
    private int currentStepIndex = 0;
    private bool inTutorialMode = false;

    private void Start()
    {

        nextButton.onClick.AddListener(OnNext);
        previousButton.onClick.AddListener(OnPrevious);
        selectButton.onClick.AddListener(OnSelect);
        
        captureButton.onClick.RemoveAllListeners();
        captureButton.onClick.AddListener(OnCaptureAndCompare);

        if (tutorialImages != null && tutorialImages.Count > 0)
        {
            displayImage = tutorialImages[0]; 
        }
        else
        {
            Debug.LogError("The 'Tutorial Images' list is empty! Please assign an Image in the Inspector.");
            return;
        }


        ShowMainSprite();

        if (cameraCapture == null)
        {
            cameraCapture = FindFirstObjectByType<ARCameraCapture>();
        }
    }

    private void ShowMainSprite()
    {
        if (spriteTutorials.Count == 0) return;
        displayImage.sprite = spriteTutorials[currentMainIndex].mainSprite;
    }

    private void OnNext()
    {
        if (!inTutorialMode)
        {
            currentMainIndex = (currentMainIndex + 1) % spriteTutorials.Count;
            ShowMainSprite();
        }
        else
        {
            var steps = spriteTutorials[currentMainIndex].tutorialSteps;
            if (steps.Count == 0) return;

            if (currentStepIndex < steps.Count - 1)
            {
                currentStepIndex++;
                displayImage.sprite = steps[currentStepIndex];
            }
            else
            {
                captureButton.gameObject.SetActive(true);
                Frame.SetActive(true);

                if (tutorialImages.Count > 1)
                {
                    tutorialImages[0].gameObject.SetActive(true);
                    tutorialImages[1].gameObject.SetActive(false);
                    displayImage = tutorialImages[0];
                    displayImage.sprite = steps[currentStepIndex];
                }

                previousButton.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(false);
                
                Debug.Log("You are on the finish line");
            }
        }
    }

    private void OnPrevious()
    {
        if (!inTutorialMode)
        {
            currentMainIndex = (currentMainIndex - 1 + spriteTutorials.Count) % spriteTutorials.Count;
            ShowMainSprite();
        }
        else
        {
            var steps = spriteTutorials[currentMainIndex].tutorialSteps;
            if (steps.Count == 0) return;

            if (currentStepIndex > 0)
            {
                currentStepIndex--;
                displayImage.sprite = steps[currentStepIndex];
            }
        }
    }

    private void OnSelect()
    {
        inTutorialMode = true;
        selectButton.gameObject.SetActive(false);

        if (tutorialImages.Count > 1)
        {
            tutorialImages[0].gameObject.SetActive(false);
            displayImage = tutorialImages[1];
        }

        var steps = spriteTutorials[currentMainIndex].tutorialSteps;
        if (steps.Count > 0)
        {
            currentStepIndex = 0;
            displayImage.sprite = steps[currentStepIndex];
        }
        else
        {
            Debug.LogWarning("No tutorial steps assigned for this sprite.");
        }
    }

    private void OnCaptureAndCompare()
    {
        if (cameraCapture == null)
        {
            Debug.LogError("ARCameraCapture not assigned!");
            return;
        }

        if(resultText == null)
        {
            Debug.LogError("ResultText not assigned!");
            return;
        }

        resultText.gameObject.SetActive(true);

        displayImage.gameObject.SetActive(false);
        Texture2D captured = cameraCapture.CaptureAndCropUI(displayImage.rectTransform);
        displayImage.gameObject.SetActive(true);

        Sprite currentSprite = spriteTutorials[currentMainIndex].tutorialSteps[currentStepIndex];
        
        Texture2D target = ImageProcessor.SpriteToTexture2D(currentSprite);
        Texture2D scaledCaptured = ImageProcessor.ScaleTexture(captured, target.width, target.height);
/*
        string scaledPath = Path.Combine(Application.persistentDataPath, "capture.png");
        File.WriteAllBytes(scaledPath, scaledCaptured.EncodeToPNG());
*/        
        Texture2D capturedFinal = ImageProcessor.DetectEdges(scaledCaptured, 0.12f);
        Texture2D targetFinal = ImageProcessor.DetectEdges(target, 0.12f); 
/*
        string capturedPath = Path.Combine(Application.persistentDataPath, "capture_edges.png");
        File.WriteAllBytes(capturedPath, capturedFinal.EncodeToPNG());
        
        string targetPath = Path.Combine(Application.persistentDataPath, "target_edges.png");
        File.WriteAllBytes(targetPath, targetFinal.EncodeToPNG());
*/
        int tolerance = 12;
        float similarity = ImageComparer.CompareWithTolerance(capturedFinal, targetFinal, tolerance);
        
        Debug.Log($"Similarity Score: {similarity * 100:F1}%");

        if (similarity > 0.75f)
           resultText.text = "Excellent! Your drawing closely matches the guide.";
        else if (similarity > 0.55f)
           resultText.text = "Good job! You're on the right track.";
        else
           resultText.text = "Keep trying! Make sure your lines follow the guide.";
    }
}
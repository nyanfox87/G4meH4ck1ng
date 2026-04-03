using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniGameSceneBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Mini-Game Scenes")]
    public static void BuildAllMiniGameScenes()
    {
        // Save current scene
        EditorSceneManager.SaveOpenScenes();

        BuildBakeScene();
        BuildIcingScene();
        BuildLobbyScene();

        // Add all scenes to build settings
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/BakeScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/IcingScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/LobbyScene.unity", true)
        };
        EditorBuildSettings.scenes = scenes;

        // Return to main scene
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        Debug.Log("[MiniGameSceneBuilder] All mini-game scenes built and added to build settings!");
    }

    private static void BuildBakeScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/BakeScene.unity");

        // Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.backgroundColor = new Color(0.1f, 0.08f, 0.15f);
        camGO.AddComponent<AudioListener>();
        camGO.tag = "MainCamera";

        // Canvas
        var canvasGO = new GameObject("BakeCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Background
        var bgGO = CreateUIElement("Background", canvasGO.transform);
        SetAnchors(bgGO, 0, 0, 1, 1);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.08f, 0.15f);

        // Title
        var titleGO = CreateUIElement("Title", canvasGO.transform);
        SetAnchors(titleGO, 0.1f, 0.88f, 0.9f, 0.98f);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = "THE PERFECT BAKE";
        titleText.fontSize = 36;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.85f, 0f);
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Slider
        var sliderGO = CreateUIElement("BakeSlider", canvasGO.transform);
        SetAnchors(sliderGO, 0.1f, 0.4f, 0.9f, 0.55f);
        var slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.interactable = false;

        // Slider Background
        var sliderBgGO = CreateUIElement("Background", sliderGO.transform);
        SetAnchors(sliderBgGO, 0, 0.25f, 1, 0.75f);
        var sliderBgImg = sliderBgGO.AddComponent<Image>();
        sliderBgImg.color = Color.gray;

        // Slider Fill Area
        var fillAreaGO = CreateUIElement("Fill Area", sliderGO.transform);
        SetAnchors(fillAreaGO, 0, 0.25f, 1, 0.75f);

        var fillGO = CreateUIElement("Fill", fillAreaGO.transform);
        SetAnchors(fillGO, 0, 0, 1, 1);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = Color.gray;

        slider.fillRect = fillGO.GetComponent<RectTransform>();

        // Position Text
        var posTextGO = CreateUIElement("PositionText", canvasGO.transform);
        SetAnchors(posTextGO, 0.3f, 0.28f, 0.7f, 0.4f);
        var posText = posTextGO.AddComponent<Text>();
        posText.text = "50.00";
        posText.fontSize = 48;
        posText.alignment = TextAnchor.MiddleCenter;
        posText.color = Color.white;
        posText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Result Text
        var resultTextGO = CreateUIElement("ResultText", canvasGO.transform);
        SetAnchors(resultTextGO, 0.1f, 0.12f, 0.9f, 0.26f);
        var resultText = resultTextGO.AddComponent<Text>();
        resultText.text = "";
        resultText.fontSize = 24;
        resultText.alignment = TextAnchor.MiddleCenter;
        resultText.color = new Color(0.5f, 1f, 0.5f);
        resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Back Button
        var backBtnGO = CreateUIElement("BackButton", canvasGO.transform);
        SetAnchors(backBtnGO, 0.02f, 0.02f, 0.15f, 0.1f);
        var backBtnImg = backBtnGO.AddComponent<Image>();
        backBtnImg.color = new Color(0.3f, 0.3f, 0.3f);
        var backBtn = backBtnGO.AddComponent<Button>();

        var backTextGO = CreateUIElement("BackText", backBtnGO.transform);
        SetAnchors(backTextGO, 0, 0, 1, 1);
        var backText = backTextGO.AddComponent<Text>();
        backText.text = "BACK";
        backText.fontSize = 18;
        backText.alignment = TextAnchor.MiddleCenter;
        backText.color = Color.white;
        backText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Instructions
        var instrGO = CreateUIElement("Instructions", canvasGO.transform);
        SetAnchors(instrGO, 0.1f, 0.58f, 0.9f, 0.7f);
        var instrText = instrGO.AddComponent<Text>();
        instrText.text = "Click to STOP the slider | Press ESC to resume";
        instrText.fontSize = 18;
        instrText.alignment = TextAnchor.MiddleCenter;
        instrText.color = new Color(0.7f, 0.7f, 0.7f);
        instrText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // BakeMinigame component
        var bakeComp = canvasGO.AddComponent<BakeMinigame>();
        bakeComp.bakeSlider = slider;
        bakeComp.sliderFill = fillImg;
        bakeComp.positionText = posText;
        bakeComp.resultText = resultText;
        bakeComp.backButton = backBtn;

        // EventSystem
        var eventGO = new GameObject("EventSystem");
        eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[MiniGameSceneBuilder] BakeScene built.");
    }

    private static void BuildIcingScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/IcingScene.unity");

        // Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.backgroundColor = new Color(0.08f, 0.1f, 0.18f);
        camGO.AddComponent<AudioListener>();
        camGO.tag = "MainCamera";

        // Canvas
        var canvasGO = new GameObject("IcingCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Background
        var bgGO = CreateUIElement("Background", canvasGO.transform);
        SetAnchors(bgGO, 0, 0, 1, 1);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.08f, 0.1f, 0.18f);

        // Title
        var titleGO = CreateUIElement("Title", canvasGO.transform);
        SetAnchors(titleGO, 0.1f, 0.88f, 0.9f, 0.98f);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = "ICING ARTIST";
        titleText.fontSize = 36;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.85f, 0f);
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Draw Area
        var drawAreaGO = CreateUIElement("DrawArea", canvasGO.transform);
        SetAnchors(drawAreaGO, 0.1f, 0.15f, 0.9f, 0.85f);
        var drawImg = drawAreaGO.AddComponent<Image>();
        drawImg.color = new Color(0.15f, 0.12f, 0.2f);

        // Target Circle Guide (visual hint)
        var circleGuideGO = CreateUIElement("CircleGuide", drawAreaGO.transform);
        circleGuideGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        circleGuideGO.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 300);
        var circleImg = circleGuideGO.AddComponent<Image>();
        circleImg.color = new Color(1f, 1f, 1f, 0.1f);

        // DrawCursor
        var cursorGO = CreateUIElement("DrawCursor", drawAreaGO.transform);
        cursorGO.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);
        var cursorImg = cursorGO.AddComponent<Image>();
        cursorImg.color = Color.yellow;

        // Precision Text
        var precTextGO = CreateUIElement("PrecisionText", canvasGO.transform);
        SetAnchors(precTextGO, 0.6f, 0.03f, 0.95f, 0.12f);
        var precText = precTextGO.AddComponent<Text>();
        precText.text = "Precision: 0.0%";
        precText.fontSize = 22;
        precText.alignment = TextAnchor.MiddleCenter;
        precText.color = Color.white;
        precText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Result Text
        var resultTextGO = CreateUIElement("ResultText", canvasGO.transform);
        SetAnchors(resultTextGO, 0.1f, 0.03f, 0.6f, 0.12f);
        var resultText = resultTextGO.AddComponent<Text>();
        resultText.text = "Trace a circle!";
        resultText.fontSize = 20;
        resultText.alignment = TextAnchor.MiddleLeft;
        resultText.color = new Color(0.5f, 1f, 0.5f);
        resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Back Button
        var backBtnGO = CreateUIElement("BackButton", canvasGO.transform);
        SetAnchors(backBtnGO, 0.02f, 0.9f, 0.12f, 0.98f);
        var backBtnImg = backBtnGO.AddComponent<Image>();
        backBtnImg.color = new Color(0.3f, 0.3f, 0.3f);
        var backBtn = backBtnGO.AddComponent<Button>();

        var backTextGO = CreateUIElement("BackText", backBtnGO.transform);
        SetAnchors(backTextGO, 0, 0, 1, 1);
        var backText = backTextGO.AddComponent<Text>();
        backText.text = "BACK";
        backText.fontSize = 16;
        backText.alignment = TextAnchor.MiddleCenter;
        backText.color = Color.white;
        backText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Clear Button
        var clearBtnGO = CreateUIElement("ClearButton", canvasGO.transform);
        SetAnchors(clearBtnGO, 0.85f, 0.9f, 0.98f, 0.98f);
        var clearBtnImg = clearBtnGO.AddComponent<Image>();
        clearBtnImg.color = new Color(0.6f, 0.3f, 0.1f);
        var clearBtn = clearBtnGO.AddComponent<Button>();

        var clearTextGO = CreateUIElement("ClearText", clearBtnGO.transform);
        SetAnchors(clearTextGO, 0, 0, 1, 1);
        var clearText = clearTextGO.AddComponent<Text>();
        clearText.text = "CLEAR";
        clearText.fontSize = 14;
        clearText.alignment = TextAnchor.MiddleCenter;
        clearText.color = Color.white;
        clearText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // IcingMinigame component
        var icingComp = canvasGO.AddComponent<IcingMinigame>();
        icingComp.drawArea = drawAreaGO.GetComponent<RectTransform>();
        icingComp.drawCursor = cursorImg;
        icingComp.precisionText = precText;
        icingComp.resultText = resultText;
        icingComp.backButton = backBtn;
        icingComp.clearButton = clearBtn;

        // EventSystem
        var eventGO = new GameObject("EventSystem");
        eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[MiniGameSceneBuilder] IcingScene built.");
    }

    private static void BuildLobbyScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/LobbyScene.unity");

        // Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.backgroundColor = new Color(0.05f, 0.08f, 0.12f);
        camGO.AddComponent<AudioListener>();
        camGO.tag = "MainCamera";

        // Canvas
        var canvasGO = new GameObject("LobbyCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Background
        var bgGO = CreateUIElement("Background", canvasGO.transform);
        SetAnchors(bgGO, 0, 0, 1, 1);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.05f, 0.08f, 0.12f);

        // Title
        var titleGO = CreateUIElement("Title", canvasGO.transform);
        SetAnchors(titleGO, 0.1f, 0.8f, 0.9f, 0.95f);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = "LONE WOLF LOBBY";
        titleText.fontSize = 40;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.3f, 0.3f);
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Player Count
        var countGO = CreateUIElement("PlayerCountText", canvasGO.transform);
        SetAnchors(countGO, 0.2f, 0.6f, 0.8f, 0.75f);
        var countText = countGO.AddComponent<Text>();
        countText.text = "Players: 1 / 2";
        countText.fontSize = 32;
        countText.alignment = TextAnchor.MiddleCenter;
        countText.color = Color.white;
        countText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Status Text
        var statusGO = CreateUIElement("StatusText", canvasGO.transform);
        SetAnchors(statusGO, 0.2f, 0.5f, 0.8f, 0.6f);
        var statusText = statusGO.AddComponent<Text>();
        statusText.text = "Waiting for players...";
        statusText.fontSize = 24;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = new Color(1f, 0.8f, 0.2f);
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Start Game Button
        var startBtnGO = CreateUIElement("StartGameBtn", canvasGO.transform);
        SetAnchors(startBtnGO, 0.3f, 0.3f, 0.7f, 0.45f);
        var startBtnImg = startBtnGO.AddComponent<Image>();
        startBtnImg.color = new Color(0.2f, 0.6f, 0.3f);
        var startBtn = startBtnGO.AddComponent<Button>();

        var startTextGO = CreateUIElement("StartText", startBtnGO.transform);
        SetAnchors(startTextGO, 0, 0, 1, 1);
        var startText = startTextGO.AddComponent<Text>();
        startText.text = "START GAME";
        startText.fontSize = 28;
        startText.alignment = TextAnchor.MiddleCenter;
        startText.color = Color.white;
        startText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Result Text
        var resultGO = CreateUIElement("ResultText", canvasGO.transform);
        SetAnchors(resultGO, 0.1f, 0.1f, 0.9f, 0.25f);
        var resultText = resultGO.AddComponent<Text>();
        resultText.text = "";
        resultText.fontSize = 22;
        resultText.alignment = TextAnchor.MiddleCenter;
        resultText.color = new Color(1f, 0.5f, 0.5f);
        resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Back Button
        var backBtnGO = CreateUIElement("BackButton", canvasGO.transform);
        SetAnchors(backBtnGO, 0.02f, 0.02f, 0.15f, 0.1f);
        var backBtnImg = backBtnGO.AddComponent<Image>();
        backBtnImg.color = new Color(0.3f, 0.3f, 0.3f);
        var backBtn = backBtnGO.AddComponent<Button>();

        var backTextGO = CreateUIElement("BackText", backBtnGO.transform);
        SetAnchors(backTextGO, 0, 0, 1, 1);
        var backText = backTextGO.AddComponent<Text>();
        backText.text = "BACK";
        backText.fontSize = 18;
        backText.alignment = TextAnchor.MiddleCenter;
        backText.color = Color.white;
        backText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // LobbyMinigame component
        var lobbyComp = canvasGO.AddComponent<LobbyMinigame>();
        lobbyComp.playerCountText = countText;
        lobbyComp.statusText = statusText;
        lobbyComp.resultText = resultText;
        lobbyComp.startGameButton = startBtn;
        lobbyComp.backButton = backBtn;

        // EventSystem
        var eventGO = new GameObject("EventSystem");
        eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[MiniGameSceneBuilder] LobbyScene built.");
    }

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>();
        return go;
    }

    private static void SetAnchors(GameObject go, float minX, float minY, float maxX, float maxY)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(minX, minY);
        rt.anchorMax = new Vector2(maxX, maxY);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

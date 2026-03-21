using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MossHarbor.UI
{
    public static class RuntimeUiFactory
    {
        public static Canvas CreateCanvas(string name)
        {
            var root = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            return canvas;
        }

        public static TMP_Text CreateLabel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, int fontSize, TextAlignmentOptions alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            var text = go.GetComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.text = name;
            return text;
        }

        public static Button CreateButton(Transform parent, string labelText, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var buttonGo = new GameObject(labelText + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(parent, false);

            var rect = buttonGo.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            var image = buttonGo.GetComponent<Image>();
            image.color = new Color(0.12f, 0.24f, 0.3f, 0.92f);

            var button = buttonGo.GetComponent<Button>();

            var text = CreateLabel(buttonGo.transform, labelText + "Label", Vector2.zero, Vector2.one, new Vector2(12f, 8f), new Vector2(-12f, -8f), 28, TextAlignmentOptions.Center);
            text.text = labelText;

            return button;
        }
    }
}

using MossHarbor.Art;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MossHarbor.UI
{
    public static class RuntimeUiFactory
    {
        private static Sprite s_buttonSprite;
        private static Sprite s_panelSprite;

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

        public static Image CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color tint)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            var image = go.GetComponent<Image>();
            image.sprite = PanelSprite;
            image.color = tint;
            image.type = Image.Type.Simple;
            return image;
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
            image.sprite = ButtonSprite;
            image.color = image.sprite != null ? new Color(0.16f, 0.22f, 0.27f, 0.94f) : new Color(0.12f, 0.24f, 0.3f, 0.92f);
            image.type = Image.Type.Simple;

            var button = buttonGo.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.92f, 0.96f, 1f, 1f);
            colors.pressedColor = new Color(0.82f, 0.9f, 0.98f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            var text = CreateLabel(buttonGo.transform, labelText + "Label", Vector2.zero, Vector2.one, new Vector2(12f, 8f), new Vector2(-12f, -8f), 22, TextAlignmentOptions.Center);
            text.text = labelText;
            text.color = Color.white;

            return button;
        }

        private static Sprite ButtonSprite => s_buttonSprite != null ? s_buttonSprite : s_buttonSprite = Resources.Load<Sprite>(ArtResourcePaths.UiPrimaryButton);
        private static Sprite PanelSprite => s_panelSprite != null ? s_panelSprite : s_panelSprite = Resources.Load<Sprite>(ArtResourcePaths.UiPanelRound);
    }
}

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

        public static Image CreateProgressBar(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color backgroundColor, Color fillColor)
        {
            // Background
            var bgGo = new GameObject(name + "Bg", typeof(RectTransform), typeof(Image));
            bgGo.transform.SetParent(parent, false);
            var bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = anchorMin;
            bgRect.anchorMax = anchorMax;
            bgRect.offsetMin = offsetMin;
            bgRect.offsetMax = offsetMax;
            var bgImage = bgGo.GetComponent<Image>();
            bgImage.color = backgroundColor;

            // Fill
            var fillGo = new GameObject(name + "Fill", typeof(RectTransform), typeof(Image));
            fillGo.transform.SetParent(bgGo.transform, false);
            var fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0f, 1f); // starts empty
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fillGo.GetComponent<Image>();
            fillImage.color = fillColor;

            return fillImage; // Caller sets anchorMax.x to fill ratio (0-1)
        }

        public static TMP_Text CreateIconLabel(Transform parent, string name, string iconChar, string labelText, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, int fontSize, Color iconColor)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            // Icon (left side)
            var iconGo = new GameObject(name + "Icon", typeof(RectTransform), typeof(TextMeshProUGUI));
            iconGo.transform.SetParent(go.transform, false);
            var iconRect = iconGo.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = new Vector2(fontSize + 4, 0f);
            var iconText = iconGo.GetComponent<TextMeshProUGUI>();
            iconText.text = iconChar;
            iconText.fontSize = fontSize;
            iconText.alignment = TextAlignmentOptions.Center;
            iconText.color = iconColor;

            // Label (right side)
            var labelGo = new GameObject(name + "Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(fontSize + 8, 0f);
            labelRect.offsetMax = Vector2.zero;
            var label = labelGo.GetComponent<TextMeshProUGUI>();
            label.text = labelText;
            label.fontSize = fontSize;
            label.alignment = TextAlignmentOptions.Left;
            label.color = Color.white;

            return label; // Caller updates text
        }

        public static TMP_Text CreateToast(Transform parent, string message, float duration = 2f)
        {
            var go = new GameObject("Toast", typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(-200f, 20f);
            rect.offsetMax = new Vector2(200f, 60f);

            // Background
            var bgGo = new GameObject("ToastBg", typeof(RectTransform), typeof(Image));
            bgGo.transform.SetParent(go.transform, false);
            var bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = bgGo.GetComponent<Image>();
            bgImage.color = new Color(0.08f, 0.14f, 0.18f, 0.88f);

            var text = CreateLabel(go.transform, "ToastText", Vector2.zero, Vector2.one, new Vector2(12f, 4f), new Vector2(-12f, -4f), 18, TextAlignmentOptions.Center);
            text.text = message;
            text.color = new Color(0.9f, 0.95f, 0.85f, 1f);

            return text;
        }

        public static Image CreateStarIcon(Transform parent, string name, Vector2 position, float size, bool filled)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = position;
            var image = go.GetComponent<Image>();
            image.color = filled ? new Color(1f, 0.85f, 0.28f, 1f) : new Color(0.3f, 0.3f, 0.3f, 0.5f);
            return image;
        }

        private static Sprite ButtonSprite => s_buttonSprite != null ? s_buttonSprite : s_buttonSprite = Resources.Load<Sprite>(ArtResourcePaths.UiPrimaryButton);
        private static Sprite PanelSprite => s_panelSprite != null ? s_panelSprite : s_panelSprite = Resources.Load<Sprite>(ArtResourcePaths.UiPanelRound);
    }
}

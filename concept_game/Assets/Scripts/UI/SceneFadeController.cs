using UnityEngine;
using UnityEngine.UI;

namespace MossHarbor.UI
{
    public sealed class SceneFadeController : MonoBehaviour
    {
        private static SceneFadeController s_instance;
        private CanvasGroup _fadeGroup;
        private float _fadeTarget;
        private float _fadeSpeed;
        private System.Action _onFadeComplete;

        public static SceneFadeController Instance => s_instance;
        public bool IsFading => _fadeGroup != null && !Mathf.Approximately(_fadeGroup.alpha, _fadeTarget);

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
            BuildFadeCanvas();
        }

        private void Update()
        {
            if (_fadeGroup == null || Mathf.Approximately(_fadeGroup.alpha, _fadeTarget))
            {
                return;
            }

            _fadeGroup.alpha = Mathf.MoveTowards(_fadeGroup.alpha, _fadeTarget, _fadeSpeed * Time.unscaledDeltaTime);
            if (Mathf.Approximately(_fadeGroup.alpha, _fadeTarget))
            {
                _fadeGroup.blocksRaycasts = _fadeTarget > 0.5f;
                _onFadeComplete?.Invoke();
                _onFadeComplete = null;
            }
        }

        public void FadeOut(float duration = 0.4f, System.Action onComplete = null)
        {
            _fadeTarget = 1f;
            _fadeSpeed = duration > 0f ? 1f / duration : 100f;
            _onFadeComplete = onComplete;
            if (_fadeGroup != null)
            {
                _fadeGroup.blocksRaycasts = true;
            }
        }

        public void FadeIn(float duration = 0.4f, System.Action onComplete = null)
        {
            _fadeTarget = 0f;
            _fadeSpeed = duration > 0f ? 1f / duration : 100f;
            _onFadeComplete = onComplete;
        }

        public void SetOpaque()
        {
            if (_fadeGroup != null)
            {
                _fadeGroup.alpha = 1f;
                _fadeGroup.blocksRaycasts = true;
            }
        }

        public void SetClear()
        {
            if (_fadeGroup != null)
            {
                _fadeGroup.alpha = 0f;
                _fadeGroup.blocksRaycasts = false;
            }
        }

        private void BuildFadeCanvas()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            _fadeGroup = gameObject.AddComponent<CanvasGroup>();
            _fadeGroup.alpha = 0f;
            _fadeGroup.blocksRaycasts = false;

            var imageGo = new GameObject("FadeOverlay", typeof(RectTransform), typeof(Image));
            imageGo.transform.SetParent(transform, false);
            var rect = imageGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = imageGo.GetComponent<Image>();
            image.color = new Color(0.02f, 0.04f, 0.06f, 1f);
            image.raycastTarget = true;
        }
    }
}

using MossHarbor.Expedition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MossHarbor.UI
{
    public sealed class HudController : MonoBehaviour
    {
        [SerializeField] private ExpeditionDirector expeditionDirector;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text districtText;
        [SerializeField] private TMP_Text objectiveText;

        private void Start()
        {
            if (expeditionDirector == null)
            {
                expeditionDirector = FindFirstObjectByType<ExpeditionDirector>();
            }

            if (timerText == null)
            {
                BuildUi();
            }
        }

        private void Update()
        {
            if (expeditionDirector == null || timerText == null)
            {
                return;
            }

            if (districtText != null)
            {
                districtText.text =
                    $"{expeditionDirector.DistrictDisplayName}\n" +
                    $"Quest: {expeditionDirector.QuestDisplayName}\n" +
                    $"Recommended Power: {Mathf.Max(1, expeditionDirector.RuntimeDistrict != null ? expeditionDirector.RuntimeDistrict.recommendedPower : 1)}";
                districtText.color = Color.Lerp(expeditionDirector.DistrictThemeColor, Color.white, 0.25f);
            }

            var totalSeconds = Mathf.Max(0, Mathf.CeilToInt(expeditionDirector.RemainingTime));
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (objectiveText != null)
            {
                objectiveText.text = expeditionDirector.ObjectiveReady
                    ? $"Beacon Ready\nReturn to the beacon in {expeditionDirector.DistrictDisplayName}\n{expeditionDirector.ObjectiveProgressText}"
                    : $"{expeditionDirector.ObjectiveDescription}\n{expeditionDirector.ObjectiveProgressText}";
            }
        }

        private void BuildUi()
        {
            var canvas = RuntimeUiFactory.CreateCanvas("ExpeditionCanvas");

            var title = RuntimeUiFactory.CreateLabel(canvas.transform, "ExpeditionTitle", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-240f, -100f), new Vector2(240f, -30f), 42, TextAlignmentOptions.Center);
            title.text = expeditionDirector != null ? expeditionDirector.DistrictDisplayName : "Expedition";

            timerText = RuntimeUiFactory.CreateLabel(canvas.transform, "TimerText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-120f, -190f), new Vector2(120f, -120f), 54, TextAlignmentOptions.Center);
            districtText = RuntimeUiFactory.CreateLabel(canvas.transform, "DistrictText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-360f, -250f), new Vector2(360f, -180f), 24, TextAlignmentOptions.Center);
            objectiveText = RuntimeUiFactory.CreateLabel(canvas.transform, "ObjectiveText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-360f, -350f), new Vector2(360f, -250f), 24, TextAlignmentOptions.Center);

            var completeButton = RuntimeUiFactory.CreateButton(canvas.transform, "Complete Run", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-360f, 40f), new Vector2(-40f, 110f));
            completeButton.onClick.AddListener(() => expeditionDirector?.CompleteCurrentRun());

            var failButton = RuntimeUiFactory.CreateButton(canvas.transform, "Fail Run", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-360f, 130f), new Vector2(-40f, 200f));
            failButton.onClick.AddListener(() => expeditionDirector?.FailCurrentRun());
        }
    }
}

using System.Collections.Generic;
using MossHarbor.Data;
using NUnit.Framework;

namespace MossHarbor.Tests.EditMode
{
    public sealed class TutorialStateRulesTests
    {
        [Test]
        public void DetermineInitialStage_NewSave_StartsTutorial()
        {
            var data = new SaveData();

            Assert.AreEqual(TutorialStage.StartFirstExpedition, TutorialStateRules.DetermineInitialStage(data));
        }

        [Test]
        public void DetermineInitialStage_SaveWithUpgradeProgress_SkipsTutorial()
        {
            var data = new SaveData();
            data.hubUpgradeLevels.FromDictionary(new Dictionary<string, int>
            {
                { "harbor_pump", 1 },
            });

            Assert.AreEqual(TutorialStage.Completed, TutorialStateRules.DetermineInitialStage(data));
        }

        [Test]
        public void HasMeaningfulProgress_RunSummaryOutcome_IsDetected()
        {
            var data = new SaveData
            {
                lastRunSummary = new RunSummary
                {
                    resultLabel = "Run Failed",
                    durationSeconds = 12f,
                },
            };

            Assert.IsTrue(TutorialStateRules.HasMeaningfulProgress(data));
        }
    }
}

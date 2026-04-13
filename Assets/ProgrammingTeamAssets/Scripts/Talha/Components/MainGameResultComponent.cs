using Game.Extensions;
using Game.Scriptables;
using Game.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Game.Components
{
    public class MainGameResultComponent : MonoBehaviour
    {
        private MainGameplayComponent mMainGameplayComponent;
        private MainGameplayComponent MainGameplayComponent
        {
            get
            {
                if (mMainGameplayComponent == null)
                {
                    mMainGameplayComponent = GetComponentInParent<MainGameplayComponent>();
                }
                return mMainGameplayComponent;
            }
        }

        public GameObject Container;
        public TextComponent AIOptionText;
        public TextComponent PlayerOptionText;
        public TextComponent ResultText;
        public List<RuleSettingData> RuleSets;

        private void Awake()
        {
            MainGameplayComponent.ActionShowResult += ShowResult;
        }

        private void ShowResult(string PlayerOption,string AIOption)
        {
            var CurrentRule = RuleSets.FirstOrDefault(x => x.RuleID.ID == PlayerOption);

            Container.SetActive(true);
            AIOptionText.SetupText(MainGameplayComponent.AIOption);
            PlayerOptionText.SetupText(MainGameplayComponent.PlayerOption);

            //if (PlayerOption == AIOption)
            //{
            //    ResultText.SetupText("It's a Draw");
            //    this.RunAfter(3f, () =>
            //    {
            //        MainGameplayComponent.OptionSelection.StartGameplay();
            //        Container.SetActive(false);
            //    });
            //}
            //else if(CurrentRule.WinAgainst.Any(x => x.ID == AIOption))
            //{
            //    ResultText.SetupText("You won");
            //    this.RunAfter(3f, () =>
            //    {
            //        MainGameplayComponent.ActionStartParagraphGame?.Invoke(PlayerOption);
            //        Container.SetActive(false);
            //    });
            //}
            //else
            {
                ResultText.SetupText("Opponent Won");
                this.RunAfter(3f, () =>
                {
                    MainGameplayComponent.ActionStartMinigame?.Invoke(AIOption);
                    Container.SetActive(false);
                });
            }
        }

    }
}
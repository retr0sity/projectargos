using Game.Extensions;
using Game.Managers;
using Game.Popups;
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

        private PopupOkay mPopupOkay;
        private PopupOkay PopupOkay
        {
            get
            {
                if(mPopupOkay == null)
                {
                    mPopupOkay = DependencyManager.Instance.PopupManager.GetPopup<PopupOkay>();
                }
                return mPopupOkay;
            }
        }

        public GameObject Container;
        public TextComponent AIOptionText;
        public TextComponent PlayerOptionText;
        public TextComponent ResultText;
        public List<RuleSettingData> RuleSets;

        public List<Sprite> SpritesWon;
        public Sprite SpriteLost1;
        public Sprite SpriteLost2;
        public Sprite SpriteDraw;

        private int NumberOfWins;

        private void Awake()
        {
            MainGameplayComponent.ActionShowResult += ShowResult;
        }

        private void ShowResult(string PlayerOption, string AIOption)
        {
            var CurrentRule = RuleSets.FirstOrDefault(x => x.RuleID.ID == PlayerOption);

            Container.SetActive(true);
            AIOptionText.SetupText(MainGameplayComponent.AIOption);
            PlayerOptionText.SetupText(MainGameplayComponent.PlayerOption);

            //if (PlayerOption == AIOption)
            //{
            //    ResultText.SetupText("It's a Draw");
            //    PopupOkay.Show(SpriteDraw, () =>
            //    {
            //        //this.RunAfter(3f, () =>
            //        //{
            //        MainGameplayComponent.OptionSelection.StartGameplay();
            //        Container.SetActive(false);
            //        //});
            //    });
            //}
            //else if (CurrentRule.WinAgainst.Any(x => x.ID == AIOption))
            {
                ResultText.SetupText("You won");
                PopupOkay.Show(SpritesWon[NumberOfWins], () =>
                {
                    //this.RunAfter(3f, () =>
                    //{
                    MainGameplayComponent.ActionStartParagraphGame?.Invoke(PlayerOption);
                    Container.SetActive(false);
                    //});
                });
                if (NumberOfWins < SpritesWon.Count - 1)
                    NumberOfWins++;
            }
            //else
            //{
            //    ResultText.SetupText("Opponent Won");
            //    PopupOkay.Show(SpriteLost1, () =>
            //    {
            //        PopupOkay.Show(SpriteLost2, () =>
            //        {
            //            //this.RunAfter(3f, () =>
            //            //{
            //            MainGameplayComponent.ActionStartMinigame?.Invoke(AIOption);
            //            Container.SetActive(false);
            //            //});
            //        });
            //    });
            //}
        }
    }
}
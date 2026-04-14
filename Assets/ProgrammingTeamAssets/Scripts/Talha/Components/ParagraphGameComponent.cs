using Game.Scriptables;
using Game.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Components
{
    public class ParagraphGameComponent : OptionSelectionComponent
    {
        //private MainGameplayComponent mMainGameplayComponent;
        //private MainGameplayComponent MainGameplayComponent
        //{
        //    get
        //    {
        //        if(mMainGameplayComponent == null)
        //        {
        //            mMainGameplayComponent = GetComponentInParent<MainGameplayComponent>();
        //        }
        //        return mMainGameplayComponent;
        //    }
        //}

        //public GameObject Container;
        public StringData DefaultPage;
        public TextComponent TextPageNumber;

        public List<PuzzleTextComponent> PuzzleTexts;

        private void Awake()
        {
            MainGameplayComponent.ActionStartParagraphGame += StartGame;
            ActionOptionChanged += ChangePageNumber;
        }

        private void ChangePageNumber(string ID)
        {
            TextPageNumber.SetupText((CurrentIndex+1).ToString()+"/"+ Options.Count);
        }

        private void StartGame(string ID)
        {
            StartGameplay();
        }



        protected override void OptionSelected(InputAction.CallbackContext context)
        {
            //base.OptionSelected(context);
        }

        public void WordCompleted(string ID)
        {
            var Puzzle = PuzzleTexts.FirstOrDefault(x => x.ID == ID);
            if(Puzzle != null)
            {
                Puzzle.gameObject.SetActive(true);
            }
        }
    }
}
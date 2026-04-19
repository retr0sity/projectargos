using UnityEngine;
using TMPro;
using Game.Utilities;
using Game.Components;
using System.Collections.Generic;

public class HangmanGameComponent : MonoBehaviour
{
    private MainGameplayComponent mMainGameplayComponent;
    private MainGameplayComponent MainGameplayComponent
    {
        get
        {
            if(mMainGameplayComponent == null)
            {
                mMainGameplayComponent = FindObjectOfType<MainGameplayComponent>();
            }
            return mMainGameplayComponent;
        }
    }

    private ParagraphGameComponent mParagraphGameComponent;
    private ParagraphGameComponent ParagraphGameComponent
    {
        get
        {
            if (mParagraphGameComponent == null)
            {
                mParagraphGameComponent = FindObjectOfType<ParagraphGameComponent>();
            }
            return mParagraphGameComponent;
        }
    }


    public string targetWord = "CAT";
    public int maxLetters = 3;

    public TextComponent displayText;

    private char[] letters;
    private int currentIndex = 0;

    private bool CanPlay;
    private int CurrentWord;
    private List<string> TargetWords;
    public GameObject Container;
    private void Start()
    {
        MainGameplayComponent.ActionStartHangmanGame += StartGame;
    }
    private void StartGame(List<string> TargetWords)
    {
        this.TargetWords = TargetWords;
        Container.gameObject.SetActive(true);
        currentIndex = 0;
        StartWord();
    }
    private void StartWord()
    {
        Debug.Log(CurrentWord.ToString()+" : "+ TargetWords.Count);
        currentIndex = 0;
        if (CurrentWord < TargetWords.Count)
        {
            targetWord = TargetWords[CurrentWord].ToUpper();
            maxLetters = targetWord.Length;
            letters = new char[maxLetters];

            for (int i = 0; i < letters.Length; i++)
            {
                letters[i] = '_';
            }

            UpdateDisplay();
            CanPlay = true;
        }
        else
        {
            Debug.Log("All Words Completed");

            CanPlay = false;
            Container.SetActive(false);
            MainGameplayComponent.ActionStartMainGameplay?.Invoke();
        }
    }


    void Update()
    {
        if (!CanPlay)
            return;
        // Backspace
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveLast();
            return;
        }

        // Input
        if (Input.anyKeyDown)
        {
            string input = Input.inputString.ToUpper();

            if (!string.IsNullOrEmpty(input))
            {
                char c = input[0];

                if (c >= 'A' && c <= 'Z')
                {
                    Fill(c);
                }
            }
        }
    }

    void Fill(char letter)
    {
        if (currentIndex >= letters.Length)
            return;

        letters[currentIndex] = letter;
        currentIndex++;

        UpdateDisplay();

        if (currentIndex == maxLetters)
        {
            CheckWord();
        }
    }

    void RemoveLast()
    {
        if (currentIndex <= 0)
            return;

        currentIndex--;
        letters[currentIndex] = '_';

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        string result = "";

        for (int i = 0; i < letters.Length; i++)
        {
            if (i == currentIndex)
            {
                // 🔴 Red highlight for current slot
                result += "<color=red>" + letters[i] + "</color> ";
            }
            else
            {
                result += letters[i] + " ";
            }
        }

        displayText.SetupText(result);
    }

    void CheckWord()
    {
        string result = "";
        bool Correct = true;
        for (int i = 0; i < letters.Length; i++)
        {
            char letter = letters[i];

            if (letter == targetWord[i])
            {
                result += "<color=green>" + letter + "</color> ";
            }
            else if (targetWord.Contains(letter.ToString()))
            {
                Correct = false;
                result += "<color=orange>" + letter + "</color> ";
            }
            else
            {
                Correct = false;
                result += "<color=grey>" + letter + "</color> ";
            }
        }
        displayText.SetupText(result);

        if (Correct)
        {
            ParagraphGameComponent.WordCompleted(TargetWords[CurrentWord]);
            CurrentWord++;
            StartWord();
        }
    }
}
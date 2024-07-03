using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject[] playerSpawnPoint;
    public static float collectedFragments, KeyItemsCollected;
    private float Timer = 2f, ResetTimer;
    public GameObject DiaBox, DiaText, fadeIn, fadeOut, CutsceneDiaBox, CutSceneText, Options, RegularPlayer, StamBar, OptionsBut, StartBut, Diff;
    Button B1, B2, B3;
    public GameObject CutsceneCam;
    public int DialogueNum, DiaNum, CutSceneDiaNum;
    public string[] Dialogues, BedroomDialogues, GameDialogues, CutSceneDialogues, CutSceneDiaOptions;
    public static bool ChangeLevel, DialogueOnScreen, Cutscene = false, End = false;
    public static int Level, Difficulty, LevelDiaNum;
    private bool once = false, CutsceneOnce = false, Monster = false, MonsterOnce = false, EndingOnce = false, Awake = false, fragOnce = false;
    private string currentDialogue = "";
    private int textIndex = 0; 

    private void Start()
    {
        InitializeGameObjects();
        InitializeState();
    }
    private void InitializeGameObjects() // Look for and assign variables and game objects
    {
        B1 = GameObject.Find("1").GetComponent<Button>();
        B2 = GameObject.Find("2").GetComponent<Button>();
        B3 = GameObject.Find("3").GetComponent<Button>();
        B1.onClick.AddListener(EasyMode);
        B2.onClick.AddListener(NormalMode);
        B3.onClick.AddListener(HardMode);
        StartBut = GameObject.Find("Start");
        Diff = GameObject.Find("Difficulty");
        OptionsBut = GameObject.Find("OptionsButt");
        StamBar = GameObject.Find("StamBar");
        CutsceneDiaBox = GameObject.Find("CutsceneDiaBox");
        CutSceneText = GameObject.Find("CutsceneDialogue");
        Options = GameObject.Find("Options");
        fadeIn = GameObject.Find("FadeIn");
        fadeOut = GameObject.Find("Fade");
        DiaBox = GameObject.Find("TextPanel");
        DiaText = GameObject.Find("Dialogue");
        StamBar.SetActive(false);
        Diff.SetActive(false);
        fadeIn.SetActive(false);
        fadeOut.SetActive(false);
        CutSceneText.SetActive(false);
        CutsceneDiaBox.SetActive(false);
        Options.SetActive(false);
        DiaBox.SetActive(false);
    }
    public void EasyMode()
    {
        Difficulty = 1;
        OptionsBut.SetActive(true);
        StartBut.SetActive(true);
        Diff.SetActive(false);
    }
    public void NormalMode()
    {
        Difficulty = 2;
        OptionsBut.SetActive(true);
        StartBut.SetActive(true);
        Diff.SetActive(false);
    }
    public void HardMode()
    {
        Difficulty = 3;
        OptionsBut.SetActive(true);
        StartBut.SetActive(true);
        Diff.SetActive(false);
    }
    public void OptionsButt()
    {
        OptionsBut.SetActive(false);
        StartBut.SetActive(false);
        Diff.SetActive(true);
    }
    private void InitializeState() // Set the reset timer value and current level
    {
        Level = 0;
        DontDestroyOnLoad(gameObject);
        ResetTimer = Timer;
    }
    public void Play() // Starting the game
    {
        GameObject.Find("Game").SetActive(false);
        GameObject.Find("Start").SetActive(false);
        StamBar.SetActive(true);
        OptionsBut.SetActive(false);
        Level += 1;
        SceneManager.LoadScene("SceneGame");
        ToggleFadeOut(true);
    }
    private void Update()
    {
        Dialogue();
        LevelChange();
        CutsceneTrigger();
        Ending();
        Exit();
        LevelDiaNum = DiaNum;
        if(!fragOnce && KeyItemsCollected == 1) // Display diaglogue when player first collects a key item
        {
            fragOnce = true;   
            DisplayDialogueBox();
        }
        if(MobSpawner.firstSpawned && !Monster) // Display dialogue when player first encounters a monster
        {
            DiaNum = 4;
            Monster = true;
            DisplayDialogueBox();
        }
        if(Input.GetKeyDown((KeyCode.O)))
        {
            CloseText();
        }
    }
    private void Dialogue() // Display story dialogues based on a timer
    {
        if(!once && Level >= 1 && Level < 4)
        {
            Timer -= Time.deltaTime;
            if(Timer <= 0 && DiaNum < 4)
            {
                DisplayDialogueBox();
                Timer = ResetTimer;
                once = true;
            }
        }
        if(!once && Level == 4 && DiaNum == 0)
        {
            DisplayDialogueBox();
            once = true;
        }
    }
    private void DisplayDialogueBox() // Allowing the dialogue box finish animating before the text begins to appear
    {
        DiaBox.SetActive(true);
        Invoke(nameof(ChangingText), 1.2f);
    }
    private void LevelChange() // Fades the player out before changing levels
    {
        if(ChangeLevel)
        {
            fadeIn.SetActive(true);
            fadeOut.SetActive(false);
            Invoke(nameof(ChangingLevel), 1.5f);
        }
    }
    private void CutsceneTrigger() // Triggering the cutscene once requirements are met
    {
        if(collectedFragments == 10 && CutsceneOnce == false)
        {
            CutsceneOnce = true;
            CutsceneCam = GameObject.FindGameObjectWithTag("CutSceneCam");
            Invoke(nameof(CutScene), 1);
        }
    }
    private void Ending() // Ends the game once requirements are met
    {
        if(!EndingOnce && End == true)
        {
            CutSceneDiaNum = 8;
            Invoke(nameof(ChangingText), 1.4f);
            EndingOnce = true;
            TransitionToEnding();
        }
    }
    private void TransitionToEnding() // Sends the player to a cutscene before the ending
    {
        RegularPlayer = GameObject.Find("Player");
        RegularPlayer.SetActive(false);
        CutsceneCam.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CutsceneDiaBox.SetActive(true);
    }
    private void Exit() // Allows the player to close the game at anytime
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    private void CutScene() // Triggers the cutscene
    {
        RegularPlayer = GameObject.Find("Player");
        Cutscene = true;
        ToggleFadeOut(true);
        RegularPlayer.SetActive(false);
        StamBar.SetActive(false);
        CutsceneCam.SetActive(true);
        CutsceneDiaBox.SetActive(true);
        Options.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DialogueOnScreen = true;
        DiaBox.SetActive(false);
        Invoke(nameof(ChangingText), 1.5f);
    }
    private void ChangingLevel() // Changes the level 
    {
        if(ChangeLevel && !DialogueOnScreen)
        {
            switch(Level)
            {
                case 2:
                    SceneManager.LoadScene("Bedroom");
                    break;
                case 3:
                    SceneManager.LoadScene("MainGame");
                    break;
                case 4:
                    SceneManager.LoadScene("BedroomEnding");
                    RegularPlayer = GameObject.Find("Player");
                    break;
            }
            ChangeLevel = false;
            ResetDialogueNumbers();
            Invoke(nameof(StartFade), 2f);
        }
    }
    private void ResetDialogueNumbers() // Resets the dialogue numbers every scene change
    {
        DiaNum = 0;
        DialogueNum = 0;
    }
    private void StartFade() // Fades out the player and closes the game if the player is at the last level
    {
        DiaBox.SetActive(true);
        DiaText.SetActive(true);
        fadeIn.SetActive(false);
        ToggleFadeOut(true);
        if(Level == 4)
        {
            Application.Quit();
        }
    }
    public void NumAdd() // Adds a number to the dialogue number 
    {
        CutSceneDiaNum += 1;
    }
    public void ChangingText() // Creates and changes the text based on levels and situations
    {
        if(!Cutscene)
        {
            RegularDialogue();
        }
        else
        {
            CutsceneDialogue();
        }
        if(Monster && !MonsterOnce)
        {
            DisplayDialogueBox();
        }
    }
    private void RegularDialogue() // Normal dialogues such as the story and first time dialogues
    {
        DiaText.SetActive(true);
        DialogueOnScreen = true;
        if(Monster && MonsterOnce == false)
        {
            DisplayDialogueLetterByLetter(GameDialogues[4]);
            MonsterOnce = true;
            Debug.Log("MobTest0");
        }
        if(Level == 1 && DiaNum < 4)
        {
            DisplayDialogueLetterByLetter(Dialogues[DialogueNum]);
            DiaNum += 1;
        }
        else if(Level == 2 && DiaNum <4)
        {
            DisplayDialogueLetterByLetter(BedroomDialogues[DialogueNum]);
            DiaNum += 1;
        }
        else if(Level == 3 && DiaNum < 4)
        {
            DisplayDialogueLetterByLetter(GameDialogues[DialogueNum]);
            DiaNum += 1;
        }
        if(Level == 4)
        {
            DisplayDialogueLetterByLetter(BedroomDialogues[4]);
        }
        if(KeyItemsCollected == 1)
        {
            DisplayDialogueLetterByLetter(GameDialogues[5]);
        }
        if(CutSceneDiaNum > 7)
        {
            CutSceneText.SetActive(true);
            DisplayDialogueLetterByLetter(CutSceneDialogues[7]);
            End = true;
        }
        Invoke(nameof(CloseText), 4f);
    }
    private void DisplayDialogueLetterByLetter(string dialogue) // For the dialogue text to appear letter by letter
    {
        currentDialogue = dialogue;
        textIndex = 0;
        InvokeRepeating(nameof(AddNextLetter), 0f, 0.01f); 
    }
    private void AddNextLetter() // For the dialogue text to appear letter by letter
    {
        if(DiaText.activeSelf)
        { 
        DiaText.GetComponent<TextMeshProUGUI>().SetText(currentDialogue.Substring(0, textIndex++));
        }
        if(CutSceneText.activeSelf)
        {
        CutSceneText.GetComponent<TextMeshProUGUI>().SetText(currentDialogue.Substring(0, textIndex++));
        }
        if(textIndex > currentDialogue.Length)
        {
            CancelInvoke(nameof(AddNextLetter));
        }
    }
    private void CutsceneDialogue() // Showing the cutscene dialogues
    {
        if(DiaBox.activeSelf)
        {
            DiaBox.SetActive(false);
        }
        fadeOut.SetActive(false);
        CutSceneText.SetActive(true);
        CutSceneText.GetComponent<TextMeshProUGUI>().SetText(CutSceneDialogues[CutSceneDiaNum]);
        if(CutSceneDiaNum < 6)
        {
        UpdateCutsceneOptions();
        }
        if(CutSceneDiaNum == 6)
        {
            EndCutscene();
        }
    }
    private void UpdateCutsceneOptions() // Updates the options during the cutscene
    {
        GameObject OptionText = GameObject.Find("OptionText");
        OptionText.GetComponent<TextMeshProUGUI>().SetText(CutSceneDiaOptions[CutSceneDiaNum]);
        if(CutSceneDiaNum == 1)
        {
            GameObject Option2 = GameObject.Find("Option2");
            Option2.SetActive(false);
        }
    }
    private void EndCutscene() // Ends the cutscene state and reverts the player back to their original position
    {
        CutsceneDiaBox.SetActive(false);
        ToggleFadeOut(true);
        CutsceneCam.SetActive(false);
        CutSceneText.SetActive(false);
        CutsceneDiaBox.SetActive(false);
        Options.SetActive(false);
        StamBar.SetActive(true);
        RegularPlayer.SetActive(true);
        Cutscene = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void CloseText() // Closes the dialogue box and text
    {
        DialogueOnScreen = false;
        if(Level == 1)
        {
            DialogueNum += 1;
        }
        else if(Level == 2 || Level == 3)
        {
            if(DialogueNum < 3)
            {
                DialogueNum += 1;
            }
        }
        DiaBox.SetActive(false);
        DiaText.SetActive(false);
        DiaText.GetComponent<TextMeshProUGUI>().SetText("");
        once = false;
        if(End == true)
        {
            Level += 1;
            CutsceneDiaBox.SetActive(false);
            ChangeLevel = true;
            Invoke(nameof(ChangingLevel), 0.2f);
        }
    }
    private void ToggleFadeOut(bool state)
    {
        fadeOut.SetActive(state);
    }
}
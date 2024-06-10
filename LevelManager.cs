using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject[] playerSpawnPoint;
    public static float collectedFragments, KeyItemsCollected;
    private float Timer = 2f, ResetTimer;
    private GameObject DiaBox, DiaText, fadeIn, fadeOut, CutsceneDiaBox, CutSceneText, Options, RegularPlayer, StamBar;
    public GameObject CutsceneCam;
    public int DialogueNum, DiaNum, CutSceneDiaNum;
    public string[] Dialogues, BedroomDialogues, GameDialogues, CutSceneDialogues, CutSceneDiaOptions;
    public static bool ChangeLevel, DialogueOnScreen, Cutscene = false, End;
    public static int Level;
    private bool once = false, CutsceneOnce = false, Monster = false, MonsterOnce = false, EndingOnce = false, Awake = false, fragOnce = false;

    private string currentDialogue = "";
    private int textIndex = 0; 

    private void Start()
    {
        InitializeGameObjects();
        InitializeState();
    }
    private void InitializeGameObjects()
    {
        StamBar = GameObject.Find("StamBar");
        CutsceneDiaBox = GameObject.Find("CutsceneDiaBox");
        CutSceneText = GameObject.Find("CutsceneDialogue");
        Options = GameObject.Find("Options");
        fadeIn = GameObject.Find("FadeIn");
        fadeOut = GameObject.Find("Fade");
        DiaBox = GameObject.Find("TextPanel");
        DiaText = GameObject.Find("Dialogue");
        StamBar.SetActive(false);
        fadeIn.SetActive(false);
        fadeOut.SetActive(false);
        CutSceneText.SetActive(false);
        CutsceneDiaBox.SetActive(false);
        Options.SetActive(false);
        DiaBox.SetActive(false);
    }
    private void InitializeState()
    {
        Level = 0;
        DontDestroyOnLoad(gameObject);
        ResetTimer = Timer;
    }
    public void Play()
    {
        GameObject.Find("Game").SetActive(false);
        GameObject.Find("Start").SetActive(false);
        StamBar.SetActive(true);
        Level += 1;
        SceneManager.LoadScene("SceneGame");
        ToggleFadeOut(true);
    }
    private void Update()
    {
        Dialogue();
        MonsterSpawn();
        LevelChange();
        CutsceneTrigger();
        Ending();
        AwakeState();
        Exit();
        if(!fragOnce && KeyItemsCollected == 1)
        {
            fragOnce = true;
            DisplayDialogueBox();
        }
    }
    private void Dialogue()
    {
        if (!once && Level >= 1 && Level < 4)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0 && DiaNum < 4)
            {
                DisplayDialogueBox();
                Timer = ResetTimer;
                once = true;
            }
        }
    }
    private void DisplayDialogueBox()
    {
        DiaBox.SetActive(true);
        Invoke(nameof(ChangingText), 1.2f);
    }
    private void MonsterSpawn()
    {
        if (MobSpawner.firstSpawned && !Monster)
        {
            DiaNum = 4;
            DiaBox.SetActive(true);
            Monster = true;
            Invoke(nameof(ChangingText), 1.5f);
        }
    }
    private void LevelChange()
    {
        if (ChangeLevel)
        {
            fadeIn.SetActive(true);
            fadeOut.SetActive(false);
            Invoke(nameof(ChangingLevel), 1.5f);
        }
    }
    private void CutsceneTrigger()
    {
        if (collectedFragments == 10 && !CutsceneOnce)
        {
            PrepareForCutscene();
            Invoke(nameof(CutScene), 1);
        }
    }
    private void PrepareForCutscene()
    {
        RegularPlayer = GameObject.Find("Player");
        CutsceneCam = GameObject.FindGameObjectWithTag("CutSceneCam");
    }
    private void Ending()
    {
        if (!EndingOnce && End)
        {
            CutSceneDiaNum = 8;
            Invoke(nameof(ChangingText), 1.4f);
            EndingOnce = true;
            TransitionToEnding();
        }
    }
    private void TransitionToEnding()
    {
        RegularPlayer = GameObject.Find("Player");
        RegularPlayer.SetActive(false);
        CutsceneCam.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CutsceneDiaBox.SetActive(true);
    }
    private void AwakeState()
    {
        if (Level == 4 && Awake)
        {
            RegularPlayer = GameObject.Find("Player");
            RegularPlayer.transform.position = new Vector3(-0.831f, 1.054f, 0.73f);
            Awake = false;
        }
    }
    private void Exit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    private void CutScene()
    {
        CutsceneOnce = true;
        Cutscene = true;
        ToggleFadeOut(true);
        RegularPlayer.SetActive(false);
        CutsceneCam.SetActive(true);
        CutsceneDiaBox.SetActive(true);
        Options.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DialogueOnScreen = true;
        DiaBox.SetActive(false);
        Invoke(nameof(ChangingText), 1.5f);
    }
    private void ChangingLevel()
    {
        if (ChangeLevel && !DialogueOnScreen)
        {
            switch (Level)
            {
                case 2:
                    SceneManager.LoadScene("Bedroom");
                    break;
                case 3:
                    SceneManager.LoadScene("MainGame");
                    break;
                case 4:
                    SceneManager.LoadScene("Bedroom");
                    break;
            }
            ChangeLevel = false;
            ResetDialogueNumbers();
            Invoke(nameof(StartFade), 2f);
        }
    }
    private void ResetDialogueNumbers()
    {
        DiaNum = 0;
        DialogueNum = 0;
    }
    private void StartFade()
    {
        DiaBox.SetActive(true);
        DiaText.SetActive(true);
        fadeIn.SetActive(false);
        ToggleFadeOut(true);
        if (Level == 4)
        {
            Application.Quit();
        }
    }
    public void NumAdd()
    {
        CutSceneDiaNum += 1;
    }
    public void ChangingText()
    {
        if (!Cutscene)
        {
            RegularDialogue();
        }
        else
        {
            CutsceneDialogue();
        }
        if (Monster && !MonsterOnce)
        {
            DisplayMonsterDialogue();
        }
    }
    private void RegularDialogue()
    {
        DiaText.SetActive(true);
        DialogueOnScreen = true;
        if (Level == 1 && DiaNum < 5)
        {
            DisplayDialogueLetterByLetter(Dialogues[DialogueNum]);
        }
        else if (Level == 2 && DiaNum <5)
        {
            DisplayDialogueLetterByLetter(BedroomDialogues[DialogueNum]);
        }
        else if (Level == 3 && DiaNum < 5)
        {
            DisplayDialogueLetterByLetter(GameDialogues[DialogueNum]);
        }
        if(KeyItemsCollected == 1)
        {
            DisplayDialogueLetterByLetter(GameDialogues[6]);
        }
        if (CutSceneDiaNum > 7)
        {
            CutSceneText.SetActive(true);
            CutSceneText.GetComponent<TextMeshProUGUI>().SetText(CutSceneDialogues[7]);
            End = true;
        }
        DiaNum += 1;
        Invoke(nameof(CloseText), 4f);
    }
    private void DisplayDialogueLetterByLetter(string dialogue)
    {
        currentDialogue = dialogue;
        textIndex = 0;
        DiaText.GetComponent<TextMeshProUGUI>().SetText(""); 
        InvokeRepeating(nameof(AddNextLetter), 0f, 0.01f); 
    }
    private void AddNextLetter()
    {
        DiaText.GetComponent<TextMeshProUGUI>().SetText(currentDialogue.Substring(0, textIndex++));
        if (textIndex > currentDialogue.Length)
        {
            CancelInvoke(nameof(AddNextLetter));
        }
    }
    private void DisplayMonsterDialogue()
    {
        DiaText.GetComponent<TextMeshProUGUI>().SetText(GameDialogues[4]);
        MonsterOnce = true;
        Invoke(nameof(CloseText), 4f);
    }
    private void CutsceneDialogue()
    {
        if (DiaBox.activeSelf)
        {
            DiaBox.SetActive(false);
        }
        fadeOut.SetActive(false);
        CutSceneText.SetActive(true);
        CutSceneText.GetComponent<TextMeshProUGUI>().SetText(CutSceneDialogues[CutSceneDiaNum]);
        UpdateCutsceneOptions();
        if (CutSceneDiaNum == 7)
        {
            EndCutscene();
        }
    }
    private void UpdateCutsceneOptions()
    {
        GameObject OptionText = GameObject.Find("OptionText");
        OptionText.GetComponent<TextMeshProUGUI>().SetText(CutSceneDiaOptions[CutSceneDiaNum]);
        if (CutSceneDiaNum == 1)
        {
            GameObject Option2 = GameObject.Find("Option2");
            Option2.SetActive(false);
        }
    }
    private void EndCutscene()
    {
        CutsceneDiaBox.SetActive(false);
        ToggleFadeOut(true);
        CutsceneCam.SetActive(false);
        CutSceneText.SetActive(false);
        CutsceneDiaBox.SetActive(false);
        Options.SetActive(false);
        RegularPlayer.SetActive(true);
        CutsceneOnce = true;
        Cutscene = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void CloseText()
    {
        DialogueOnScreen = false;
        if (Level == 1)
        {
            DialogueNum += 1;
        }
        else if (Level == 2 || Level == 3)
        {
            if (DialogueNum < 3)
            {
                DialogueNum += 1;
            }
        }
        DiaBox.SetActive(false);
        DiaText.SetActive(false);
        DiaText.GetComponent<TextMeshProUGUI>().SetText("");
        once = false;
        if (End)
        {
            Level += 1;
            CutsceneDiaBox.SetActive(false);
            ToggleFadeOut(false);
            ChangeLevel = true;
            Invoke(nameof(ChangingLevel), 0.2f);
        }
    }
    private void ToggleFadeOut(bool state)
    {
        fadeOut.SetActive(state);
    }
}
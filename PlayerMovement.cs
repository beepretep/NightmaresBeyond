using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Camera PlayerCamera;
    public float walkingSpeed, runningSpeed, lookingSpeed, LookingLimit, rotationX = 0, loudnessSens, threshold, stamina, loudness;
    Vector3 DirectionOfMovement = Vector3.zero;
    public static float MicLevel;
    public static bool hitOnce = false, Alive = true;
    public float collectedFragments, KeyItemsCollected, HP = 3;
    int amountOfTimesHit;
    bool canMove = true, isSprinting, rightDoor, wrongDoor, Moveable = true, gemstone1, gemstone2, finale = false, falseDoor = false;
    public string[] ItemDialogues;
    public GameObject[] L2Doors;
    public int sampleWindow = 64;
    private AudioClip micClip;
    public AudioClip WalkingSound;
    private string currentDialogue = "";
    private int textIndex = 0;
    private AudioSource audioSource; 
    private bool isPlayingMovementSound = false; 
    GameObject DialogueBox, DialogueText, FragmentText, keyItemsText, TrueDoor;
    Image Stambar;
    PostProcessVolume volume;
    Animator animator;
    Vignette vignette;
    CharacterController characterController;
    public void Start()
    {
        InitializeComponents();
        SetupInitialSettings();
        MicrophoneToAudioClip();
 
    }
    void InitializeComponents()
    {
        volume = GameObject.Find("Post").GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<Vignette>(out vignette);
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = WalkingSound;
        if (LevelManager.Level == 3)
        {
            animator = GetComponent<Animator>();
            animator.enabled = false;
            FragmentText = GameObject.Find("FragmentCollected");
            FragmentText.SetActive(false);
            keyItemsText = GameObject.Find("KeyItemsText");
            keyItemsText.SetActive(false);
        }
    }
    void SetupInitialSettings()
    {
        stamina = 10;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Update()
    {
        EnsureComponentsExist();
        if (Moveable && Alive)
        {
            SecMovement();
            SecMicInput();
        }
        if (HP <= 0)
        {
            SecDeath();
        }
        if (collectedFragments >= 16 && KeyItemsCollected == 2 && !finale)
        {
            SecEndingSetup();
        }
    }
    void EnsureComponentsExist()
    {
        if (DialogueBox == null || DialogueText == null)
        {
            DialogueBox = GameObject.Find("TextPanel");
            DialogueText = GameObject.Find("Dialogue");
        }
        if(Stambar == null)
        {
            Stambar = GameObject.Find("StamBar").GetComponent<Image>();
        }
    }
    void SecMovement()
    {
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        bool canCurrentlySprint = Input.GetKey(KeyCode.LeftShift) && stamina > 0;
        if (!isSprinting && stamina < 2)
        {
            canCurrentlySprint = false;
        }
        isSprinting = canMove && canCurrentlySprint;
        if (isSprinting)
        {
            stamina = Mathf.Clamp(stamina - 1 * Time.deltaTime, 0, 10);
        }
        else
        {
            stamina = Mathf.Clamp(stamina + 1 * Time.deltaTime, 0, 10);
        }
        if (Stambar != null)
        {
            Stambar.fillAmount = stamina / 10f;
        }
        float curSpeedX = canMove ? (isSprinting ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isSprinting ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        DirectionOfMovement = (forward * curSpeedX) + (right * curSpeedY);
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookingSpeed;
            rotationX = Mathf.Clamp(rotationX, -LookingLimit, LookingLimit);
            PlayerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookingSpeed, 0);
        }

        characterController.Move(DirectionOfMovement * Time.deltaTime);

        if (DirectionOfMovement.magnitude > 0)
        {
            if (!isPlayingMovementSound)
            {
                audioSource.clip = WalkingSound;
                audioSource.loop = true;
                audioSource.Play();
                isPlayingMovementSound = true;
            }
            audioSource.pitch = isSprinting ? 1.7f : 1.2f;
        }
        else if (isPlayingMovementSound)
        {
            audioSource.Stop();
            isPlayingMovementSound = false;
        }
    }
    void SecMicInput()
    {
        loudness = GetLoudFromMic() * loudnessSens;
        MicLevel = loudness < threshold ? loudness : 0;
    }
    void SecDeath()
    {
        animator.enabled = true;
        Moveable = false;
        animator.SetBool("Death", true);
        Invoke("Death", 2.5f);
    }
    void SecEndingSetup()
    {
        int randomNum = Random.Range(0, 5);
        TrueDoor = L2Doors[randomNum];
        TrueDoor.tag = "TrueDoor";
        GameObject[] fakes = GameObject.FindGameObjectsWithTag("FalseDoor");
        foreach (GameObject obj in fakes)
        {
            Destroy(obj);
        }
        finale = true;
    }
    void Death()
    {
        Application.Quit();
    }
    public float GetLoudness(int clipPosition, AudioClip clip)
    {
        int startPos = clipPosition - sampleWindow;
        if (startPos < 0)
        {
            return 0;
        }
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPos);
        float loudness = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            loudness += Mathf.Abs(waveData[i]);
        }
        return loudness / sampleWindow;
    }
    public float GetLoudFromMic()
    {
        return GetLoudness(Microphone.GetPosition(Microphone.devices[0]), micClip);
    }
    public void MicrophoneToAudioClip()
    {
        string microphoneName = Microphone.devices[0];
        micClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Frags"))
        {
            SecFragmentCollection(other);
        }
        else if (other.CompareTag("KeyItems"))
        {
            SecKeyItemCollection();
        }
        else if (other.CompareTag("KeyItems2"))
        {
            SecKeyItemCollection();
        }
        else if (other.CompareTag("Props"))
        {
            ShowDialogue();
        }
    }
    void SecFragmentCollection(Collider other)
    {
        FragmentText.SetActive(true);
        collectedFragments += 1;
        LevelManager.collectedFragments = collectedFragments;
        FragmentText.GetComponent<TextMeshProUGUI>().SetText("Fragments: " + collectedFragments);
        if(collectedFragments == 1)
        {
            DialogueBox.SetActive(true);
            Invoke(nameof(ShowDialogue), 1.2f);
        }
        Destroy(other.gameObject);
    }
    void SecKeyItemCollection()
    {
        if (!gemstone1)
        {
            gemstone1 = true;
            KeyItemsCollected += 1;
        }
        else if(!gemstone2)
        {
            gemstone2 = true;
            KeyItemsCollected += 1;
        }
        UpdateKeyItemsText();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.collider.tag)
        {
            case "WrongDoor":
                wrongDoor = true;
                DialogueBox.SetActive(true);
                Invoke(nameof(ShowDialogue), 1.2f);
                break;
            case "RightDoor":
                if (!LevelManager.DialogueOnScreen)
                {
                    SecRightDoorCollision();
                }
                break;
            case "Bed":
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Invoke(nameof(AdvanceToNextLevel), 1.4f);
                }
                break;
            case "Mobs":
                SecMobCollision();
                break;
            case "FalseDoor":
                falseDoor = true;
                DialogueBox.SetActive(true);
                Invoke(nameof(ShowDialogue), 1.2f);
                break;
            case "TrueDoor":
                SecTrueDoorCollision();
                break;
        }
    }
    void SecRightDoorCollision()
    {
        DialogueBox.SetActive(true);
        Invoke(nameof(ShowDialogue), 1.2f);
        rightDoor = true;
        Moveable = false;
    }
    void SecMobCollision()
    {
        if (!hitOnce)
        {
            HP -= 1;
            hitOnce = true;
            amountOfTimesHit += 1;
            vignette.intensity.value = amountOfTimesHit <= 1 ? 0.4f : 0.6f;
        }
    }
    void SecTrueDoorCollision()
    {
        keyItemsText.SetActive(false);
        FragmentText.SetActive(false);
        LevelManager.End = true;
    }
    void AdvanceToNextLevel()
    {
        if (!LevelManager.DialogueOnScreen && LevelManager.Level == 2)
        {
            LevelManager.Level += 1;
            LevelManager.ChangeLevel = true;
        }
    }
    void ShowDialogue()
    {
        if (!LevelManager.DialogueOnScreen)
        {
            LevelManager.DialogueOnScreen = true;
            DialogueText.SetActive(true);
            StartCoroutine(TypeDialogue(GetDialogueText()));
        }
    }
    IEnumerator TypeDialogue(string dialogue)
    {
        DialogueText.GetComponent<TextMeshProUGUI>().text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            DialogueText.GetComponent<TextMeshProUGUI>().text += letter;
            yield return new WaitForSeconds(0.01f); 
        }
        Invoke(nameof(CloseDialogue), 3f);
    }
    string GetDialogueText()
    {
        if (wrongDoor == true)
        {
           
            wrongDoor = false;
            return ItemDialogues[0];
        }
        if (rightDoor)
        {
            LevelManager.Level += 1;
            LevelManager.ChangeLevel = true;
            return ItemDialogues[1];
        }
        if (collectedFragments == 1)
        { 
            Debug.Log("Testing");
            return ItemDialogues[0];
        }
        if (falseDoor)
        {
            falseDoor = false;
            return ItemDialogues[1];
        }
        return string.Empty;
    }
    void UpdateKeyItemsText()
    {
        keyItemsText.SetActive(true);
        LevelManager.KeyItemsCollected = KeyItemsCollected;
        keyItemsText.GetComponent<TextMeshProUGUI>().SetText("Key Items: " + KeyItemsCollected);
    }
    void CloseDialogue()
    {
        DialogueText.SetActive(false);
        DialogueBox.SetActive(false);
        rightDoor = wrongDoor = false;
        LevelManager.DialogueOnScreen = false;
    }
}

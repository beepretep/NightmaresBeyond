using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public GameObject[] Mobs;
    GameObject mobSpawnPoint;
    public PlayerMovement playerMic;
    float MicLoudness,Timer = 20f, ResetTimer, randValue, randomValue;
    public static bool firstSpawned = false;
    public float SpawningChance, MicSpawningChance;
    GameObject clone;
    int mobChance;
    bool spawnedOnce,Triggered = false, rollOnce;
    private void Start()
    {
        mobSpawnPoint = this.gameObject;
        spawnedOnce = false;
        ChanceChange();
    }
    public void Update()
    {
        MicLoudness = playerMic.loudness;
        mobChance = Random.Range(0, 2);
        if (Triggered)
        {
            if(rollOnce== false)
            {
                randValue = Random.value;
                randomValue = Random.value;
                rollOnce = true;
            }
            if (MicLoudness > 1.7)
            {
 
                if (randValue < SpawningChance && GameObject.FindGameObjectsWithTag("Mobs").Length == 0 && !spawnedOnce && rollOnce) // 45% of the time
                {
                        clone = Instantiate(Mobs[mobChance], mobSpawnPoint.transform.position, Quaternion.identity);
                        MicLoudness = 0;
                        Invoke("Deletion", 15f);
                        Triggered = false;
                        spawnedOnce = true;
                        firstSpawned = true;
                }
                Debug.Log("Test");
            }
            if (randomValue < MicSpawningChance && GameObject.FindGameObjectsWithTag("Mobs").Length == 0 && !spawnedOnce&& rollOnce ) // 25% of the time
            {
                    clone = Instantiate(Mobs[mobChance], mobSpawnPoint.transform.position, Quaternion.identity);
                    MicLoudness = 0;
                    rollOnce = false;
                    Invoke("Deletion", 13f);
                    Triggered = false;
                    spawnedOnce = true;
                firstSpawned = true;
            }
        }
        if (Timer > 0 && spawnedOnce)
        {
            Timer -= Time.deltaTime;
        }
        if (Timer <= 0)
        {
            spawnedOnce = false;
            Timer = ResetTimer;
        }
    }
    void ChanceChange()
    {
        if(LevelManager.Difficulty == 1)
        {
            SpawningChance = .35f;
            MicSpawningChance = .15f;
        }
        if (LevelManager.Difficulty == 2)
        {
            SpawningChance = .45f;
            MicSpawningChance = .25f;
        }
        if (LevelManager.Difficulty == 3)
        {
            SpawningChance = .55f;
            MicSpawningChance = .35f;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (Triggered == false && other.tag == "Player")
        {
            Triggered = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (Triggered == true && other.tag == "Player")
        {
            Triggered = false;
        }
    }
    public void Deletion()
    {
        Destroy(clone);
    }

}

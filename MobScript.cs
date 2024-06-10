using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobScript : MonoBehaviour
{
    public NavMeshAgent Mob;
    public Transform player;
    public Animator ani;
    int timesCollided;
    private AudioSource audiosource;
    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        ani = GetComponent<Animator>();
        Mob = GetComponent <NavMeshAgent>();
        Mob.speed = 2.5f;
        Invoke("Running", 7f);
        audiosource.Play();
    }

    // Update is called once per frame
    public void Update()
    {
        if(LevelManager.Cutscene == false)
        {
        Mob.SetDestination(player.position);
        }
        if(LevelManager.End)
        {
            Destroy(this);
        }
        else if(timesCollided >= 3)
        {
            Destroy(this);
        }
       
    }
    public void Running()
    {
        audiosource.pitch = 1.8f;
        audiosource.Play();
        ani.speed = 1;
        Mob.speed = 7;
        ani.SetBool("Running", true);
        PlayerMovement.hitOnce = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            audiosource.Pause();
            ani.speed = 0;
            Mob.speed = 0;
            timesCollided = 1;
            Invoke("Running", 4f);
        }
    }
}

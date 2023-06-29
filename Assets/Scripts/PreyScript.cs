using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyScript : MonoBehaviour
{
    private GameObject player;
    private GameObject level;
    private Animator animator;

    public string name;
    public float preyFleeDistance = 2.5f;
    public float preySpeed = 15f;
    public int evoPoints = 5;
    private bool fleeing = false;
    public float startFleeTimer = 1.0f;
    private float fleeingTimer = 0;
    private float fleeingTime = 5.0f; // after x seconds of fleeing, delete
    private Vector3 move;

    public bool ableToFlee = true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Reptile");
        level = GameObject.Find("Level");
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < preyFleeDistance && ableToFlee)
        {
            fleeing = true;
        }
        if (fleeing && level.GetComponent<LevelScript>().pauseGame == false) {
            if (fleeingTimer > fleeingTime) {
                Destroy(gameObject);
            }
            // fly/walk away if player gets too close (either left or right depending on which side is closest to edge)
            if (gameObject.transform.position.x > 0)
            {
                animator.SetInteger("walk_direction", 1);
                if(fleeingTimer > startFleeTimer) 
                    move = new Vector3(1.0f, 1.0f, 0);
            }
            else {
                animator.SetInteger("walk_direction", -1);
                if (fleeingTimer > startFleeTimer)
                    move = new Vector3(-1.0f, 1.0f, 0);
            }

            fleeingTimer += Time.deltaTime;

            if (name == "Butterfly")
            {
                if(fleeingTimer < 1.0f)
                    fleeingTimer = 1.0f;
                move.y = 0.0f;
            }
            move *= preySpeed * Time.deltaTime;
            gameObject.GetComponent<Transform>().position = move + gameObject.transform.position;
        }
    }
}

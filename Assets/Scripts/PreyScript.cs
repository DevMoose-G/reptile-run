using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyScript : MonoBehaviour
{
    private GameObject player;
    private Animator animator;

    public string name;
    public float preyFleeDistance = 2.5f;
    public float preySpeed = 15f;
    public int evoPoints = 5;
    private bool fleeing = false;
    private float fleeingTimer = 0;
    private float fleeingTime = 5.0f; // after x seconds of fleeing, delete
    private Vector3 move;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Reptile");
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < preyFleeDistance)
        {
            fleeing = true;
        }
        if (fleeing) {
            if (fleeingTimer > fleeingTime) {
                Destroy(gameObject);
            }
            // fly/walk away if player gets too close (either left or right depending on which side is closest to edge)
            if (gameObject.transform.position.x > 0)
            {
                animator.SetInteger("walk_direction", 1);
                if(fleeingTimer > 1.0f) 
                    move = new Vector3(1.0f, 1.0f, 0);
            }
            else {
                animator.SetInteger("walk_direction", -1);
                if (fleeingTimer > 1.0f)
                    move = new Vector3(-1.0f, 1.0f, 0);
            }

            fleeingTimer += Time.deltaTime;

            if (name == "Butterfly")
                move.y = 0.0f;
            move *= preySpeed * Time.deltaTime;
            gameObject.GetComponent<Transform>().position = move + gameObject.transform.position;
        }
    }
}

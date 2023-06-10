using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyScript : MonoBehaviour
{
    private GameObject player;
    public float preyFleeDistance = 2.5f;
    public float preySpeed = 15f;
    public int evoPoints = 5;
    private bool fleeing = false;
    private float fleeingTimer = 0;
    private float fleeingTime = 5.0f; // after x seconds of fleeing, delete
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Reptile");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < preyFleeDistance)
        {
            fleeing = true;
        }
        if (fleeing) {
            fleeingTimer += Time.deltaTime;
            if (fleeingTimer > fleeingTime) {
                Destroy(gameObject);
            }
            // fly/walk away if player gets too close (either left or right depending on which side is closest to edge)
            if (gameObject.transform.position.x > 0)
            {
                Vector3 move = new Vector3(1.0f, 1.0f, 0) * preySpeed * Time.deltaTime;
                gameObject.GetComponent<Transform>().position = move + gameObject.transform.position;
            }
            else {
                Vector3 move = new Vector3(-1.0f, 1.0f, 0) * preySpeed * Time.deltaTime;
                gameObject.GetComponent<Transform>().position = move + gameObject.transform.position;
            }
            
        }
    }
}

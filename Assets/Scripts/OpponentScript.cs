using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentScript : MonoBehaviour
{
    public float damage = 0.5f;
    public float attackSpeed = 1.0f;
    public float health;
    public float MAX_HEALTH = 2.0f;
    public int evoPoints = 40;
    public string Name = "";

    // Start is called before the first frame update
    void Start()
    {
        health = MAX_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

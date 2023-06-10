using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //Detect when there is a collision starting
    void OnCollisionEnter(Collision collision)
    {
        PreyScript prey = collision.gameObject.GetComponent<PreyScript>();
        if (prey != null && gameObject.transform.root.GetComponent<ReptileScript>().tongueOut)
        {
            Destroy(collision.gameObject);
            GameState.current.addEvoPoints(prey.evoPoints);
            gameObject.transform.root.GetComponent<ReptileScript>().isRetracting = true;
            gameObject.transform.root.GetComponent<ReptileScript>().tongueTimer = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

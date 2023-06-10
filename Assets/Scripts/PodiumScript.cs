using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumScript : MonoBehaviour
{

    public float rotationSpeed = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch theTouch = Input.GetTouch(0);
            float rotation = -theTouch.deltaPosition.x * rotationSpeed;
            gameObject.transform.Rotate(0, rotation, 0);
        }
    }
}

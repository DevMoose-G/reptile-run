using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCameraScript : MonoBehaviour
{
    float cameraOffset = -4.75f;
    float battleCameraOffset = 2.75f;
    bool battleMode = false;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Reptile");
    }

    public void BattleModeSetup(GameObject battleStage) {
        battleMode = true;
        gameObject.transform.position = new Vector3(17.5f, 2.5f, battleStage.transform.position.z - 1.5f);
        gameObject.transform.Rotate(0, -90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currPos = gameObject.GetComponent<Transform>().position;
        if (battleMode)
        {
            gameObject.GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y, player.GetComponent<Transform>().position.z + battleCameraOffset);
        }
        else
        {
            gameObject.GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y, player.GetComponent<Transform>().position.z + cameraOffset);
        }
    }
}

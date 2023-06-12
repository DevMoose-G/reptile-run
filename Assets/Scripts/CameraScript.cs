using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float cameraOffset = -4.75f; // z
    float cameraYOffset = 1.9f;
    float battleCameraOffset = 2.75f;
    bool battleMode = false;
    GameObject player;
    GameObject UI;

    public bool stageIntro = false;
    public bool tutorialIntro = false;
    public float stageTutorialTimer = 0; // pause timer
    public float stageIntroTimer = 0.0f;
    public float STAGEINTRO_TIME = 1.25f;

    public Vector3 starterPos;
    public Quaternion starterQuat;
    public Quaternion starterIntroQuat;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Reptile");
        UI = GameObject.Find("UIDocument");
        starterPos = gameObject.transform.position;
        starterPos.x = player.transform.position.x;
        starterQuat = gameObject.transform.rotation;
    }

    public void BattleModeSetup(GameObject battleStage) {
        battleMode = true;
        gameObject.transform.position = new Vector3(17.5f, 2.5f, battleStage.transform.position.z - 1.5f);
        gameObject.transform.Rotate(0, -90, 0);
    }

    public void TutorialStageIntro()
    {
        
        if (stageTutorialTimer == 0)
        {
            UI.GetComponent<UI>().UISetActive(false);
            gameObject.transform.position = new Vector3(player.GetComponent<Transform>().position.x, 5.5f, player.GetComponent<Transform>().position.z + cameraOffset);
        }
        if (stageTutorialTimer < 2.0f)
        {
            stageTutorialTimer += Time.deltaTime;
            return;
        }
        float percent = (stageIntroTimer / STAGEINTRO_TIME);

        gameObject.transform.position = new Vector3(player.GetComponent<Transform>().position.x, (5.5f * (1-percent)) + cameraYOffset*percent, player.GetComponent<Transform>().position.z + cameraOffset);

        // slows down as you get to the end
        if ((STAGEINTRO_TIME - stageIntroTimer) < 0.1f)
            stageIntroTimer += Time.deltaTime * 0.1f;
        stageIntroTimer += (Time.deltaTime * (STAGEINTRO_TIME - stageIntroTimer));

        if (stageIntroTimer >= STAGEINTRO_TIME)
        {
            tutorialIntro = false;
            UI.GetComponent<UI>().UISetActive(true);
        }
    }


    public void StageIntro()
    {
        if(stageIntroTimer == 0)
        {
            UI.GetComponent<UI>().UISetActive(false);
            gameObject.transform.position = new Vector3(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y - 0.125f, player.GetComponent<Transform>().position.z + 2.0f);
            gameObject.transform.LookAt(player.transform);
            starterIntroQuat = gameObject.transform.rotation;
        }
        float percent = (stageIntroTimer / STAGEINTRO_TIME);
        stageIntroTimer += Time.deltaTime;
        float currentAngle = 3.1415f * (stageIntroTimer / STAGEINTRO_TIME);
        Vector2 angleOffset = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * (3.1415f / 2) * STAGEINTRO_TIME;
        Vector3 offset = new Vector3(angleOffset.x, 0, angleOffset.y);
        offset.y = ( (player.GetComponent<Transform>().position.y - 0.125f) * (1- percent) ) + ( (cameraYOffset) * (percent) );
        offset.z += (cameraOffset - (player.GetComponent<Transform>().position.z - 2.0f) ) * percent;

        gameObject.transform.position = player.transform.position + offset;
        gameObject.transform.LookAt(player.transform.position + new Vector3(0, 1.5f * percent, 0));

        // gameObject.transform.rotation = Quaternion.SlerpUnclamped(starterIntroQuat, starterQuat, -percent);

        if (stageIntroTimer >= STAGEINTRO_TIME) {
            // gameObject.transform.LookAt(gameObject.transform.position + new Vector3(0, 0, 1.0f));
            // gameObject.transform.position = starterPos;
            gameObject.transform.LookAt(player.transform.position + new Vector3(0, 1.5f, 0));
            // gameObject.transform.rotation = starterQuat;
            stageIntro = false;
            stageIntroTimer = 0.0f;
            UI.GetComponent<UI>().UISetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(stageIntro)
        {
            StageIntro();
            return;
        } else if (tutorialIntro)
        {
            TutorialStageIntro();
            return;
        }
        if (stageIntroTimer <= 1.0f)
        {
            stageIntroTimer += Time.deltaTime;
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, starterQuat, stageIntroTimer);
        }
        else
        {
            gameObject.transform.position = starterPos;
        }
        
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

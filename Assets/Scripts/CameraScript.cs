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
    public float STAGEINTRO_TIME = 1.0f;

    public float stageIntroPauseTimer = 0.0f;
    public float STAGEINTROPAUSE_TIME = 1.25f;

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
            UI.GetComponent<UI>().SetupBattleUI();
        }
    }


    public void StageIntro()
    {
        float intro_currentAngle = 3.1415f / 5.0f;
        if (stageIntroPauseTimer < STAGEINTROPAUSE_TIME)
        {
            if (stageIntroPauseTimer == 0)
            {
                UI.GetComponent<UI>().UISetActive(false);

                Vector2 intro_angleOffset = new Vector2(Mathf.Sin(intro_currentAngle), Mathf.Cos(intro_currentAngle)) * (3.1415f / 2) * STAGEINTRO_TIME;
                Vector3 intro_offset = new Vector3(intro_angleOffset.x, 0, intro_angleOffset.y);
                intro_offset.y = ((player.GetComponent<Transform>().position.y));
                intro_offset.z += (cameraOffset - (player.GetComponent<Transform>().position.z - 2.0f)) * 0;

                gameObject.transform.position = player.transform.position + intro_offset;
                gameObject.transform.LookAt(player.transform);
                starterIntroQuat = gameObject.transform.rotation;
            }

            // if screen tapped, automatically end pause timer
            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                stageIntroPauseTimer = STAGEINTROPAUSE_TIME;
            }

            stageIntroTimer = STAGEINTRO_TIME * (intro_currentAngle / 3.1415f);

            stageIntroPauseTimer += Time.deltaTime;
            return;
        }

        
        float percent = (stageIntroTimer - STAGEINTRO_TIME * (intro_currentAngle / 3.1415f)) / (STAGEINTRO_TIME - STAGEINTRO_TIME * (intro_currentAngle / 3.1415f));
        if( (percent+0.01f) * 3 < 1)
            stageIntroTimer += (Time.deltaTime * (percent + 0.01f) * 3); //smoothing
        else
            stageIntroTimer += Time.deltaTime;
        float currentAngle = 3.1415f * (stageIntroTimer / STAGEINTRO_TIME);
        Vector2 angleOffset = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * (3.1415f / 2) * STAGEINTRO_TIME;
        Vector3 offset = new Vector3(angleOffset.x, 0, angleOffset.y);
        offset.y = ( (player.GetComponent<Transform>().position.y) * (1- percent) ) + ( (cameraYOffset) * (percent) );
        offset.z += (cameraOffset - (player.GetComponent<Transform>().position.z - 2.0f) ) * percent;

        gameObject.transform.position = player.transform.position + offset;
        gameObject.transform.LookAt(player.transform.position + new Vector3(0, 1.77f * percent, 0));

        // gameObject.transform.rotation = Quaternion.SlerpUnclamped(starterIntroQuat, starterQuat, -percent);

        if (stageIntroTimer >= STAGEINTRO_TIME) {
            // gameObject.transform.LookAt(gameObject.transform.position + new Vector3(0, 0, 1.0f));
            gameObject.transform.position = starterPos;
            gameObject.transform.LookAt(player.transform.position + new Vector3(0, 1.77f, 0));
            // gameObject.transform.rotation = starterQuat;
            stageIntro = false;
            stageIntroTimer = 0.0f;
            UI.GetComponent<UI>().UISetActive(true);
            UI.GetComponent<UI>().SetupBattleUI();
        }

    }

    // Update is called once per frame
    void Update()
    {
        // wait for loading to finish
        if (GameObject.Find("LoadingScreen") != null)
            return;

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

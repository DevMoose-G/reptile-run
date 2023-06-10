using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class TutorialLevelScript : LevelScript
{

    public bool startMoving = false;
    public float startRocksTimer = 0.25f;
    public GameObject rockPathPrefab;
    private GameObject rockPath = null;
    private float rockPathZOffset = 25.0f;

    public bool hitRock = false;
    private GameObject hitRockLabel;

    public bool died = false;
    private GameObject diedLabel;

    public bool upgradeTipShow = false;
    private GameObject upgradeTip;

    public bool evoPointsTipShow = false;
    private GameObject evoPointsTip;

    public bool swipeUpTipShow = false;
    private GameObject swipeUpTip;

    new void Start()
    {
        // if(PlayerPrefs.GetInt("diedFromHealth", 0) == 0)
            PlayerPrefs.DeleteAll();

        base.Start();
        hitRockLabel = GameObject.Find("HitRock");
        hitRockLabel.SetActive(false);

        diedLabel = GameObject.Find("Died");
        diedLabel.SetActive(false);

        upgradeTip = GameObject.Find("UpgradeTip");
        upgradeTip.SetActive(false);

        evoPointsTip = GameObject.Find("EvoPointsTip");
        evoPointsTip.SetActive(false);

        swipeUpTip = GameObject.Find("SwipeUpTip");
        swipeUpTip.SetActive(false);

        int diedFromHealth = PlayerPrefs.GetInt("diedFromHealth", 0);
        if(diedFromHealth == 1) // just died from health loss
        {
            upgradeTip.SetActive(true);
            PlayerPrefs.SetInt("diedFromHealth", 2); // already saw the tip for it
        }

        
    }

    void TurnOffAllLabels() {
        diedLabel.SetActive(false);
        hitRockLabel.SetActive(false);
        upgradeTip.SetActive(false);
        evoPointsTip.SetActive(false);
        swipeUpTip.SetActive(false);
    }

    // Update is called once per frame

    void Update()
    {
        if (Mathf.Abs(player.transform.position.x) > 1.5)
        { // you have falled off map
            PlayerPrefs.SetInt("diedFromMap", 1);
            PlayerPrefs.Save();
            Debug.Log("You have fallen off the map");
            EndRun();
        }
        if (player.GetComponent<ReptileScript>().health <= 0)
        {
            PlayerPrefs.SetInt("diedFromHealth", 1);
            PlayerPrefs.Save();
            EndRun();
        }

        if(Mathf.Abs(player.transform.position.x) > 0.25f)
        {
            startMoving = true;
        }
        if(startMoving && startRocksTimer > 0)
            startRocksTimer -= Time.deltaTime;

        if (startRocksTimer <= 0 && rockPath == null)
        {
            Vector3 rockPathPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, rockPathZOffset);
            rockPath = Instantiate(rockPathPrefab, gameObject.transform, true);
            rockPath.transform.position = rockPathPos;
        }

        if (hitRock)
        {
            isMoving = false;
            hitRockLabel.SetActive(true);
            hitRock = false;
        }

        if (died)
        {
            isMoving = false;
            diedLabel.SetActive(true);
            died = false;
        }

        if (upgradeTipShow)
        {
            isMoving = false;
            upgradeTip.SetActive(true);
            upgradeTipShow = false;
        }

        if (evoPointsTipShow)
        {
            GameState.current.addEvoPoints(250);
            isMoving = false;
            evoPointsTip.SetActive(true);
            evoPointsTipShow = false;
        }

        if (rockPath.transform.position.z < -43)
        {
            isMoving = false;
            swipeUpTip.SetActive(true);
            swipeUpTipShow = false;
        }

        if (isMoving && timeElapsed < timeTillStage)
        {
            timeElapsed += Time.deltaTime;
            Vector3 currPos = gameObject.GetComponent<Transform>().position;
            gameObject.GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y, currPos.z - Time.deltaTime * levelSpeed);
        }
        else if (!stationary && UpgradeScreen.activeSelf == false)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            { // currently touching

                if (evoPointsTip.activeSelf) {
                    // check if it is a vertical swipe
                    print(player.GetComponent<ReptileScript>().tongueOut);
                    if (player.GetComponent<ReptileScript>().tongueOut)
                    {
                        isMoving = true;
                        TurnOffAllLabels();
                    }
                }
                else
                {
                    isMoving = true;
                    TurnOffAllLabels();
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class TutorialLevelScript : LevelScript
{
    public enum TipStatus { NotSeen = 0, JustSeen = 1, BeenSeen = 2 };

    public bool startMoving = false;
    public float startRocksTimer = 0.25f;
    public GameObject rockPathPrefab;
    private GameObject rockPath = null;
    private float rockPathZOffset = 25.0f;

    public float timeSinceTip = 0.0f;
    private float timeSincePrey = 0.0f;

    private GameObject reptileRunTitle;

    private float moveAroundTimer = 4.0f;
    private GameObject moveAroundTip;

    public TipStatus hitRock = TipStatus.NotSeen;
    private GameObject hitRockLabel;

    public TipStatus died = TipStatus.NotSeen;
    private GameObject diedLabel;

    public TipStatus diedFromMapShown = TipStatus.NotSeen;
    private GameObject diedFromMapTip;

    public TipStatus upgradeTipShown = TipStatus.NotSeen;
    private GameObject upgradeTip;

    public TipStatus evoPointsTipShown = TipStatus.NotSeen;
    private GameObject evoPointsTip;

    public TipStatus swipeUpTipShown = TipStatus.NotSeen;
    private GameObject swipeUpTip;

    public TipStatus afterPreyShown = TipStatus.NotSeen;
    private GameObject afterPreyTip;

    public TipStatus preyRunsShown = TipStatus.NotSeen;
    private GameObject preyRunsTip;

    public TipStatus battleStageShown = TipStatus.NotSeen;
    private GameObject battleStageTip;

    public TipStatus autoAttackShown = TipStatus.NotSeen;
    private GameObject autoAttackTip;

    public TipStatus crownShown = TipStatus.NotSeen;
    private GameObject crownTip;

    public TipStatus endOfTutorialShown = TipStatus.NotSeen;
    private GameObject endOfTutorialTip;

    private GameObject exitTutorialButton;

    void Awake() // overrides the LevelScript Awake (so it never checks if you need tutorial as you already in tutorial)
    {
        playerCam = GameObject.Find("Main Camera");
        isTutorial = true;
        if(PlayerPrefs.GetInt("NumberOfRuns", 0) == 0)
            playerCam.GetComponent<CameraScript>().tutorialIntro = true;
    }

    new void Start()
    {
        Load();

        base.Start();

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));

        moveAroundTip = GameObject.Find("MoveAroundTip");
        hitRockLabel = GameObject.Find("HitRock");
        diedLabel = GameObject.Find("Died");
        diedFromMapTip = GameObject.Find("DiedFromMapTip");
        upgradeTip = GameObject.Find("UpgradeTip");
        evoPointsTip = GameObject.Find("EvoPointsTip");
        swipeUpTip = GameObject.Find("SwipeUpTip");
        afterPreyTip = GameObject.Find("AfterPreyTip");
        preyRunsTip = GameObject.Find("PreyRunsTip");
        battleStageTip = GameObject.Find("BattleStageIntroTip");
        autoAttackTip = GameObject.Find("AutoAttackTip");
        crownTip = GameObject.Find("CrownTip");
        endOfTutorialTip = GameObject.Find("EndOfTutorialTip");

        exitTutorialButton = GameObject.Find("ExitTutorial");

        TurnOffAllLabels();
    }

    void TurnOffAllLabels() {
        moveAroundTip.SetActive(false);
        diedLabel.SetActive(false);
        diedFromMapTip.SetActive(false);
        hitRockLabel.SetActive(false);
        upgradeTip.SetActive(false);
        evoPointsTip.SetActive(false);
        swipeUpTip.SetActive(false);
        afterPreyTip.SetActive(false);
        preyRunsTip.SetActive(false);
        battleStageTip.SetActive(false);
        autoAttackTip.SetActive(false);
        crownTip.SetActive(false);
        endOfTutorialTip.SetActive(false);
    }

    void Load()
    {
        hitRock = (TipStatus)PlayerPrefs.GetInt("hitRock", (int)TipStatus.NotSeen);
        died = (TipStatus)PlayerPrefs.GetInt("died", (int)TipStatus.NotSeen);
        diedFromMapShown = (TipStatus)PlayerPrefs.GetInt("diedFromMapShown", (int)TipStatus.NotSeen);
        swipeUpTipShown = (TipStatus)PlayerPrefs.GetInt("swipeUpTipShown", (int)TipStatus.NotSeen);
        afterPreyShown = (TipStatus)PlayerPrefs.GetInt("afterPreyShown", (int)TipStatus.NotSeen);
        upgradeTipShown = (TipStatus)PlayerPrefs.GetInt("upgradeTipShown", (int)TipStatus.NotSeen);
        evoPointsTipShown = (TipStatus)PlayerPrefs.GetInt("evoPointsTipShown", (int)TipStatus.NotSeen);
        preyRunsShown = (TipStatus)PlayerPrefs.GetInt("preyRunsShown", (int)TipStatus.NotSeen);
        battleStageShown = (TipStatus)PlayerPrefs.GetInt("battleStageShown", (int)TipStatus.NotSeen);
        autoAttackShown = (TipStatus)PlayerPrefs.GetInt("autoAttackShown", (int)TipStatus.NotSeen);
        crownShown = (TipStatus)PlayerPrefs.GetInt("crownShown", (int)TipStatus.NotSeen);
        endOfTutorialShown = (TipStatus)PlayerPrefs.GetInt("endOfTutorialShown", (int)TipStatus.NotSeen);
    }

    void JustSeenToBeenSeen()
    {
        if(hitRock == TipStatus.JustSeen)
            hitRock = TipStatus.BeenSeen;
        if (died == TipStatus.JustSeen)
            died = TipStatus.BeenSeen;
        if (diedFromMapShown == TipStatus.JustSeen)
            diedFromMapShown = TipStatus.BeenSeen;
        if (upgradeTipShown == TipStatus.JustSeen)
            upgradeTipShown = TipStatus.BeenSeen;
        if (evoPointsTipShown == TipStatus.JustSeen)
            evoPointsTipShown = TipStatus.BeenSeen;
        if (swipeUpTipShown == TipStatus.JustSeen)
            swipeUpTipShown = TipStatus.BeenSeen;
        if (afterPreyShown == TipStatus.JustSeen)
            afterPreyShown = TipStatus.BeenSeen;
        if (preyRunsShown == TipStatus.JustSeen)
            preyRunsShown = TipStatus.BeenSeen;
        if (battleStageShown == TipStatus.JustSeen)
            battleStageShown = TipStatus.BeenSeen;
        if (autoAttackShown == TipStatus.JustSeen)
            autoAttackShown = TipStatus.BeenSeen;
        if (crownShown == TipStatus.JustSeen)
            crownShown = TipStatus.BeenSeen;
        if (endOfTutorialShown == TipStatus.JustSeen)
            endOfTutorialShown = TipStatus.BeenSeen;
    }

    void PauseGame()
    {
        pauseGame = true;
        isMoving = false;
        player.GetComponent<ReptileScript>().canMove = false;
    }

    void UnpauseGame()
    {
        pauseGame = false;
        if(battleStage == null)
            isMoving = true;
        player.GetComponent<ReptileScript>().canMove = true;
    }

    public void FinishTutorial()
    {
        SaveGameScript.Clear();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("FinishTutorial", 1);
        SceneManager.LoadSceneAsync(0);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceTip += Time.deltaTime;
        if (Mathf.Abs(player.transform.position.x) > 1.5)
        { // you have falled off map
            diedFromMapShown = TipStatus.JustSeen;
            EndRun();
            PlayerPrefs.Save();
        }
        if (player.GetComponent<ReptileScript>().health <= 0.0f)
        {
            EndRun();
        }

        if(Mathf.Abs(player.transform.position.x) > 0.25f)
        {
            moveAroundTip.SetActive(false);
            startMoving = true;
        }

        if(startMoving && startRocksTimer > 0)
            startRocksTimer -= Time.deltaTime;

        if (startRocksTimer <= 0 && rockPath == null && !(preyRunsShown == TipStatus.BeenSeen && upgradeTipShown == TipStatus.BeenSeen)
            && UpgradeScreen.activeSelf == false)
        {
            Vector3 rockPathPos = new Vector3(gameObject.transform.position.x + 0.9f, gameObject.transform.position.y, rockPathZOffset);
            rockPath = Instantiate(rockPathPrefab, gameObject.transform, true);
            rockPath.transform.position = rockPathPos;
        }

        // start of tips
        if (startMoving == false && isMoving == true && rockPath == null && afterPreyShown == TipStatus.NotSeen)
        {
            moveAroundTimer -= Time.deltaTime;
            if (moveAroundTimer <= 0)
            {
                moveAroundTip.SetActive(true);
                Color prevColor = moveAroundTip.GetComponent<TMP_Text>().color;
                moveAroundTip.GetComponent<TMP_Text>().color = new Color(prevColor.r, prevColor.g, prevColor.b, prevColor.a + (Time.deltaTime * 0.75f));

                timeSinceTip = 0.0f;
            }
        }

        if (hitRock == TipStatus.JustSeen)
        {
            hitRock = TipStatus.BeenSeen;
            PauseGame();
            hitRockLabel.SetActive(true);
            timeSinceTip = 0.0f;
        }

        // died from hitting rock
        if (died == TipStatus.JustSeen && hitRock == TipStatus.BeenSeen)
        {
            died = TipStatus.BeenSeen;
            PauseGame();
            timeSinceTip = 0.0f;
            diedLabel.SetActive(true);
        }

        // died from falling off map
        if(diedFromMapShown == TipStatus.JustSeen)
        {
            diedFromMapShown = TipStatus.BeenSeen;
            PauseGame();
            diedFromMapTip.SetActive(true);
            timeSinceTip = 0.0f;
        }

        // swipe up
        if (rockPath != null && rockPath.transform.position.z < -43 && swipeUpTipShown == TipStatus.NotSeen)
        {
            PauseGame();
            swipeUpTip.SetActive(true);
            swipeUpTipShown = TipStatus.JustSeen;
            timeSinceTip = 0;
        }

        // upgrade tip on screen
        if (UpgradeScreen.activeSelf && swipeUpTipShown == TipStatus.BeenSeen && upgradeTipShown == TipStatus.NotSeen && GameObject.Find("LoadingScreen") != null)
        {
            PauseGame();
            upgradeTip.SetActive(true);
            upgradeTipShown = TipStatus.JustSeen;
            timeSinceTip = 0.0f;
            UI.GetComponent<UI>().playGame.UnregisterCallback<ClickEvent>(UI.GetComponent<UI>().StartGame);
        }

        if (upgradeTipShown == TipStatus.BeenSeen && evoPointsTipShown == TipStatus.NotSeen)
        {
            evoPointsTipShown = TipStatus.JustSeen;
            GameState.current.addEvoPoints(250);
            UI.GetComponent<UI>().UpdateUpgrades();
            PauseGame();
            evoPointsTip.SetActive(true);
            timeSinceTip = 0.0f;
        }

        // re-enables play game button after upgradeTip and evoTip
        if(upgradeTipShown == TipStatus.BeenSeen && evoPointsTipShown == TipStatus.BeenSeen && battleStageShown == TipStatus.NotSeen)
        {
            UI.GetComponent<UI>().playGame.RegisterCallback<ClickEvent>(UI.GetComponent<UI>().StartGame);
        }

        // points to evo points
        if(swipeUpTipShown == TipStatus.BeenSeen && afterPreyShown == TipStatus.NotSeen)
        {
            timeSincePrey += Time.deltaTime;
            if (timeSincePrey > 0.90f)
            {
                afterPreyTip.SetActive(true);
                afterPreyShown = TipStatus.JustSeen;
                PauseGame();
                timeSinceTip = 0.0f;
            }
        }

        // prey runs away sometimes
        if(afterPreyShown == TipStatus.BeenSeen && rockPath != null && rockPath.transform.position.z < -53 && preyRunsShown == TipStatus.NotSeen)
        {
            preyRunsTip.SetActive(true);
            PauseGame();
            timeSinceTip = 0.0f;
            preyRunsShown = TipStatus.JustSeen;
        }

        if (lastObjectPlaced == null)
        {
            lastObjectPlaced = player;
        }
        // increasingly hard obstacles until died == TipStatus.BeenSeen && upgradeScreen == TipStatus.BeenSeen
        if (preyRunsShown == TipStatus.BeenSeen && (upgradeTipShown != TipStatus.BeenSeen))
        {
            if(minZ_SpawnDistance >= ending_SpawnDistance)
                minZ_SpawnDistance -= 0.5f * Time.deltaTime; 
            // creating the level as you go
            if (lastObjectPlaced != null
                    && (Vector3.Distance(player.transform.position, lastObjectPlaced.transform.position) < 50 || lastObjectPlaced.transform.position.y < -2))/*if off map*/
            {
                // figure out position
                float randX = Random.Range(-1.35f, 1.35f);
                float randZ = Random.Range(minZ_SpawnDistance, minZ_SpawnDistance * 1.5f);
                Vector3 newObject_position = lastObjectPlaced.transform.position + new Vector3(0.0f, 0.0f, randZ);
                if(lastObjectPlaced == player)
                {
                    newObject_position.z += 25.0f;
                }
                newObject_position.x = randX;

                float typeToSpawn = Random.Range(0.0f, 1.0f);
                // percents: stone/0.9, ladybug/0.075, spider/0.025
                if (typeToSpawn > 0.1)
                {
                    lastObjectPlaced = Instantiate(rockPrefab, gameObject.transform, true);
                    float randRot = Random.Range(0.0f, 360.0f);
                    lastObjectPlaced.transform.Rotate(0, randRot, 0);
                }
                else if (typeToSpawn > 0.025)
                {
                    lastObjectPlaced = Instantiate(preyPrefabs[0], gameObject.transform, true);
                }
                else
                {
                    lastObjectPlaced = Instantiate(preyPrefabs[1], gameObject.transform, true);
                }
                lastObjectPlaced.transform.position = newObject_position;
            }
        }

        // battle stage intro tip
        // enter battle stage (if battle stage is not created, you have seen preytip & diedTip & upgradeTip & you have traveled 5 units of level
        // & you are 3 units in front of last gameobject
        if (battleStage == null && preyRunsShown == TipStatus.BeenSeen && upgradeTipShown == TipStatus.BeenSeen 
            && gameObject.transform.position.z < -5 && player.gameObject.transform.position.z > lastObjectPlaced.transform.position.z + 3)
        {
            UI.GetComponent<UI>().SwitchToBattleUI();

            battleStage = Instantiate(battleStagePrefab, gameObject.transform, true);
            battleStage.transform.position = new Vector3(0, 0, player.transform.position.z + 5.0f);

            // center player
            player.GetComponent<Transform>().position = new Vector3(0, player.transform.position.y, player.transform.position.z);

            player.GetComponent<ReptileScript>().battleStage = battleStage;
            playerCam.GetComponent<CameraScript>().BattleModeSetup(battleStage);

            battleStageShown = TipStatus.JustSeen;
            battleStageTip.SetActive(true);
            PauseGame();
            timeSinceTip = 0.0f;
        }

        // if somehow you created battle stage before, but you died, then create it again w/o tip
        if(battleStage == null && battleStageShown == TipStatus.BeenSeen 
            && gameObject.transform.position.z < -5 && player.gameObject.transform.position.z > lastObjectPlaced.transform.position.z + 3)
        {
            UI.GetComponent<UI>().SwitchToBattleUI();

            battleStage = Instantiate(battleStagePrefab, gameObject.transform, true);
            battleStage.transform.position = new Vector3(0, 0, player.transform.position.z + 5.0f);

            // center player
            player.GetComponent<Transform>().position = new Vector3(0, player.transform.position.y, player.transform.position.z);

            player.GetComponent<ReptileScript>().battleStage = battleStage;
            playerCam.GetComponent<CameraScript>().BattleModeSetup(battleStage);
        }

        // timing attacks tip
        if(battleStage != null && battleStageShown == TipStatus.BeenSeen && timeSinceTip > 1.5f && autoAttackShown == TipStatus.NotSeen)
        {
            autoAttackTip.SetActive(true);
            autoAttackShown = TipStatus.JustSeen;
            PauseGame();
            timeSinceTip = 0.0f;
        }

        // there's the crown tip
        float playerDistBattleStage = 0.0f;
        if (battleStage != null && autoAttackShown == TipStatus.BeenSeen && battleStage.transform.position.z < 5.1 && player.transform.position.z > 3.4f && crownShown == TipStatus.NotSeen)
        {
            PauseGame();
            timeSinceTip = 0.0f;
            crownShown = TipStatus.JustSeen;
            crownTip.SetActive(true);
        }

        // end of tutorial tip
        if(UpgradeScreen.activeSelf && crownShown == TipStatus.BeenSeen && endOfTutorialShown == TipStatus.NotSeen) {
            endOfTutorialShown = TipStatus.JustSeen;
            endOfTutorialTip.SetActive(true);
            PauseGame();
            timeSinceTip = 0.0f;
        }


        if (isMoving)
        {
            timeElapsed += Time.deltaTime;
            Vector3 currPos = gameObject.GetComponent<Transform>().position;
            gameObject.GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y, currPos.z - Time.deltaTime * levelSpeed);
        }
        else if (!stationary && (!playerCam.GetComponent<CameraScript>().stageIntro && !playerCam.GetComponent<CameraScript>().tutorialIntro))
        {
            if (Input.touchCount > 0)
            { // currently touching
                Touch theTouch = Input.GetTouch(0);
                if (swipeUpTip.activeSelf) {
                    // check if it is a vertical swipe
                    if (player.GetComponent<ReptileScript>().tongueOut)
                    {
                        if(UpgradeScreen.activeSelf == false)
                            UnpauseGame();
                        JustSeenToBeenSeen();
                        TurnOffAllLabels();
                    }
                }
                else if(timeSinceTip > 1.0f && theTouch.phase == TouchPhase.Began)
                {
                    if(UpgradeScreen.activeSelf == false && UI.GetComponent<UI>().progressScreen.activeSelf == false)
                        UnpauseGame();
                    JustSeenToBeenSeen();
                    TurnOffAllLabels();

                    if(endOfTutorialShown == TipStatus.BeenSeen)
                    {
                        // move to main game
                        FinishTutorial();
                    }
                }
            }
        }
        
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt("hitRock", (int)hitRock);
        PlayerPrefs.SetInt("died", (int)died);
        PlayerPrefs.SetInt("diedFromMapShown", (int)diedFromMapShown);
        PlayerPrefs.SetInt("swipeUpTipShown", (int)swipeUpTipShown);
        PlayerPrefs.SetInt("afterPreyShown", (int)afterPreyShown);
        PlayerPrefs.SetInt("upgradeTipShown", (int)upgradeTipShown);
        PlayerPrefs.SetInt("evoPointsTipShown", (int)evoPointsTipShown);
        PlayerPrefs.SetInt("preyRunsShown", (int)preyRunsShown);
        PlayerPrefs.SetInt("battleStageShown", (int)battleStageShown);
        PlayerPrefs.SetInt("autoAttackShown", (int)autoAttackShown);
        PlayerPrefs.SetInt("crownShown", (int)crownShown);
        PlayerPrefs.SetInt("endOfTutorialShown", (int)endOfTutorialShown);
    }
}

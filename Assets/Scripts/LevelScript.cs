using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelScript : MonoBehaviour
{

    public float levelSpeed = 4.5f;
    public bool stationary = false;
    public bool isMoving = false;
    public bool pauseGame = false;
    public bool isTutorial = false;

    protected GameObject lastObjectPlaced = null;
    protected static float starting_minZ_SpawnDistance = 6.0f;
    public float minZ_SpawnDistance = starting_minZ_SpawnDistance; // decreases as time goes by
    protected float ending_SpawnDistance = 3.0f; // make it so that this decreases as you get more totalEvopoints (closer to evolutions)
    public GameObject player;
    public GameObject playerCam;
    public GameObject battleStage;

    public GameObject rockPrefab;
    public List<GameObject> preyPrefabs;
    public GameObject battleStagePrefab;
    protected GameObject UI;
    protected GameObject UpgradeScreen;

    protected float timeElapsed = 0.0f;
    public float timeToStopBuilding = 33.0f; // x seconds to stop building the level forward
    public float timeTillStage = 45.0f; // x seconds until battle stage

    private List<GameObject> flowers = new List<GameObject> { }; // list of flower prefabs for decoration
    public GameObject lastDecorationPlaced;

    private GameObject adsManager;


    void Awake()
    {

        SaveGameScript.Load();

        playerCam = GameObject.Find("Main Camera");

        adsManager = GameObject.Find("Ads Manager");
        if (AdsManager.interstitialAd == null || AdsManager.interstitialAd.CanShowAd() == false)
        {
            adsManager.GetComponent<AdsInitializer>().LoadInterstitialAd();
        }

        // check if you need tutoral

        if (PlayerPrefs.GetInt("FinishTutorial", 0) == 0)
        {
            SceneManager.LoadSceneAsync(2);// tutorial scene
        }
        else
        {
            if(PlayerPrefs.GetInt("NumberOfRuns", 0) == 0)
                playerCam.GetComponent<CameraScript>().stageIntro = true;
        }
        
    }

    // Start is called before the first frame update
    protected void Start()
    {
        player = GameObject.Find("Reptile");
        
        lastObjectPlaced = GameObject.Find("StartingRock");
        UI = GameObject.Find("UIDocument");
        UpgradeScreen = GameObject.Find("UpgradeScreen");
        if (!isTutorial)
        {
            switch (GameState.current.currentReptile().stage_levels["ForestStage"])
            {
                case 1:
                    battleStagePrefab = Resources.Load("Prefabs/1_BattleStage") as GameObject;
                    break;
                case 2:
                    battleStagePrefab = Resources.Load("Prefabs/2_BattleStage") as GameObject;
                    break;
            }
        }

        flowers.Add(Resources.Load("Prefabs/flower01") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower02") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower03") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower04") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower05") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower06") as GameObject);

        // change after conservatory disables it
        RenderSettings.fog = true;

        // check if we need to enabled AdOffer Screen
        if (AdsManager.timeTillAdOffer <= 0)
        {
            GameObject.Find("AdOffers").SetActive(true);
            AdsManager.timeTillAdOffer = AdsManager.AD_OFFER_TIMER;
        } else
        {
            GameObject.Find("AdOffers").SetActive(false);
            GameObject.Find("LoadingCanvas").SetActive(true);
        }
    }

    public string GetCurrentStage()
    {
        if (timeElapsed > timeTillStage)
        {
            return "BATTLE";
        }

        return "RUN";
    }

    public void EndRun() { // ends run & shows progressScreen
        if (UI.GetComponent<UI>().progressScreen.activeSelf) { // if progressScreen already displayed, then don't do anything
            return;
        }
        player.GetComponent<ReptileScript>().animator.SetBool("isDead", true);

        isMoving = false;
        SaveGameScript.Save();

        if (UI.GetComponent<UI>().progressScreen == null)
            UI.GetComponent<UI>().GetUIDocuments();

        UI.GetComponent<UI>().progressScreen.SetActive(true);

        // after progressScreen is re-activated, you have to get all the previous variables again
        VisualElement progressRoot = UI.GetComponent<UI>().progressScreen.GetComponent<UIDocument>().rootVisualElement;
        Button progressContinueButton = progressRoot.Q<Button>("Continue");
        progressContinueButton.RegisterCallback<ClickEvent>(EndGame);

        
        VisualElement bgImage = progressRoot.Q<VisualElement>("BGImage");
        IStyle bgStyle = bgImage.style;
        switch (GameState.current.currentReptile().currentEvolution)
        {
            case 1:
                bgStyle.backgroundImage = Background.FromTexture2D(Resources.Load("Evolutions/GeckoStage2_whiteout") as Texture2D);
                break;
            case 2:
                bgStyle.backgroundImage = Background.FromTexture2D(Resources.Load("Evolutions/Gecko3-whiteout") as Texture2D);
                break;
        }
        

        /*
        VisualElement progressImage = progressRoot.Q<VisualElement>("ProgressImage");
        IStyle progressImageStyle = progressImage.style;
        progressImageStyle.height = new StyleLength(Length.Percent(GameState.current.progressTowardsEvolution() * 100.0f));
        print(progressImageStyle.height);
        progressImageStyle.top = new StyleLength(Length.Percent(100.0f - (GameState.current.progressTowardsEvolution() * 100.0f) ));
        print(progressImageStyle.top);
        */

        VisualElement progressBar = progressRoot.Q<VisualElement>("ProgressBar");
        IStyle progressStyle = progressBar.style;
        progressStyle.width = new StyleLength(Length.Percent(GameState.current.progressTowardsEvolution() * 100));

        UI.GetComponent<UI>().upgradeScreen.SetActive(false);
    }

    public void EndGame(ClickEvent evt)
    {
        PlayerPrefs.SetInt("NumberOfRuns", PlayerPrefs.GetInt("NumberOfRuns", 0) + 1);
        PlayerPrefs.Save();

        

        if (isTutorial)
        {
            SceneManager.LoadSceneAsync(2);

        }
        else
        {
            if (AdsManager.timeTillAd < 0)
            {
                adsManager.GetComponent<AdsInitializer>().ShowInterstitialAd();
                AdsManager.timeTillAd = AdsManager.AD_TIMER;
            }
            else
            {
                SceneManager.LoadSceneAsync(0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            AdsManager.timeTillAd -= Time.deltaTime;
            AdsManager.timeTillAdOffer -= Time.deltaTime;
        }

        if (Mathf.Abs(player.transform.position.x) > 1.5)
        { // you have falled off map
            EndRun();
        }
        if (player.GetComponent<ReptileScript>().health <= 0.0f)
        {
            EndRun();
        }

        if (timeElapsed >= timeTillStage)
        {
            // enter battle stage
            if (battleStage == null)
            {
                UI.GetComponent<UI>().SwitchToBattleUI();

                battleStage = Instantiate(battleStagePrefab, gameObject.transform, true);
                battleStage.transform.position = new Vector3(0, 0, player.transform.position.z + 5.0f);

                // center player
                player.GetComponent<Transform>().position = new Vector3(0, player.transform.position.y, player.transform.position.z);

                player.GetComponent<ReptileScript>().battleStage = battleStage;
                playerCam.GetComponent<CameraScript>().BattleModeSetup(battleStage);
            }
        }

        float progressTowardsStage = 1 - (timeToStopBuilding - timeElapsed) / timeToStopBuilding; // from 0.0 to 1.0 (reach stage)
        minZ_SpawnDistance = (progressTowardsStage * ending_SpawnDistance) + ((1 - progressTowardsStage) * starting_minZ_SpawnDistance);

        // creating the level as you go
        if (timeElapsed < timeToStopBuilding && lastObjectPlaced != null 
                && (Vector3.Distance(player.transform.position, lastObjectPlaced.transform.position) < 50 || lastObjectPlaced.transform.position.y < -2) )/*if off map*/ {
            // figure out position
            float randX = Random.Range(-1.35f, 1.35f);
            float randZ = Random.Range(minZ_SpawnDistance, minZ_SpawnDistance * 1.5f);
            Vector3 newObject_position = lastObjectPlaced.transform.position + new Vector3(0.0f, 0.0f, randZ);

            float typeToSpawn = Random.Range(0.0f, 1.0f);
            // percents: stone/0.3, ladybug/0.45, spider/0.175, butterfly/0.075
            if (typeToSpawn > 0.7)
            {
                lastObjectPlaced = Instantiate(rockPrefab, gameObject.transform, true);
                float randRot = Random.Range(0.0f, 360.0f);
                lastObjectPlaced.transform.Rotate(0, randRot, 0);
            }
            else if (typeToSpawn > 0.25) // ladybug
            {
                if (Mathf.Abs(randX) > 1.25)
                    randX = Random.Range(-1.25f, 1.25f);
                newObject_position.y = -0.009f;
                lastObjectPlaced = Instantiate(preyPrefabs[0], gameObject.transform, true);
            }
            else if (typeToSpawn > 0.075)
            {
                if (Mathf.Abs(randX) > 1.25)
                    randX = Random.Range(-1.25f, 1.25f);
                newObject_position.z += 0.5f;
                lastObjectPlaced = Instantiate(preyPrefabs[1], gameObject.transform, true);
            }
            else // butterfly
            {
                if(Mathf.Abs(randX) > 1.2)
                    randX = Random.Range(-1.2f, 1.2f);
                newObject_position.z += 1.0f; // adds some space from last object so you can actually react to butterfly
                lastObjectPlaced = Instantiate(preyPrefabs[2], gameObject.transform, true);
            }
            newObject_position.x = randX;
            lastObjectPlaced.transform.position = newObject_position;
        }

        // adding decoration
        if(lastDecorationPlaced != null && Vector3.Distance(player.transform.position, lastDecorationPlaced.transform.position) < 50)
        {
            // figure out position
            float randX = Random.Range(-1.4f, 1.4f);
            float randZ = Random.Range(1.5f, 2.5f);
            Vector3 newObject_position = lastDecorationPlaced.transform.position + new Vector3(0.0f, 0.0f, randZ);
            newObject_position.x = randX;

            int typeToSpawn = Random.Range(0, 6);
            lastDecorationPlaced = Instantiate(flowers[typeToSpawn], gameObject.transform, true);
            lastDecorationPlaced.transform.position = newObject_position;
        }

        // deleting objects that have passed
        foreach (Transform child in transform)
        {
            if (child.position.z < -2.5 && (lastObjectPlaced == null || child != lastObjectPlaced.transform))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        if (isMoving && timeElapsed < timeTillStage)
        {
            timeElapsed += Time.deltaTime;
            Vector3 currPos = gameObject.GetComponent<Transform>().position;
            gameObject.GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y, currPos.z - Time.deltaTime * levelSpeed);
        }
        else if (!stationary && UI.GetComponent<UI>().upgradeScreen.activeSelf == false && UI.GetComponent<UI>().progressScreen.activeSelf == false 
            && (!playerCam.GetComponent<CameraScript>().stageIntro && !playerCam.GetComponent<CameraScript>().tutorialIntro) )
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            { // currently touching
                isMoving = true;
            }
        }
    }

    void OnDestroy()
    {
        SaveGameScript.Save();
    }
}

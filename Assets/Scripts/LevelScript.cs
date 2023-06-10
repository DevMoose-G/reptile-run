using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class LevelScript : MonoBehaviour
{

    public float levelSpeed = 4.5f;
    public bool stationary = false;
    public bool isMoving = false;

    protected GameObject lastObjectPlaced = null;
    protected static float starting_minZ_SpawnDistance = 6.0f;
    protected float minZ_SpawnDistance = starting_minZ_SpawnDistance; // decreases as time goes by
    protected float ending_SpawnDistance = 3.0f; // make it so that this decreases as you get more totalEvopoints (closer to evolutions)
    public GameObject player;
    public GameObject playerCam;
    public GameObject battleStage;

    public GameObject treePrefab;
    public List<GameObject> preyPrefabs;
    public GameObject battleStagePrefab;
    protected GameObject UI;
    protected GameObject UpgradeScreen;
    protected GameObject ProgressScreen;

    protected float timeElapsed = 0.0f;
    public float timeToStopBuilding = 33.0f; // x seconds to stop building the level forward
    public float timeTillStage = 45.0f; // x seconds until battle stage

    private List<GameObject> flowers = new List<GameObject> { }; // list of flower prefabs for decoration
    public GameObject lastDecorationPlaced;

    // Start is called before the first frame update
    protected void Start()
    {
        player = GameObject.Find("Reptile");
        playerCam = GameObject.Find("Main Camera");
        lastObjectPlaced = GameObject.Find("StartingRock");
        UI = GameObject.Find("UIDocument");
        UpgradeScreen = GameObject.Find("UpgradeScreen");
        ProgressScreen = GameObject.Find("ProgressScreen");

        flowers.Add(Resources.Load("Prefabs/flower01") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower02") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower03") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower04") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower05") as GameObject);
        flowers.Add(Resources.Load("Prefabs/flower06") as GameObject);
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

        UI.GetComponent<UI>().progressScreen.SetActive(true);

        // after progressScreen is re-activated, you have to get all the previous variables again
        VisualElement progressRoot = UI.GetComponent<UI>().progressScreen.GetComponent<UIDocument>().rootVisualElement;
        Button progressContinueButton = progressRoot.Q<Button>("Continue");
        progressContinueButton.RegisterCallback<ClickEvent>(UI.GetComponent<UI>().EndGame);

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

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(player.transform.position.x) > 1.5)
        { // you have falled off map
            Debug.Log("You have fallen off the map");
            EndRun();
        }
        if (player.GetComponent<ReptileScript>().health <= 0)
        {
            EndRun();
        }

        if (timeElapsed >= timeTillStage)
        {
            // enter battle stage
            if (battleStage == null)
            {
                Debug.Log("ENTERING BATTLE STAGE");
                battleStage = Instantiate(battleStagePrefab, gameObject.transform, true);
                battleStage.transform.position = new Vector3(0, 0, player.transform.position.z + 5.0f);

                // center player
                player.GetComponent<Transform>().position = new Vector3(0, player.transform.position.y, player.transform.position.z);

                player.GetComponent<ReptileScript>().battleStage = battleStage;
                print("BATTLE STAGE");
                print(player.GetComponent<ReptileScript>().battleStage);
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
            newObject_position.x = randX;

            float typeToSpawn = Random.Range(0.0f, 1.0f);
            // percents: stone/0.5, ladybug/0.35, spider/0.1, butterfly/0.05
            if (typeToSpawn > 0.5)
            {
                lastObjectPlaced = Instantiate(treePrefab, gameObject.transform, true);
                float randRot = Random.Range(0.0f, 360.0f);
                lastObjectPlaced.transform.Rotate(0, randRot, 0);
            }
            else if (typeToSpawn > 0.15)
            {
                lastObjectPlaced = Instantiate(preyPrefabs[0], gameObject.transform, true);
            }
            else if (typeToSpawn > 0.05)
            {
                lastObjectPlaced = Instantiate(preyPrefabs[1], gameObject.transform, true);
            }
            else 
            {
                lastObjectPlaced = Instantiate(preyPrefabs[2], gameObject.transform, true);
            }
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
        else if (!stationary && UpgradeScreen.activeSelf == false && ProgressScreen.activeSelf == false)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            { // currently touching
                isMoving = true;
            }
        }
    }

    void OnDestroy()
    {
        GameState.current.Save();
    }
}

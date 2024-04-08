using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;

public class ConservatoryScript : MonoBehaviour
{
    public bool loadingScreenOn = true;

    public GameObject reptiles;

    public GameObject UI;
    public Button backToStageButton;

    void Awake()
    {
        SaveGameScript.Load();
    }

        // Start is called before the first frame update
        void Start()
    {
        RenderSettings.fog = false;

        reptiles = GameObject.Find("Reptiles");
        UI = GameObject.Find("UI");
        VisualElement root = UI.GetComponent<UIDocument>().rootVisualElement;
        backToStageButton = root.Q<Button>("BackToStage");
        backToStageButton.RegisterCallback<ClickEvent>(backToStage);

        // load in the reptiles
        for (int i = 0; i < GameState.current.reptiles.Count; i++)
        {
            float randX = Random.Range(-3.5f, 6.0f);
            float randZ = Random.Range(-2.0f, -12.3f);
            GameObject prefab = Resources.Load("Evolutions/ConservatoryReptile") as GameObject;
            prefab = Instantiate(prefab, reptiles.transform);
            GameObject loadedModel = Resources.Load(GameState.current.reptiles[i].getModelLocation()) as GameObject;
            GameObject model = Instantiate(loadedModel, prefab.transform);
            prefab.transform.position = new Vector3(randX, 0, randZ);
        }

        // load in eggs
        for (int i = 0; i < GameState.current.eggs.Count; i++)
        {
            float randX = Random.Range(-3.5f, 6.0f);
            float randZ = Random.Range(-2.0f, -12.3f);
            GameObject prefab = Resources.Load(GameState.current.eggs[i].getModelLocation()) as GameObject;
            prefab = Instantiate(prefab, reptiles.transform);
            prefab.transform.position = new Vector3(randX, 0, randZ);
        }
    }

    void backToStage(ClickEvent evt)
    {
        SceneManager.LoadSceneAsync(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (loadingScreenOn)
        {
            GameObject.Find("LoadingScreen").SetActive(true);
        }
        else if (GameObject.Find("LoadingScreen") != null)
        {
            GameObject.Find("LoadingScreen").SetActive(false);
        }
    }

    void OnDestroy()
    {
        SaveGameScript.Save();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StoreScript : MonoBehaviour
{
    public Button backButton;
    public Label crownsLabel;

    public GameObject loadingScreen;

    void Awake()
    {
        SaveGameScript.Load();
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        backButton = root.Q<Button>("BackButton");
        backButton.RegisterCallback<ClickEvent>(BackToGame);

        crownsLabel = root.Q<Label>("CrownsLabel");

        loadingScreen = GameObject.Find("LoadingScreen");
        loadingScreen.SetActive(false);
    }

    private void BackToGame(ClickEvent evt)
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadSceneAsync(0);
    }

    // Update is called once per frame
    void Update()
    {
        crownsLabel.text = GameState.current.crowns.ToString();
    }
}

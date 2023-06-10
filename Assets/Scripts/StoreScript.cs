using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StoreScript : MonoBehaviour
{
    public Button backButton;
    public Label crownsLabel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        backButton = root.Q<Button>("BackButton");
        backButton.RegisterCallback<ClickEvent>(BackToGame);

        crownsLabel = root.Q<Label>("CrownsLabel");
    }

    private void BackToGame(ClickEvent evt)
    {
        SceneManager.LoadSceneAsync(0);
    }

    // Update is called once per frame
    void Update()
    {
        crownsLabel.text = GameState.current.crowns.ToString();
    }
}

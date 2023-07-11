using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumScript : MonoBehaviour
{

    public float rotationSpeed = 0.25f;
    public GameObject displayModel;
    // Start is called before the first frame update
    void Start()
    {
        switch (GameState.current.currentReptile().currentEvolution)
        {
            case 1:
                GameObject loadedModel = Resources.Load("Evolutions/Gecko_Stage1") as GameObject;
                GameObject newModel = Instantiate(loadedModel, displayModel.transform);
                displayModel.transform.localPosition = new Vector3(0, 0.465f, 0);
                break;
            case 2:
                loadedModel = Resources.Load("Evolutions/Gecko_Stage2") as GameObject;
                newModel = Instantiate(loadedModel, displayModel.transform);
                displayModel.transform.localPosition = new Vector3(0, 0.5f, 0);
                break;
            case 3:
                loadedModel = Resources.Load("Evolutions/Gecko_Stage3") as GameObject;
                newModel = Instantiate(loadedModel, displayModel.transform);
                displayModel.transform.localPosition = new Vector3(0, 0.325f, 0);
                break;
        }
    }

    public void ShowModel(string name, int stage)
    {
        GameObject loadedModel;
        switch (name)
        {
            case "Gecko":
                if(stage == 1)
                {
                    loadedModel = Resources.Load("Evolutions/Gecko_Stage1") as GameObject;
                    GameObject newModel = Instantiate(loadedModel, displayModel.transform);
                    displayModel.transform.localPosition = new Vector3(0, 0.465f, 0);
                } else if (stage == 2)
                {
                    loadedModel = Resources.Load("Evolutions/Gecko_Stage2") as GameObject;
                    GameObject newModel = Instantiate(loadedModel, displayModel.transform);
                    displayModel.transform.localPosition = new Vector3(0, 0.5f, 0);
                } else if (stage == 3)
                {
                    loadedModel = Resources.Load("Evolutions/Gecko_Stage3") as GameObject;
                    GameObject newModel = Instantiate(loadedModel, displayModel.transform);
                    displayModel.transform.localPosition = new Vector3(0, 0.325f, 0);
                }
                break;
            case "Chameleon":
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch theTouch = Input.GetTouch(0);
            float rotation = -theTouch.deltaPosition.x * rotationSpeed;
            gameObject.transform.Rotate(0, rotation, 0);
        }
    }
}

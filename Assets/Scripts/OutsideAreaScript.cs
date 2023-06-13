using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideAreaScript : MonoBehaviour
{
    public GameObject level;
    public GameObject player;

    public float furthestZ = 0.0f;
    public float distanceFromPlayer = 60.0f;
    public float distanceTillPlayerSees = 40.0f;
    private GameObject lastObjectPlaced;
    public GameObject treeModel;

    public bool addLoadingScreen = true;
    public bool loadingScreenOn = true;

    // Start is called before the first frame update
    void Start()
    {
        level = GameObject.Find("Level");
        player = GameObject.Find("Reptile");
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

        if (level.GetComponent<LevelScript>().isMoving && level.GetComponent<LevelScript>().battleStage == null) {
            Vector3 currPos = gameObject.GetComponent<Transform>().position;
            gameObject.GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y, currPos.z - Time.deltaTime * level.GetComponent<LevelScript>().levelSpeed);
        }

        // creating trees as background
        if (player.transform.position.z + distanceFromPlayer > furthestZ + gameObject.GetComponent<Transform>().position.z) {
            // figure out position
            float randX = Random.Range(-5f, 5f);
            while (randX > -2.0f && randX < 2.0f) {
                randX = Random.Range(-5f, 5f);
            }
            float randZ = Random.Range(furthestZ + 0.15f, furthestZ+0.6f);
            Vector3 newObject_position = new Vector3(randX, 0.0f, randZ);
            furthestZ = randZ;

            lastObjectPlaced = Instantiate(treeModel, gameObject.transform, true);
            lastObjectPlaced.transform.localPosition = newObject_position;
            float randRot = Random.Range(0.0f, 360.0f);
            lastObjectPlaced.transform.Rotate(0, randRot, 0);
        }
        if(addLoadingScreen == false || (player.transform.position.z + distanceTillPlayerSees) < (furthestZ + gameObject.GetComponent<Transform>().position.z))
        {
            loadingScreenOn = false;
        }

        // deleting all trees on the right side when battle happens and all trees behind player
        foreach (Transform child in transform)
        {
            if (level.GetComponent<LevelScript>().battleStage != null && child.position.x > 0)
            {
                DestroyImmediate(child.gameObject);
            }
            if (child != null && child.position.z < player.transform.position.z - 6)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}

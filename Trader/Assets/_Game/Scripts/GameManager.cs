using UnityEngine;
using System.Collections;
using CodeControl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    internal GameObject selectedObj;
    internal Text infoText;

	// Use this for initialization
	void Start () {

        Items coalOre = new Items("Coal Ore", 100, 5, 3);
        coalOre.selfProducing = true;

        StationModel model = new StationModel();

        model.name = "Alpha Station";
        model.position = Vector3.zero;
        model.capacity = 1000;

        Items[] inputs = {};
        Items[] outputs = { coalOre };

        model.factory = new Factory("Coal Mine", inputs, outputs, 2);

        StationController station = Controller.Instantiate<StationController>("station", model);

        //----------------

        Items unrefinedFuel = new Items("Unrefined Fuel", 100, 1, 10);
        unrefinedFuel.selfProducing = true;
        coalOre = new Items("Coal Ore", 100, 5, 1);
        Items fuel = new Items("Fuel", 100, 2, 5);

        model = new StationModel();

        model.name = "Beta Station";
        model.position = new Vector3(0,5,0);
        model.capacity = 1000;

        inputs = new Items[] { coalOre, unrefinedFuel };
        outputs = new Items[]  { fuel };

        model.factory = new Factory("Coal Mine", inputs, outputs, 5);

        station = Controller.Instantiate<StationController>("station", model);

        //----------------

        Items ironOre = new Items("Iron Ore", 100, 1, 15);
        ironOre.selfProducing = true;
        coalOre = new Items("Coal Ore", 100, 5, 2);
        Items steel = new Items("Steel", 100, 10, 5);

        model = new StationModel();

        model.name = "Theta Station";
        model.position = new Vector3(5, 5, 0);
        model.capacity = 1000;

        inputs = new Items[] { ironOre, coalOre };
        outputs = new Items[] { steel };

        model.factory = new Factory("Coal Mine", inputs, outputs, 10);

        station = Controller.Instantiate<StationController>("station", model);

        //------------------------------------------------------------------------------------------------//
        infoText = GameObject.FindGameObjectWithTag("InfoText").GetComponent<Text>();

	
	}
	
	// Update is called once per frame
	void Update () {

        //Mouse Hit

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (hit)
            {
                Debug.Log(hit.transform.name);
                selectedObj = hit.transform.gameObject;
                print("Hit " + selectedObj.name);
            }
        }

        if (selectedObj != null)
        {
            infoText.text = selectedObj.GetComponent<StationController>().info;
        }
    }
}

using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float camMoveSpeed = 1;
    public float zoomSpeed = 1;
    public float cameraScaleModPow = .7f;
    public float cameraScaleModMult = .1f;

    private Camera mainCam;
    private GameManager game;

	// Use this for initialization
	void Start () {

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        game = GameManager.instance;
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.setup){
            //Camera Controls
            double moveModifier = camMoveSpeed * game.data.cameraOrth;
            var orthTransform = Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;
            if (game.data.cameraOrth + orthTransform < GameDataModel.galaxyCameraScaleMax)
            {
                game.data.cameraOrth += orthTransform;
            }
            game.data.cameraScaleMod =  Mathd.Pow(mainCam.orthographicSize / game.data.cameraOrth, cameraScaleModPow) * cameraScaleModMult;
            game.data.distanceDivision = GameDataModel.galaxyDistanceMultiplication * mainCam.orthographicSize / game.data.cameraOrth; //TODO: Maybe finish creating a new way to scale down and avoid precision error

            double transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime / Time.timeScale;
            double transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime / Time.timeScale;

            game.data.cameraPosition += new Vector2d(transX, transY);
        }
    }

    public static Vector3 CameraOffsetPoistion(Vector3d position)
    {
        return (Vector3)(new Vector3d(position.x - GameManager.instance.data.cameraPosition.x, position.y - GameManager.instance.data.cameraPosition.y)
                    * Camera.main.orthographicSize / GameManager.instance.data.cameraOrth);
    }

    public static SolarModel ClosestSolar()
    {
        var data = GameManager.instance.data;
        var closestSolar = data.stars[0];
        var distance = Vector2d.Distance(data.stars[0].galacticPosition, data.cameraPosition);
        {
            foreach (SolarModel star in GameManager.instance.data.stars)
            {
                var newDistance = Vector2d.Distance(star.galacticPosition, data.cameraPosition);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closestSolar = star;
                }
            }
        }

        return closestSolar;
    }
}

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
            double moveModifier = camMoveSpeed * game.data.cameraGalaxyOrtho;
            var orthTransform = Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;
            if (game.data.cameraGalaxyOrtho + orthTransform < GameDataModel.galaxyCameraScaleMax)
            {
                game.data.cameraGalaxyOrtho += orthTransform;
            }
            game.data.cameraGalCameraScaleMod =  Mathd.Pow(mainCam.orthographicSize / game.data.cameraGalaxyOrtho, cameraScaleModPow) * cameraScaleModMult;
            //game.data.distanceDivision = GameDataModel.galaxyDistanceMultiplication * mainCam.orthographicSize / game.data.cameraGalaxyOrtho; //TODO: Maybe finish creating a new way to scale down and avoid precision error
            double transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime / Time.timeScale;
            double transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime / Time.timeScale;

            game.data.cameraGalaxyPosition += new Vector2d(transX, transY);

            //Solar Camera Settings
            //moveModifier = camMoveSpeed * game.data.cameraSolarOrtho;
            //var solOrthoTransform = Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;
            //if (game.data.cameraGalaxyOrtho + orthTransform < GameDataModel.galaxyCameraScaleMax)
            //{
            //    game.data.cameraSolarOrtho += solOrthoTransform;
            //}
            //game.data.cameraSolCameraScaleMod = Mathd.Pow(mainCam.orthographicSize / game.data.cameraSolarOrtho, cameraScaleModPow) * cameraScaleModMult;
            //transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime / Time.timeScale;
            //transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime / Time.timeScale;

            //game.data.cameraSolarPosition += new Vector2d(transX, transY);

        }
    }

    public static Vector3 CameraOffsetGalaxyPosition(Vector3d position)
    {
        return (Vector3)(new Vector3d(position.x - GameManager.instance.data.cameraGalaxyPosition.x, position.y - GameManager.instance.data.cameraGalaxyPosition.y)
                    * Camera.main.orthographicSize / GameManager.instance.data.cameraGalaxyOrtho);
    }
    public static Vector3 CameraOffsetSolarPosition(Vector3d position)
    {
        return (Vector3)(new Vector3d(position.x - GameManager.instance.data.cameraSolarPosition.x, position.y - GameManager.instance.data.cameraSolarPosition.y)
                    * Camera.main.orthographicSize / GameManager.instance.data.cameraSolarOrtho);
    }
    public static SolarModel ClosestSolar()
    {
        var data = GameManager.instance.data;
        var closestSolar = data.stars[0];
        var distance = Vector2d.Distance(data.stars[0].galacticPosition, data.cameraGalaxyPosition);
        {
            foreach (SolarModel star in GameManager.instance.data.stars)
            {
                var newDistance = Vector2d.Distance(star.galacticPosition, data.cameraGalaxyPosition);
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

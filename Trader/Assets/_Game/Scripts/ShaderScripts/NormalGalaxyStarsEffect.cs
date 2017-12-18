using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGalaxyStarsEffect : MonoBehaviour {

    public Material NormalGalaxyMaterial;
    public Material SolarTempMaterial;
    public Material GalaxyTerritoryMaterial;
    public float starPow = .02f;
    public float starDiv = 1f;
    public float influenceScale = 1;
    public float backgroundStarRadi = .35f;
    public float backgroundStarRadi2 = .35f;
    public float backgroundStarPos1 = .1f;
    public float backgroundStarPos2 = .1f;
    public float backgroundStarPosAdd = 10f;
    public float backgroundSunPosition = .1f;
    public float backgroundSunRadi1 = .35f;
    public float backgroundSunRadi2 = .35f;
    private List<Vector4> positions;
    private List<Vector4> normalColors;
    private List<Vector4> territoryColors;
    private List<float> starRadi;
    private List<float> influenceRadi;
    private GameManager game;

    [ExecuteInEditMode]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (GameManager.instance.galaxyView)
        {
            if ((positions.Count > 0 && MapTogglePanel.instance.galaxyNormalVisual.isOn))
            {
                Graphics.Blit(source, destination, NormalGalaxyMaterial);
            }
            else if (positions.Count > 0 && MapTogglePanel.instance.galaxyTerritory.isOn)
            {
                Graphics.Blit(source, destination, GalaxyTerritoryMaterial);
            }
            else
            {
                Graphics.Blit(source, destination, NormalGalaxyMaterial);
            }
        }
        else
        {
            if (MapTogglePanel.instance.visualDisplay[VisualDisplay.Temperature].isOn)
            {
                Graphics.Blit(source, destination, SolarTempMaterial);
            }
            else
            {
                Graphics.Blit(source, destination, NormalGalaxyMaterial);
            }
        }
        
        
    }
    [ExecuteInEditMode]
    public void Start()
    {
        game = GameManager.instance;
        positions = new List<Vector4>();
        normalColors = new List<Vector4>();
        territoryColors = new List<Vector4>();
        starRadi = new List<float>();
        influenceRadi = new List<float>();
       
    }
    public void Update()
    {
        if (MapTogglePanel.instance.solarNormalVisual.isOn || MapTogglePanel.instance.galaxyNormalVisual.isOn)
        {
            if (positions.Count == 0)
            {
                foreach (SolarModel star in game.data.stars)
                {
                    positions.Add(new Vector4((float)(star.galaxyPosition.x - game.data.cameraGalaxyPosition.x),(float) (star.galaxyPosition.y - game.data.cameraGalaxyPosition.y)));
                    normalColors.Add(new Vector4(star.solar.color.r, star.solar.color.g, star.solar.color.b, star.solar.color.a));
                    territoryColors.Add(Vector4.zero);
                    starRadi.Add((float)Mathd.Pow((star.solar.bodyRadius), starPow) / starDiv);
                    influenceRadi.Add((float)Mathd.Pow((star.solar.bodyRadius), starPow) / starDiv);
                }
                return;
            }
            for (int i = 0; i < game.data.stars.Count; i++)
            {
                Vector3 position = CameraController.CameraOffsetGalaxyPosition(game.data.stars[i].galaxyPosition);
                float localScale = (float)(Mathd.Pow((game.data.stars[i].solar.bodyRadius), starPow) / starDiv * game.data.cameraGalCameraScaleMod); ;

                if (!game.galaxyView)
                {
                    if (position.sqrMagnitude < 60000)
                    {
                        if (localScale * game.data.cameraGalaxyOrtho / Camera.main.orthographicSize * GameDataModel.galaxyDistanceMultiplication < game.data.stars[i].solar.bodyRadius)
                        {
                            localScale = (float)(game.data.stars[i].solar.bodyRadius / GameDataModel.galaxyDistanceMultiplication * Camera.main.orthographicSize / game.data.cameraGalaxyOrtho);
                        }
                    }
                    else
                    {
                        if (game.data.stars[i] == GalaxyManager.instance.solarModel)
                        {

                            position *= Mathf.Pow((float)game.data.cameraGalaxyOrtho / Camera.main.orthographicSize, backgroundStarPos1) * backgroundSunPosition;
                            localScale *= Mathf.Pow((float)game.data.cameraGalaxyOrtho / Camera.main.orthographicSize, backgroundSunRadi2) / Mathf.Pow(position.sqrMagnitude, backgroundSunRadi1);
                        }
                        else
                        {
                            position *= Mathf.Pow((float)game.data.cameraGalaxyOrtho / Camera.main.orthographicSize, backgroundStarPos1);
                            localScale *= Mathf.Pow((float)game.data.cameraGalaxyOrtho / Camera.main.orthographicSize, backgroundStarRadi) / Mathf.Pow(position.sqrMagnitude, backgroundStarRadi2);
                            position += position.normalized * (backgroundStarPosAdd / Mathf.Pow(position.magnitude, backgroundStarPos2));
                            
                            
                        }
                        
                    }
                }
                positions[i] = (new Vector4(position.x, position.y));
                normalColors[i] = (new Vector4(game.data.stars[i].solar.color.r, game.data.stars[i].solar.color.g, game.data.stars[i].solar.color.b, game.data.stars[i].solar.color.a));                
                starRadi[i] = (localScale);
            }
            NormalGalaxyMaterial.SetInt("_StarPositionLength", positions.Count);
            Shader.SetGlobalVectorArray("_StarPositionArray", positions);
            Shader.SetGlobalVectorArray("_StarColorArray", normalColors);
            Shader.SetGlobalFloatArray("_StarRadi", starRadi);
        }
        if (MapTogglePanel.instance.galaxyTerritory.isOn && game.galaxyView)
        {
            if (positions.Count == 0)
            {
                foreach (SolarModel star in game.data.stars)
                {
                    positions.Add(new Vector4((float)(star.galaxyPosition.x - game.data.cameraGalaxyPosition.x), (float)(star.galaxyPosition.y - game.data.cameraGalaxyPosition.y)));
                    normalColors.Add(new Vector4(star.solar.color.r, star.solar.color.g, star.solar.color.b, star.solar.color.a));
                    territoryColors.Add(Vector4.zero);
                    starRadi.Add((float)Mathd.Pow((star.solar.bodyRadius), starPow) / starDiv);
                    influenceRadi.Add((float)Mathd.Pow((star.solar.bodyRadius), starPow) / starDiv);
                }
                return;
            }
            for (int i = 0; i < game.data.stars.Count; i++)
            {
                var position = CameraController.CameraOffsetGalaxyPosition(game.data.stars[i].galaxyPosition);
                positions[i] = (new Vector4(position.x, position.y));
                if (game.data.stars[i].government.Model != null)
                {
                    territoryColors[i] = (new Vector4(game.data.stars[i].government.Model.spriteColor.r, game.data.stars[i].government.Model.spriteColor.g,
                    game.data.stars[i].government.Model.spriteColor.b, game.data.stars[i].government.Model.spriteColor.a));
                }
                else
                {
                    territoryColors[i] = new Vector4(.1f, .1f, .1f, 1);
                }

                influenceRadi[i] = (float)(game.data.cameraGalCameraScaleMod * game.data.stars[i].governmentInfluence * influenceScale + .1);
            }
            NormalGalaxyMaterial.SetInt("_StarPositionLength", positions.Count);
            Shader.SetGlobalVectorArray("_StarPositionArray", positions);
            Shader.SetGlobalVectorArray("_StarColorArray", territoryColors);
            Shader.SetGlobalFloatArray("_StarRadi", influenceRadi);
        }
        if (!GameManager.instance.galaxyView && MapTogglePanel.instance.visualDisplay[VisualDisplay.Temperature].isOn)
        {
            SolarTempMaterial.SetVector("_StarPosition", CameraController.CameraOffsetGalaxyPosition(GalaxyManager.instance.solarModel.galaxyPosition));
            SolarTempMaterial.SetFloat("_StarTemperature", (float)GalaxyManager.instance.solarModel.solar.surfaceTemp);
            SolarTempMaterial.SetFloat("_StarLum", (float)GalaxyManager.instance.solarModel.solar.luminosity);
            SolarTempMaterial.SetFloat("_BondAlebo", (float)GalaxyManager.instance.solarModel.solar.bondAlebo);
            SolarTempMaterial.SetFloat("_Greenhouse", (float)GalaxyManager.instance.solarModel.solar.greenhouse);
            SolarTempMaterial.SetFloat("_DistanceMod",(float) GameDataModel.galaxyDistanceMultiplication);
            SolarTempMaterial.SetFloat("_CameraOrtho", (float) GameManager.instance.data.cameraGalaxyOrtho);
        }

    }
}

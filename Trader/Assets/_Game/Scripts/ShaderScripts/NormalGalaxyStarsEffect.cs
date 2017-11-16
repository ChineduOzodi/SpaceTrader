using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGalaxyStarsEffect : MonoBehaviour {

    public Material NormalGalaxyMaterial;
    public Material SolarTempMaterial;
    public float starPow = .02f;
    public float starDiv = 1f;
    private List<Vector4> positions;
    private List<Vector4> colors;
    private List<float> starRadi;
    private GameManager game;

    [ExecuteInEditMode]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if ((positions.Count > 0 && GameManager.instance.galaxyView))
        {
            Graphics.Blit(source, destination, NormalGalaxyMaterial);
        }
        else if (MapTogglePanel.instance.visualDisplay[VisualDisplay.Temperature].isOn && !GameManager.instance.galaxyView)
        {
            Graphics.Blit(source, destination, SolarTempMaterial);
        }
        else
        {
            Graphics.Blit(source, destination, NormalGalaxyMaterial);
        }
    }
    [ExecuteInEditMode]
    public void Start()
    {
        game = GameManager.instance;
        positions = new List<Vector4>();
        colors = new List<Vector4>();
        starRadi = new List<float>();
       
    }
    public void Update()
    {
        if (GameManager.instance.galaxyView || MapTogglePanel.instance.visualDisplay[VisualDisplay.Normal].isOn)
        {
            if (positions.Count == 0)
            {
                foreach (SolarModel star in game.data.stars)
                {
                    positions.Add(new Vector4((float)(star.galacticPosition.x - game.data.cameraPosition.x),(float) (star.galacticPosition.y - game.data.cameraPosition.y)));
                    colors.Add(new Vector4(star.solar.color.r, star.solar.color.g, star.solar.color.b, star.solar.color.a));
                    starRadi.Add((float)Mathd.Pow((star.solar.bodyRadius), starPow) / starDiv);
                }
                return;
            }
            for (int i = 0; i < game.data.stars.Count; i++)
            {
                var position = CameraController.CameraOffsetPoistion(game.data.stars[i].galacticPosition);
                positions[i] = (new Vector4(position.x, position.y));
                starRadi[i] = (float)((Mathd.Pow((game.data.stars[i].solar.bodyRadius), starPow) / starDiv) * game.data.cameraScaleMod);
            }
            NormalGalaxyMaterial.SetInt("_StarPositionLength", positions.Count);
            Shader.SetGlobalVectorArray("_StarPositionArray", positions);
            Shader.SetGlobalVectorArray("_StarColorArray", colors);
            Shader.SetGlobalFloatArray("_StarRadi", starRadi);
        }
        if (!GameManager.instance.galaxyView && MapTogglePanel.instance.visualDisplay[VisualDisplay.Temperature].isOn)
        {
            SolarTempMaterial.SetVector("_StarPosition", CameraController.CameraOffsetPoistion(GalaxyManager.instance.solarModel.galacticPosition));
            SolarTempMaterial.SetFloat("_StarTemperature", (float)GalaxyManager.instance.solarModel.solar.surfaceTemp);
            SolarTempMaterial.SetFloat("_StarLum", (float)GalaxyManager.instance.solarModel.solar.luminosity);
            SolarTempMaterial.SetFloat("_BondAlebo", (float)GalaxyManager.instance.solarModel.solar.bondAlebo);
            SolarTempMaterial.SetFloat("_Greenhouse", (float)GalaxyManager.instance.solarModel.solar.greenhouse);
            SolarTempMaterial.SetFloat("_DistanceMod",(float) GameDataModel.galaxyDistanceMultiplication);
            SolarTempMaterial.SetFloat("_CameraOrtho", (float) GameManager.instance.data.cameraOrth);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomImageEffect : MonoBehaviour {

    public Material EffectMaterial;
    public float starPow = .02f;
    public float starDiv = 1f;
    private List<Vector4> positions;
    private List<Vector4> colors;
    private List<float> starRadi;
    private GameManager game;

    [ExecuteInEditMode]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (positions.Count > 0)
        {
            Graphics.Blit(source, destination, EffectMaterial);
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
        if(positions.Count == 0)
        {
            foreach (SolarModel star in game.data.stars)
            {
                positions.Add(new Vector4(star.galacticPosition.x, star.galacticPosition.y));
                colors.Add(new Vector4(star.solar.color.r, star.solar.color.g, star.solar.color.b, star.solar.color.a));
                starRadi.Add((float)Mathd.Pow((star.solar.bodyRadius), starPow) / starDiv);
            }
            return;
        }
        for (int i = 0; i < game.data.stars.Count; i++)
        {
            starRadi[i] = ((float)Mathd.Pow((game.data.stars[i].solar.bodyRadius), starPow) / starDiv);
        }
        EffectMaterial.SetInt("_StarPositionLength", positions.Count);
        Shader.SetGlobalVectorArray("_StarPositionArray", positions);
        Shader.SetGlobalVectorArray("_StarColorArray", colors);
        Shader.SetGlobalFloatArray("_StarRadi", starRadi);
    }
}

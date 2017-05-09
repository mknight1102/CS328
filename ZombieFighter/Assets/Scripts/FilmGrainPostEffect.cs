using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FilmGrainPostEffect : MonoBehaviour {

    //public Material mat;
    public Shader filmShader;
    Material filmMat;

    private void Start()
    {
        filmMat = new Material(filmShader);
    }
   
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //make the temporary rendertexture
        RenderTexture TempRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);

        //put it to video memory
        TempRT.Create();

        filmMat.SetTexture("_SceneTex", source);

        Graphics.Blit(TempRT, destination, filmMat);

        TempRT.Release();
    }
}

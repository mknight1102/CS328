using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class BloodPostEffect : MonoBehaviour
{
    public Shader BloodMapShader;
    public Shader BloodAnimShader;
    public RenderTexture gravityMap;

    private Material BloodMapMat;
    private Material BloodAnimMat;


    // Use this for initialization
    void Start()
    {
        BloodMapMat = new Material(BloodMapShader);
        BloodAnimMat = new Material(BloodAnimMat);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, gravityMap, BloodMapMat);   
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class BloodPostEffect : MonoBehaviour
{
    public Shader BloodMapShader;
    public Shader BloodAnimShader;

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
        Graphics.Blit(source, destination, BloodMapMat);
        BloodAnimMat.SetTexture("_MainTex", BloodMapMat.GetTexture("_MainTex"));
        Graphics.Blit(source, destination, BloodAnimMat);
        
        

    }
}

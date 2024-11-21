using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShaderProperties : MonoBehaviour
{
    void Update()
    {
        if(gameObject.transform.hasChanged)
        {
            Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
            foreach(var r in renderers)
            {
                Material m;
                m = r.material;
                if(string.Compare(m.shader.name, "Shader Graphs/ToonRamp") == 0)
                {
                    m.SetVector("_LightDir", transform.forward);
                }
            }
        }
    }
}

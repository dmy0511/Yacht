using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 셰이더의 속성값을 업데이트하는 클래스 (특히 ToonRamp 셰이더의 라이트 방향)
public class UpdateShaderProperties : MonoBehaviour
{
    void Update()
    {
        // 오브젝트의 Transform이 변경되었을 때만 실행
        if (gameObject.transform.hasChanged)
        {
            // 씬의 모든 Renderer 컴포넌트 가져오기
            Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
            foreach (var r in renderers)
            {
                Material m;
                m = r.material;
                // ToonRamp 셰이더를 사용하는 material인 경우
                if (string.Compare(m.shader.name, "Shader Graphs/ToonRamp") == 0)
                {
                    // 셰이더의 라이트 방향을 현재 오브젝트의 forward 방향으로 설정
                    m.SetVector("_LightDir", transform.forward);
                }
            }
        }
    }
}

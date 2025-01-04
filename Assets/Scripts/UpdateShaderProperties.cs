using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���̴��� �Ӽ����� ������Ʈ�ϴ� Ŭ���� (Ư�� ToonRamp ���̴��� ����Ʈ ����)
public class UpdateShaderProperties : MonoBehaviour
{
    void Update()
    {
        // ������Ʈ�� Transform�� ����Ǿ��� ���� ����
        if (gameObject.transform.hasChanged)
        {
            // ���� ��� Renderer ������Ʈ ��������
            Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
            foreach (var r in renderers)
            {
                Material m;
                m = r.material;
                // ToonRamp ���̴��� ����ϴ� material�� ���
                if (string.Compare(m.shader.name, "Shader Graphs/ToonRamp") == 0)
                {
                    // ���̴��� ����Ʈ ������ ���� ������Ʈ�� forward �������� ����
                    m.SetVector("_LightDir", transform.forward);
                }
            }
        }
    }
}

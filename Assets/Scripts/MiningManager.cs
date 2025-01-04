using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ä�� ���μ����� �����ϴ� �̱��� Ŭ����
public class MiningManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static MiningManager instance;
    public static MiningManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MiningManager");
                instance = go.AddComponent<MiningManager>();
                DontDestroyOnLoad(go);  // �� ��ȯ�ÿ��� ����
            }
            return instance;
        }
    }

    // ä�� ���μ��� ����
    public void StartMiningProcess(SpeedUpCtrl ctrl)
    {
        StartCoroutine(MonitorMining(ctrl));
    }

    // ����� ���൵�� ä�� ���μ��� ����
    public void RestoreMiningProcess(SpeedUpCtrl ctrl, float progress)
    {
        StartMiningProcess(ctrl);
        ctrl.UpdateProgress(progress);
    }

    // ä�� ���൵�� ����͸��ϴ� �ڷ�ƾ
    private IEnumerator MonitorMining(SpeedUpCtrl ctrl)
    {
        float progress = 0f;

        while (true)
        {
            // �� �ӵ� ���� ���
            float totalSpeedMultiplier = ctrl.GetTotalSpeedMultiplier();
            // ���൵ ������Ʈ
            progress += Time.deltaTime * ctrl.GetBaseSpeed() * totalSpeedMultiplier;

            ctrl.UpdateProgress(progress);

            // ä�� �Ϸ�� ���� ���� �� ����
            if (progress >= 1f)
            {
                ctrl.GiveReward();
                progress = 0f;
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }
    }
}

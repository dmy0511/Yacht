using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    private static MiningManager instance;
    public static MiningManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MiningManager");
                instance = go.AddComponent<MiningManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public void StartMiningProcess(SpeedUpCtrl ctrl)
    {
        StartCoroutine(MonitorMining(ctrl));
    }

    public void RestoreMiningProcess(SpeedUpCtrl ctrl, float progress)
    {
        StartMiningProcess(ctrl);
        ctrl.UpdateProgress(progress);
    }

    private IEnumerator MonitorMining(SpeedUpCtrl ctrl)
    {
        float progress = 0f;

        while (true)
        {
            float totalSpeedMultiplier = ctrl.GetTotalSpeedMultiplier();
            progress += Time.deltaTime * ctrl.GetBaseSpeed() * totalSpeedMultiplier;

            ctrl.UpdateProgress(progress);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 채굴 프로세스를 관리하는 싱글톤 클래스
public class MiningManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static MiningManager instance;
    public static MiningManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MiningManager");
                instance = go.AddComponent<MiningManager>();
                DontDestroyOnLoad(go);  // 씬 전환시에도 유지
            }
            return instance;
        }
    }

    // 채굴 프로세스 시작
    public void StartMiningProcess(SpeedUpCtrl ctrl)
    {
        StartCoroutine(MonitorMining(ctrl));
    }

    // 저장된 진행도로 채굴 프로세스 복원
    public void RestoreMiningProcess(SpeedUpCtrl ctrl, float progress)
    {
        StartMiningProcess(ctrl);
        ctrl.UpdateProgress(progress);
    }

    // 채굴 진행도를 모니터링하는 코루틴
    private IEnumerator MonitorMining(SpeedUpCtrl ctrl)
    {
        float progress = 0f;

        while (true)
        {
            // 총 속도 배율 계산
            float totalSpeedMultiplier = ctrl.GetTotalSpeedMultiplier();
            // 진행도 업데이트
            progress += Time.deltaTime * ctrl.GetBaseSpeed() * totalSpeedMultiplier;

            ctrl.UpdateProgress(progress);

            // 채굴 완료시 보상 지급 및 리셋
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

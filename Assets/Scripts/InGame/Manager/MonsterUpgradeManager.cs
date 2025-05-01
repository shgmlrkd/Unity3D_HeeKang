using UnityEngine;

public static class MonsterUpgradeManager
{
    private static readonly int _one = 1;

    // 강화 횟수 계산
    // 공식: (현재 시간 - 스폰 시작 시간) / 강화 간격
    public static int GetUpgradeCount(float currentGameTime, float spawnStartTime, float interval)
    {
        float passedTime = currentGameTime - spawnStartTime;

        if (passedTime < 0)
            return 0;

        return (int)(passedTime / interval);
    }

    // 강화된 능력치 배수 계산
    // 공식: 1 + (강화 횟수 × 증가량) / 스케일 기준값
    public static float GetScaleFactor(int upgradeCount, float stateUpgradeValue, float stateScaleFactor)
    {
        return _one + ((upgradeCount * stateUpgradeValue) / stateScaleFactor);
    }
}

using UnityEngine;

public static class MonsterUpgradeManager
{
    private static readonly int _one = 1;

    // ��ȭ Ƚ�� ���
    // ����: (���� �ð� - ���� ���� �ð�) / ��ȭ ����
    public static int GetUpgradeCount(float currentGameTime, float spawnStartTime, float interval)
    {
        float passedTime = currentGameTime - spawnStartTime;

        if (passedTime < 0)
            return 0;

        return (int)(passedTime / interval);
    }

    // ��ȭ�� �ɷ�ġ ��� ���
    // ����: 1 + (��ȭ Ƚ�� �� ������) / ������ ���ذ�
    public static float GetScaleFactor(int upgradeCount, float stateUpgradeValue, float stateScaleFactor)
    {
        return _one + ((upgradeCount * stateUpgradeValue) / stateScaleFactor);
    }
}

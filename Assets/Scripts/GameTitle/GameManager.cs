using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private string _playerName;
    public string PlayerName
    {
        get { return _playerName; }
    }

    private int _playerKey;
    public int PlayerKey
    {
        get { return _playerKey; }
    }

    private int _skillIndex;
    public int SkillIndex
    {
        get { return _skillIndex; }
    }

    private void Awake()
    {
        // �̹� �ٸ� GameManager�� �����ϸ� �ڽ��� �ı�
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� �� ������Ʈ�� �ı����� ����
        }
        else
        {
            // �̹� �����ϴ� �̱����� �ִٸ� ���� ������ ������Ʈ�� �ı�
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(SoundKey.TitleBGM, 0.01f);
    }

    public void SetPlayer(int key, int skillIndex, string playerPrefabName)
    {
        _playerKey = key;
        _skillIndex = skillIndex;
        _playerName = playerPrefabName;
    }
}

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
        // 이미 다른 GameManager가 존재하면 자신을 파괴
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 이 오브젝트를 파괴하지 않음
        }
        else
        {
            // 이미 존재하는 싱글톤이 있다면 새로 생성된 오브젝트는 파괴
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

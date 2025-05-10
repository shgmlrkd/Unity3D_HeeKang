using Photon.Pun;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private PhotonView _photonView;
    public PhotonView PhotonView
    {
        get { return _photonView; }
    }

    private string[] _bestName = new string[10];
    private float[] _bestClearTime = new float[10];

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

        if (_photonView == null)
        { 
            _photonView = GetComponent<PhotonView>();
        }
    }
    
    public void SetPlayer(int key, int skillIndex, string playerPrefabName)
    {
        _playerKey = key;
        _skillIndex = skillIndex;
        _playerName = playerPrefabName;
    }

    [PunRPC]
    private void RankingSet(string curName, float curClearTime)
    {
        print("랭킹 세팅");
        PlayerPrefs.SetString("CurPlayerName", curName);
        PlayerPrefs.SetFloat("CurClearTime", curClearTime);

        float tempClearTime = 0.0f;
        string tempName = "";

        for (int i = 0; i < _bestClearTime.Length; i++)
        {
            _bestName[i] = PlayerPrefs.GetString(i + "BestName");
            _bestClearTime[i] = PlayerPrefs.GetFloat(i + "BestClearTime");
            if (_bestClearTime[i] == 0.0f)
            {
                _bestClearTime[i] = float.MaxValue;
            }

            while (_bestClearTime[i] > curClearTime)
            {
                tempClearTime = _bestClearTime[i];
                tempName = _bestName[i];
                _bestClearTime[i] = curClearTime;
                _bestName[i] = curName;

                PlayerPrefs.SetString(i + "BestName", curName);
                PlayerPrefs.SetFloat(i + "BestClearTime", curClearTime);

                curClearTime = tempClearTime;
                curName = tempName;
            }
        }

        print("직렬화 시작");
        string serializedData = SerializeRankingData(); // 랭킹 데이터를 직렬화
        print("전체 랭킹 세팅");
        _photonView.RPC("UpdateRanking", RpcTarget.All, serializedData);
    }

    [System.Serializable] // 직렬화 구조체
    public struct RankingData
    {
        public string[] names;
        public float[] scores;
    }

    private string SerializeRankingData()
    {
        // RankingData 구조체에 데이터 저장
        RankingData rankingData = new RankingData
        {
            names = _bestName,
            scores = _bestClearTime
        };

        print("직렬화 끝");
        // RankingData를 JSON으로 직렬화
        return JsonUtility.ToJson(rankingData);
    }

    [PunRPC]
    private void UpdateRanking(string serializedData)
    {
        // 랭킹 데이터를 역직렬화하여 사용
        DeserializeRankingData(serializedData);

        // UI에 랭킹 정보를 업데이트하는 코드
        UpdateRankingUI();
    }

    private void DeserializeRankingData(string serializedData)
    {
        // JSON 문자열을 RankingData 구조체로 역직렬화
        RankingData rankingData = JsonUtility.FromJson<RankingData>(serializedData);

        // 역직렬화된 데이터 할당
        _bestName = rankingData.names;
        _bestClearTime = rankingData.scores;
    }

    private void UpdateRankingUI()
    {
        // 랭킹 갱신
        for (int i = 0; i < _bestName.Length; i++)
        {
            PlayerPrefs.SetString(i + "BestName", _bestName[i]);
            PlayerPrefs.SetFloat(i + "BestClearTime", _bestClearTime[i]);

            // 랭킹 이름과 점수를 UI에 표시
            Debug.Log($"{_bestName[i]}: {_bestClearTime[i]}");
        }
        print("업데이트 랭킹 끝");
    }

    public void DestroyGameManager()
    {
        Destroy(gameObject);
    }
}
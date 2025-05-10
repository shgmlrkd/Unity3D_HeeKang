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
        print("��ŷ ����");
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

        print("����ȭ ����");
        string serializedData = SerializeRankingData(); // ��ŷ �����͸� ����ȭ
        print("��ü ��ŷ ����");
        _photonView.RPC("UpdateRanking", RpcTarget.All, serializedData);
    }

    [System.Serializable] // ����ȭ ����ü
    public struct RankingData
    {
        public string[] names;
        public float[] scores;
    }

    private string SerializeRankingData()
    {
        // RankingData ����ü�� ������ ����
        RankingData rankingData = new RankingData
        {
            names = _bestName,
            scores = _bestClearTime
        };

        print("����ȭ ��");
        // RankingData�� JSON���� ����ȭ
        return JsonUtility.ToJson(rankingData);
    }

    [PunRPC]
    private void UpdateRanking(string serializedData)
    {
        // ��ŷ �����͸� ������ȭ�Ͽ� ���
        DeserializeRankingData(serializedData);

        // UI�� ��ŷ ������ ������Ʈ�ϴ� �ڵ�
        UpdateRankingUI();
    }

    private void DeserializeRankingData(string serializedData)
    {
        // JSON ���ڿ��� RankingData ����ü�� ������ȭ
        RankingData rankingData = JsonUtility.FromJson<RankingData>(serializedData);

        // ������ȭ�� ������ �Ҵ�
        _bestName = rankingData.names;
        _bestClearTime = rankingData.scores;
    }

    private void UpdateRankingUI()
    {
        // ��ŷ ����
        for (int i = 0; i < _bestName.Length; i++)
        {
            PlayerPrefs.SetString(i + "BestName", _bestName[i]);
            PlayerPrefs.SetFloat(i + "BestClearTime", _bestClearTime[i]);

            // ��ŷ �̸��� ������ UI�� ǥ��
            Debug.Log($"{_bestName[i]}: {_bestClearTime[i]}");
        }
        print("������Ʈ ��ŷ ��");
    }

    public void DestroyGameManager()
    {
        Destroy(gameObject);
    }
}
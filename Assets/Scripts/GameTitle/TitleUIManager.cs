using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    private enum TitlePanel
    {
        PlayButton = 3, RankButton, SelectPlayerPanel, SelectPlayerText
    }
    private enum RankingPanel
    {
        RankingButtonPanel = 1, RankingTextPanel, RankingCloseButton
    }

    private enum RankText
    {
        Number, Name, Record
    }

    private Transform _title;
    private Transform[] _titleUIs;
    private Transform[] _rankingUIs;
    private Transform[] _rankingUIsChildren;

    private GameObject[] _topTenRanks;

    private Button _playBtn;
    private Button _rankBtn;
    private Button _rankCloseBtn;

    private readonly float _oneMinute = 60.0f;

    int _rankCount = 10;

    private bool _isStart = false;

    private void OnEnable()
    {
        if (!_isStart)
        {
            _isStart = true;
            _title = GameObject.Find("TitlePanel").GetComponent<Transform>();
            SetTitlePanel();
            SetRankPanel();
            SetRankingRecord();
            SetInActivePanel();
        }
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(SoundKey.TitleBGM, 0.01f);

        _topTenRanks = new GameObject[_rankCount];

        _playBtn = _titleUIs[(int)TitlePanel.PlayButton].GetComponent<Button>();
        _rankBtn = _titleUIs[(int)TitlePanel.RankButton].GetComponent<Button>();
        _rankCloseBtn = _rankingUIsChildren[(int)RankingPanel.RankingCloseButton].GetComponent<Button>();

        _playBtn.onClick.AddListener(() => OnClickPlayButton());
        _rankBtn.onClick.AddListener(() => OnClickRankButton());
        _rankCloseBtn.onClick.AddListener(() => OnClickRankCloseButton());

        LoadRankData();
    }

    private void LoadRankData()
    {
        Transform rankParent = _rankingUIsChildren[(int)RankingPanel.RankingTextPanel];

        GameObject textPrefab = Resources.Load<GameObject>("Prefabs/RankingInfo");

        for (int i = 0; i < _rankCount; i++)
        {
            int rankId = i + 1;
            string nameKey = i + "BestName";
            string timeKey = i + "BestClearTime";

            string name = PlayerPrefs.GetString(nameKey);

            if (string.IsNullOrEmpty(name))
            {
                name = "----";
            }

            string displayTime;

            float clearTime = PlayerPrefs.GetFloat(timeKey);

            if (clearTime >= float.MaxValue || clearTime == 0.0f)
            {
                displayTime = "-- : --";
            }
            else
            {
                float minute = Mathf.FloorToInt(clearTime / _oneMinute);
                float second = Mathf.FloorToInt(clearTime % _oneMinute);
                displayTime = $"{minute:00} : {second:00}";
            }

            GameObject instance = Instantiate(textPrefab, rankParent);
            _topTenRanks[i] = instance;
            TextMeshProUGUI[] text = instance.GetComponentsInChildren<TextMeshProUGUI>();

            text[(int)RankText.Number].text = rankId.ToString();
            text[(int)RankText.Name].text = name;
            text[(int)RankText.Record].text = displayTime;
        }
    }

    private void CurrentRanking()
    {
        for (int i = 0; i < _rankCount; i++)
        {
            int rankId = i + 1;
            string nameKey = i + "BestName";
            string timeKey = i + "BestClearTime";

            string name = PlayerPrefs.GetString(nameKey);

            if (string.IsNullOrEmpty(name))
            {
                name = "----";
            }

            string displayTime;

            float clearTime = PlayerPrefs.GetFloat(timeKey);

            if (clearTime >= float.MaxValue || clearTime == 0.0f)
            {
                displayTime = "-- : --";
            }
            else
            {
                float minute = Mathf.FloorToInt(clearTime / _oneMinute);
                float second = Mathf.FloorToInt(clearTime % _oneMinute);
                displayTime = $"{minute:00} : {second:00}";
            }

            TextMeshProUGUI[] text = _topTenRanks[i].GetComponentsInChildren<TextMeshProUGUI>();

            text[(int)RankText.Number].text = rankId.ToString();
            text[(int)RankText.Name].text = name;
            text[(int)RankText.Record].text = displayTime;
        }
    }

        private void SetTitlePanel()
    {
        int panelChildrenCount = _title.childCount;

        _titleUIs = new Transform[panelChildrenCount];

        for (int i = 0; i < panelChildrenCount; i++)
        {
            _titleUIs[i] = _title.GetChild(i);
        }
    }

    private void SetRankPanel()
    {
        int rankBtnChildrenCount = _titleUIs[(int)TitlePanel.RankButton].childCount;

        _rankingUIs = new Transform[rankBtnChildrenCount];

        for (int i = 0; i < rankBtnChildrenCount; i++)
        {
            _rankingUIs[i] = _titleUIs[(int)TitlePanel.RankButton].GetChild(i);
        }
    }

    private void SetRankingRecord()
    {
        _rankingUIsChildren = _rankingUIs[(int)RankingPanel.RankingButtonPanel].GetComponentsInChildren<Transform>();
    }

    private void SetInActivePanel()
    {
        _titleUIs[(int)TitlePanel.SelectPlayerPanel].gameObject.SetActive(false);
        _titleUIs[(int)TitlePanel.SelectPlayerText].gameObject.SetActive(false);
        _rankingUIs[(int)RankingPanel.RankingButtonPanel].gameObject.SetActive(false);
    }

    private void OnClickPlayButton()
    {
        SoundManager.Instance.PlayFX(SoundKey.ButtonClickSound, 0.04f);
        _titleUIs[(int)TitlePanel.SelectPlayerPanel].gameObject.SetActive(true);
        _titleUIs[(int)TitlePanel.SelectPlayerText].gameObject.SetActive(true);
    }

    private void OnClickRankButton()
    {
        CurrentRanking();
        SoundManager.Instance.PlayFX(SoundKey.ButtonClickSound, 0.04f);
        _rankingUIs[(int)RankingPanel.RankingButtonPanel].gameObject.SetActive(true);
    }

    private void OnClickRankCloseButton()
    {
        SoundManager.Instance.PlayFX(SoundKey.ButtonClickSound, 0.04f);
        _rankingUIs[(int)RankingPanel.RankingButtonPanel].gameObject.SetActive(false);
    }
}

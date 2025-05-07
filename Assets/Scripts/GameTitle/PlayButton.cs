using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    private enum SelectPlayerUI
    {
        SelectPlayerPanel = 1, SelectPlayerText
    }

    private Button _playButton;
    private Transform[] _childrenTransforms;

    private int _playButtonChildCount;

    private void Start()
    {
        _playButton = GetComponent<Button>();
        _playButton.onClick.AddListener(() => OnSelectPlayerPanel());

        _playButtonChildCount = transform.childCount;

        _childrenTransforms = new Transform[_playButtonChildCount];

        for (int i = 0; i <  _playButtonChildCount; i++)
        {
            _childrenTransforms[i] = transform.GetChild(i);
        }

        // ó�� ���� �� ���� â�� ��Ȱ��ȭ
        for(int i = (int)SelectPlayerUI.SelectPlayerPanel; i < _playButtonChildCount; i++)
        {
            _childrenTransforms[i].gameObject.SetActive(false);
        }
    }

    // �÷��� ��ư�� ������ ���� â�� ��
    private void OnSelectPlayerPanel()
    {
        SoundManager.Instance.PlayFX(SoundKey.ButtonClickSound, 0.04f);

        for (int i = (int)SelectPlayerUI.SelectPlayerPanel; i < _playButtonChildCount; i++)
        {
            _childrenTransforms[i].gameObject.SetActive(true);
        }
    }
}
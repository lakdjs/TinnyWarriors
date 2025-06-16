using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] private CanvasGroup _bodyAlphaGroup;
    [SerializeField] private RectTransform _body;
    [SerializeField] private Button _buttonLobby;
    [SerializeField] private Button _buttonBack;
    [SerializeField] private Image _antiClicker;

    private Vector2 _targetBodyPos;
    private Vector2 _startShift;

    private void Awake()
    {
        _targetBodyPos = _body.anchoredPosition;
        _startShift = new Vector2(_targetBodyPos.x, -Screen.height/2);
    }

    public void Show()
    {
       // _bodyAlphaGroup.DOFade(1, 0.5f);
       Sequence animation = DOTween.Sequence();

        animation.Append(_bodyAlphaGroup.DOFade(1, 1f).From(0)).Join(_antiClicker.DOFade(0.5f, 0.5f).From(0))
            .Join(_body.DOAnchorPos(_targetBodyPos, 1f).From(_startShift))
            .Append(_buttonLobby.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBounce)).Append(_buttonBack.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBounce));
    }
    public void Hide() 
    {
        Sequence animation = DOTween.Sequence();

        animation.Append(_bodyAlphaGroup.DOFade(0, 1f).From(1)).Join(_antiClicker.DOFade(0, 0.5f).From(0.5f))
            .Join(_body.DOAnchorPos(_startShift, 1f).From(_targetBodyPos));
    }
}

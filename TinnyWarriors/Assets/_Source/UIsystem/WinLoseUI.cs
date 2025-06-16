using HexSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup winingObj;
    [SerializeField] private CanvasGroup loseObj;
    [SerializeField] private UnitManager unitMgr;
    [SerializeField] private Button buttonLobbyWin;
    [SerializeField] private Button buttonRetry;
    [SerializeField] private Button buttonLobbyLose;

    private void Awake()
    {
        unitMgr.losingAction += OpenLosingObj;
        unitMgr.winingAction += OpenWiningObj;
    }

    private void OpenWiningObj()
    {
        unitMgr.winingAction -= OpenWiningObj;
        Sequence animation = DOTween.Sequence();
        animation.Append(winingObj.DOFade(1, 1f).From(0))
            .Append(buttonLobbyWin.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBounce));
        buttonLobbyWin.interactable = true;
       
    }

    private void OpenLosingObj()
    {
        unitMgr.losingAction -= OpenLosingObj;
        Sequence animation = DOTween.Sequence();
        animation.Append(loseObj.DOFade(1, 1f).From(0))
            .Append(buttonLobbyLose.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBounce)).Append(buttonRetry.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBounce));
        buttonLobbyLose.interactable = true;
        buttonRetry.interactable = true;
    }

    IEnumerator WaitingAbit()
    {
        yield return new WaitForSeconds(3);
    }
}

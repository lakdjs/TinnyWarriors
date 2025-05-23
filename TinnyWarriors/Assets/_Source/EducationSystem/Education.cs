using CameraSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Education : MonoBehaviour
{
    public Action onPKMtapped;
    public Action onPlayerTargeted;
    public Action onPathTargeted;
    public Action onMovedToEnemy;
    public Action onEnemyDied;

    [SerializeField] private GameObject firstMessage;
    [SerializeField] private GameObject choosePlayerMessage;
    [SerializeField] private GameObject choosePathMessage;
    [SerializeField] private GameObject moveMessage;
    [SerializeField] private GameObject attackMessage;
    [SerializeField] private GameObject congratsMessage;
    [SerializeField] private GameObject finishButton;

    [SerializeField] private CameraRotation cameraRot;

    private void Awake()
    {
        firstMessage.gameObject.SetActive(true);
        onPKMtapped += ChoosePlayer;
        onPlayerTargeted += ChoosePath;
        onPathTargeted += MoveMessage;
        onMovedToEnemy += AttackMessage;
        onEnemyDied += CongratsMessage;
    }

    private void ChoosePlayer()
    {
        cameraRot.DeleteEducation();
        firstMessage.SetActive(false);
        choosePlayerMessage.SetActive(true);
        
    }
    private void ChoosePath()
    {
        cameraRot.DeleteEducation();
        firstMessage.SetActive(false);
        choosePlayerMessage.SetActive(false);
        choosePathMessage.SetActive(true);
    }
    private void MoveMessage()
    { 
        onPathTargeted -= MoveMessage;
        choosePathMessage.SetActive(false);
        moveMessage.SetActive(true);
    }
    private void AttackMessage()
    {
        moveMessage.SetActive(false);
        attackMessage.SetActive(true);
    }
    private void CongratsMessage()
    {
        attackMessage.SetActive(false);
        congratsMessage.SetActive(true);
        finishButton.SetActive(true);
    }
}

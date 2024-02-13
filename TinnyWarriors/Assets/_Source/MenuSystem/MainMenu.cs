using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MenuSystem
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button loadButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private int levelIndex;

        private void Awake()
        {
            loadButton.onClick.AddListener(LoadLevel);
            exitButton.onClick.AddListener(ExitGame);
        }

        private void OnDestroy()
        {
            loadButton.onClick.RemoveAllListeners(); 
            exitButton.onClick.RemoveAllListeners();
        }

        private void LoadLevel()
        {
            SceneManager.LoadScene(levelIndex);
        }
  
        private void ExitGame()
        {
            Application.Quit();
        }
    }
}

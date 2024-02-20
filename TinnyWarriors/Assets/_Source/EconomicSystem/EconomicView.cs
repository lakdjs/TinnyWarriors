using TMPro;
using UnityEngine;

namespace EconomicSystem
{
    public class EconomicView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private Economic _score;

        public void Construct(Economic score)
        {
            _score = score;
        }

        private void Start()
        {
            _score.OnScoreChange += RefreshScoreText;
            scoreText.text = $"score: {_score.ScoreValue}";
        }

        private void RefreshScoreText(int curScore)
        {
            scoreText.text = $"score: {curScore}";
        }

        private void OnDisable()
        {
            _score.OnScoreChange -= RefreshScoreText;
        }
    }
}

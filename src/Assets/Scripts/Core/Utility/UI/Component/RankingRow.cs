using TMPro;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class RankingRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;

        public void SetRank(int rank)
        {
            rankText.text = rank.ToString();
        }

        public void SetRef(string username, int score)
        {
            nameText.text = username == "" ? "Unknown" : username;
            scoreText.text = score.ToString();
        }
    }
}
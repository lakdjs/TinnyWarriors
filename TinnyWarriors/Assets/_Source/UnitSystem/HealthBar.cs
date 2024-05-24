using UnityEngine;
using UnityEngine.UI;

namespace UnitSystem
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;

        public void UpdateBar(float fraction)
        {
            healthBar.fillAmount = fraction;
        }
    }
}

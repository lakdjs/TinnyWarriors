using UnityEngine;
using UnityEngine.UI;

namespace EconomicSystem
{
    public class AddEconomic : MonoBehaviour
    {
        [SerializeField] private int value;
        [SerializeField] private Button addButton;
        [SerializeField] private Button reduceButton;
        private Economic _economic;

        public void Construct(Economic economic)
        {
            _economic = economic;
            addButton.onClick.AddListener(AddValue);
            reduceButton.onClick.AddListener(ReduceValue);
        }

        public void AddValue()
        {
            _economic.AddScore(value);
        }

        public void ReduceValue()
        {
            _economic.AddScore(-value);
        }
    }
}

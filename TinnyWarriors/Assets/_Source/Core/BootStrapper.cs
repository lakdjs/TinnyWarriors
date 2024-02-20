using System;
using EconomicSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class BootStrapper : MonoBehaviour
    {
        private Economic _economic; 
        [SerializeField] private EconomicView economicView;
        [SerializeField] private AddEconomic addEconomic;


        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _economic = new Economic();
            economicView.Construct(_economic);
            addEconomic.Construct(_economic);
        }
    }
}

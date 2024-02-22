using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ViewHandler : MonoBehaviour
    {
        [SerializeField] private MerchantBrains _merchantBrains;

        [Header("Buttons")] 
        [SerializeField] private Button tradeButton;
        [SerializeField] private Button leaderActivateBoardButton;
        [SerializeField] private Button updateLeaderBoard;

        [Header("LeaderBoard Links")] 
        [SerializeField] private GameObject leaderBoard;
        [SerializeField] private TextMeshProUGUI tradeYearsText;
        [SerializeField] private TextMeshProUGUI[] nameMerchants;
        [SerializeField] private TextMeshProUGUI[] tacticsMerchants;

        private const string Altruist = "altruist";
        private const string Threw = "threw";
        private const string Cunning = "cunning";
        private const string Unpredictable = "unpredictable";
        private const string Vindictive = "vindictive";
        private const string Quirky = "quirky";

        
        private List<MerchantData> _topList = new List<MerchantData>();
        private int _tradeYears;

        private void Awake()
        {
            tradeButton.onClick.AddListener(_merchantBrains.Trade);
            leaderActivateBoardButton.onClick.AddListener( () =>leaderBoard.gameObject.SetActive(true));
            updateLeaderBoard.onClick.AddListener(UpdateLeaderBoard);
            
        }

        private void UpdateLeaderBoard()
        {
            _topList = _merchantBrains.GetSuccessfulMerchants();
            _tradeYears = _merchantBrains.tradeYears;
            tradeYearsText.text = _tradeYears.ToString();

            if (_tradeYears > 0)
            {
                for (int i = 0; i < nameMerchants.Length; i++)
                {
                    nameMerchants[i].text = _topList[i].unitIndex.ToString();
                    tacticsMerchants[i].text = DecipherBehavior(_topList[i].behaviorIndex);
                }
            }
        }

        private string DecipherBehavior(int indexBehaviour)
        {
            switch (indexBehaviour)
            {
                case 0:
                    return Altruist;
                case 1:
                    return Threw;
                case 2:
                    return Cunning;
                case 3:
                    return Unpredictable;
                case 4:
                    return Vindictive;
                case 5:
                    return Quirky;
            }

            return "error to decipher";
        }
        
    }
}
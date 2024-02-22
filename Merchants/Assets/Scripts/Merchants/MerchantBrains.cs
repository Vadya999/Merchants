using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MerchantBrains : MonoBehaviour
{
    [HideInInspector] public int tradeYears;
    
    [Header("Merchant Trade Settings")] 
    [Range(5, 10)] 
    [SerializeField] private int tradeCountForOneYear;
    [Range(0,100)]
    [SerializeField] private int percentToReplaceUnsuccessfulTraders;
    
    [SerializeField] private int rewardAllForHonestTrade;
    [SerializeField] private int rewardAllForThrewTrade;
    [SerializeField] private int rewardAllForRepeatAndRepeatTrade;
    [SerializeField] private int rewardHonestForHonestAndThrewTrade;
    [SerializeField] private int rewardThrewForHonestAndThrewTrade;

    [Header("Links")] [SerializeField] private MerchantFactory merchantFactory;

    private List<MerchantData> _merchantDataList;
    private bool _isTradingInProgress;

    public void Trade()
    {
        if(!_isTradingInProgress) StartCoroutine(TradeOneYear(tradeCountForOneYear));
    }

    public List<MerchantData> GetSuccessfulMerchants()
    {
        List<MerchantData> topList = _merchantDataList.OrderByDescending(merchantData => merchantData.balance).ToList();
        return topList;
    }
    
    private void Awake()
    {
        _merchantDataList = merchantFactory.MakeStartMerchant();
    }

    private void ReplaceUnsuccessfulTraders(int percentileCont)
    {
        int countReplace = (int)Math.Floor(_merchantDataList.Count * (double)percentileCont / 100);
        List<int> namesForNewMerchants = new List<int>(countReplace);
        
        _merchantDataList = _merchantDataList.OrderByDescending(merchantData => merchantData.balance).ToList();

        for (int i = 0; i < countReplace && _merchantDataList.Count > 0; i++)
        {
            namesForNewMerchants.Add(_merchantDataList[_merchantDataList.Count - 1].unitIndex);
            _merchantDataList.RemoveAt(_merchantDataList.Count - 1);
        }

        for (int i = 0; i < countReplace; i++)
        {
            merchantFactory.AddMerchantForReplace(_merchantDataList,_merchantDataList[i].behaviorIndex, namesForNewMerchants[i]);
        }

    }

    private IEnumerator TradeOneYear(int amountTrade)
    {
        _isTradingInProgress = true;
        tradeYears++;

        for (int i = 0; i < amountTrade; i++)
        {
            TradeAllMerchantsOnceWithEachOther();
            yield return new WaitForSeconds(0.1f);
        }

        ReplaceUnsuccessfulTraders(percentToReplaceUnsuccessfulTraders);
        _isTradingInProgress = false;
    }

    private void TradeAllMerchantsOnceWithEachOther()
    {
        for (int i = 0; i < _merchantDataList.Count; i++)
        {
            TradeCurrentMerchantOnceWithOther(_merchantDataList[i], i);
        }
    }


    private void TradeCurrentMerchantOnceWithOther(MerchantData firstMerchant, int indexCurrentMerchant)
    {
        MerchantData firstMerchantData = firstMerchant;
        int dealIndexFirstMerchant = firstMerchant.GetCurrentDealBehavior();

        if (indexCurrentMerchant + 1 > _merchantDataList.Count) return;


        for (int i = indexCurrentMerchant + 1; i < _merchantDataList.Count; i++)
        {
            int dealIndexOtherMerchant = _merchantDataList[i].GetCurrentDealBehavior();
            MerchantData otherMerchantData = _merchantDataList[i];

            if (dealIndexFirstMerchant == 0 & dealIndexOtherMerchant == 0) // honest deal
            {
                HonestAndHonestDeal(firstMerchantData, otherMerchantData, true);
            }

            else if (dealIndexFirstMerchant == 0 & dealIndexOtherMerchant == 1 |
                     dealIndexFirstMerchant == 1 & dealIndexOtherMerchant == 0) // threw and honest or honest and threw
            {
                if (dealIndexFirstMerchant == 0)
                {
                    HonestAndThrewDeal(firstMerchantData, otherMerchantData, true);
                }
                else
                {
                    HonestAndThrewDeal(otherMerchantData, firstMerchantData, true);
                }
            }

            else if (dealIndexFirstMerchant == 1 & dealIndexOtherMerchant == 1) // threw and threw
            {
                ThrewAndThrewDeal(firstMerchantData, otherMerchantData, true);
            }

            else if (dealIndexFirstMerchant == 6 & dealIndexOtherMerchant == 6) // reaped and repeat
            {
                RepeatAndRepeatDeal(firstMerchantData, otherMerchantData);
            }

            else if (dealIndexFirstMerchant == 0 & dealIndexOtherMerchant == 6 |
                     dealIndexFirstMerchant == 6 &
                     dealIndexOtherMerchant == 0) // honest and repeat or repeat and honest
            {
                HonestAndHonestDeal(firstMerchantData, otherMerchantData, true);
            }

            else if (dealIndexFirstMerchant == 1 & dealIndexOtherMerchant == 6 |
                     dealIndexFirstMerchant == 6 & dealIndexOtherMerchant == 1) // threw and repeat or repeat and threw
            {
                ThrewAndThrewDeal(firstMerchantData, otherMerchantData, true);
            }

            else if (dealIndexFirstMerchant == 7 | dealIndexOtherMerchant == 7)
            {
                Debug.Log("error of deal");
            }
        }
    }

    private void HonestAndHonestDeal(MerchantData firstMerchant, MerchantData secondMerchant, bool isNeedCheckMistake)
    {
        //all +4
        if (isNeedCheckMistake)
        {
            bool isFirstMerchantWrong = CheckProbably(firstMerchant.probablyGetMistake);
            bool isSecondMerchantWrong = CheckProbably(secondMerchant.probablyGetMistake);

            if (isSecondMerchantWrong & isFirstMerchantWrong)
            {
                ThrewAndThrewDeal(firstMerchant, secondMerchant, false);
                return;
            }

            if (isFirstMerchantWrong)
            {
                HonestAndThrewDeal(secondMerchant, firstMerchant, false);
                return;
            }

            if (isSecondMerchantWrong)
            {
                HonestAndThrewDeal(firstMerchant, secondMerchant, false);
                return;
            }
        }

        firstMerchant.balance += rewardAllForHonestTrade;
        secondMerchant.balance += rewardAllForHonestTrade;

    }

    private void HonestAndThrewDeal(MerchantData firstMerchant, MerchantData secondMerchant, bool isNeedCheckMistake)
    {
        // honest and threw trade, honest +1 threw +5 

        if (isNeedCheckMistake)
        {
            bool isFirstMerchantWrong = CheckProbably(firstMerchant.probablyGetMistake);
            bool isSecondMerchantWrong = CheckProbably(secondMerchant.probablyGetMistake);

            if (isSecondMerchantWrong & isFirstMerchantWrong)
            {
                HonestAndThrewDeal(secondMerchant, firstMerchant, false);
                return;
            }

            if (isFirstMerchantWrong)
            {
                ThrewAndThrewDeal(firstMerchant, secondMerchant, false);
                return;
            }

            if (isSecondMerchantWrong)
            {
                HonestAndHonestDeal(firstMerchant, secondMerchant, false);
                return;
            }
        }

        firstMerchant.balance += rewardHonestForHonestAndThrewTrade;
        secondMerchant.balance += rewardThrewForHonestAndThrewTrade;

    }

    private void ThrewAndThrewDeal(MerchantData firstMerchant, MerchantData secondMerchant, bool isNeedCheckMistake)
    {
        // threw and threw trade, all +2

        if (isNeedCheckMistake)
        {
            bool isFirstMerchantWrong = CheckProbably(firstMerchant.probablyGetMistake);
            bool isSecondMerchantWrong = CheckProbably(secondMerchant.probablyGetMistake);

            if (isSecondMerchantWrong & isFirstMerchantWrong)
            {
                HonestAndHonestDeal(firstMerchant, secondMerchant, false);
                return;
            }

            if (isFirstMerchantWrong)
            {
                HonestAndThrewDeal(firstMerchant, secondMerchant, false);
                return;
            }

            if (isSecondMerchantWrong)
            {
                HonestAndThrewDeal(secondMerchant, firstMerchant, false);
                return;
            }
        }

        firstMerchant.balance += rewardAllForThrewTrade;
        secondMerchant.balance += rewardAllForThrewTrade;

    }

    private void RepeatAndRepeatDeal(MerchantData firstMerchant, MerchantData secondMerchant)
    {
        firstMerchant.balance += rewardAllForRepeatAndRepeatTrade;
        secondMerchant.balance += rewardAllForRepeatAndRepeatTrade;

    }

    private bool CheckProbably(int chance)
    {
        return Random.Range(0, 100) <= chance;
    }
}
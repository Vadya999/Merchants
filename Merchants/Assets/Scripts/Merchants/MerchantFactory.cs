using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MerchantFactory : MonoBehaviour
{
    [Header("Merchant Factory Settings")] 
    [SerializeField] private int merchantCount;

    [Range(0,100)]
    [SerializeField] private int percentProbablyGetMistake;

    [SerializeField] private int altruistBehaviorCount;
    [SerializeField] private int threwBehaviorCount;
    [SerializeField] private int cunningBehaviorCount;
    [SerializeField] private int unpredictableBehaviorCount;
    [SerializeField] private int vindictiveBehaviorCount;
    [SerializeField] private int quirkyBehaviorCount;

    private int nameMerchant = 0;

    public void AddMerchantForReplace(List<MerchantData> list, int behaviourIndex, int name)
    {
        MerchantData newMerchant = new MerchantData(name, behaviourIndex, percentProbablyGetMistake);
        list.Add(newMerchant);
    }

    public List<MerchantData> MakeStartMerchant()
    {
        List<MerchantData> merchantsList = new List<MerchantData>(merchantCount);

        AddStartMerchant(altruistBehaviorCount, merchantsList, 0);

        AddStartMerchant(threwBehaviorCount, merchantsList, 1);

        AddStartMerchant(cunningBehaviorCount, merchantsList, 2);

        AddStartMerchant(unpredictableBehaviorCount, merchantsList, 3);

        AddStartMerchant(vindictiveBehaviorCount, merchantsList, 4);

        AddStartMerchant(quirkyBehaviorCount, merchantsList, 5);

        return merchantsList;
    }

    private void AddStartMerchant(int spawnCount, List<MerchantData> merchantsList, int behaviour)
    {
        
        for (int i = 0; i < spawnCount; i++)
        {
            MerchantData currentUnit = new MerchantData(nameMerchant, behaviour, percentProbablyGetMistake);
            merchantsList.Add(currentUnit);
            nameMerchant++;
        }
    }
}
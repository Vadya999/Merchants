using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class MerchantData
{
    public int unitIndex;
    public int behaviorIndex;
    public int balance;
    public bool isWasDeseived;
    public int probablyGetMistake;

    private int _amountOfDeal = 0;
    private bool _alwaysPlayAsThrew;
    private bool _alwaysPlayAsCunning;


    public MerchantData(int index, int behavior, int probablyMistake)
    {
        unitIndex = index;
        behaviorIndex = behavior;
        probablyGetMistake = probablyMistake;
    }

    public int GetCurrentDealBehavior() // 0 - honestDeal; 1 - threwDeal; 6 - copy opponent behavior; 7 - error calculate
    {
        int dealIndex = 0;

        if (behaviorIndex == 0) //altruist
        {
            dealIndex = 0;
            _amountOfDeal++;
            return dealIndex;
        }

        if (behaviorIndex == 1) //threw 
        {
            dealIndex = 1;
            _amountOfDeal++;
            return dealIndex;
        }

        if (behaviorIndex == 2) //cunning
        {
            return CalculateCunningDealBehavior();
        }

        if (behaviorIndex == 3) //unpredictable 
        {
            dealIndex = Random.Range(0, 1);
            _amountOfDeal++;
            return dealIndex;
        }

        if (behaviorIndex == 4) //vindictive
        {
            return CalculateVindictiveBehavior();
        }

        if (behaviorIndex == 5) // quirky
        {
            return CalculateQuirkyBehavior();
        }

        return 7;
    }

    private int CalculateCunningDealBehavior()
    {
        if (_amountOfDeal == 0)
        {
            _amountOfDeal++;
            return 0;
        }

        if (_amountOfDeal > 0)
        {
            _amountOfDeal++;
            return 6;
        }

        _amountOfDeal++;
        return 7;
    }

    private int CalculateVindictiveBehavior()
    {
        if (_amountOfDeal == 0)
        {
            _amountOfDeal++;
            return 0;
        }

        if (isWasDeseived)
        {
            _amountOfDeal++;
            return 1;
        }

        if (!isWasDeseived)
        {
            _amountOfDeal++;
            return 0;
        }
        

        _amountOfDeal++;
        return 7;
    }

    private int CalculateQuirkyBehavior()
    {
        if (_alwaysPlayAsThrew)
        {
            _amountOfDeal++;
            return 1;
        }

        if (_alwaysPlayAsCunning)
        {
            return CalculateCunningDealBehavior();
        }


        if (_amountOfDeal == 0 | _amountOfDeal == 2 | _amountOfDeal == 3)
        {
            _amountOfDeal++;
            return 0;
        }

        if (_amountOfDeal == 1)
        {
            _amountOfDeal++;
            return 1;
        }

        if (_amountOfDeal >= 4)
        {
            if (isWasDeseived)
            {
                _amountOfDeal++;
                _alwaysPlayAsThrew = true;
                return 1;
            }

            _alwaysPlayAsCunning = true;
            return CalculateCunningDealBehavior();
        }

        _amountOfDeal++;
        return 7;
    }
}
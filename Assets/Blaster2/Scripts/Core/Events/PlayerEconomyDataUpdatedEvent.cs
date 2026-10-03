using UnityEngine;

public class PlayerEconomyDataUpdatedEvent 
{
        public enum StatType
        {
            Coins,
            CoinsSpend,
            CoinsPicked
        }

        public StatType type;
        public float value;
}

namespace AdvancedDealing.Persistence.Datas
{
    public class DealerData : DataBase
    {
        /// <summary>
        /// Legacy field used by AdvancedDealing 1.4.1 and earlier.
        /// Kept only so existing save files and older multiplayer payloads can be migrated.
        /// New saves write this field as null.
        /// </summary>
        public string DeadDrop;

        public string ProductDeadDrop;

        public string CashDeadDrop;

        public int MaxCustomers;

        public int ItemSlots;

        public float Cut;

        public float SpeedMultiplier;

        public bool DeliverCash;

        public bool PickupProducts;

        public float CashThreshold;

        public int ProductThreshold;

        public int DaysUntilNextNegotiation;

        public DealerData(string identifier, bool loadDefaults = false) : base(identifier)
        {
            if (loadDefaults)
            {
                LoadDefaults();
            }
        }

        public void LoadDefaults()
        {
            DeadDrop = null;
            ProductDeadDrop = null;
            CashDeadDrop = null;
            MaxCustomers = 8;
            ItemSlots = 5;
            Cut = 0.2f;
            SpeedMultiplier = 1f;
            DeliverCash = false;
            PickupProducts = false;
            CashThreshold = 1500f;
            ProductThreshold = 20;
            DaysUntilNextNegotiation = 0;
        }

        /// <summary>
        /// Copies the single legacy dead-drop assignment into both new assignments.
        /// This method is intentionally safe to call more than once.
        /// </summary>
        public bool MigrateLegacyDeadDrop()
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(DeadDrop))
            {
                if (string.IsNullOrWhiteSpace(ProductDeadDrop))
                {
                    ProductDeadDrop = DeadDrop;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(CashDeadDrop))
                {
                    CashDeadDrop = DeadDrop;
                    changed = true;
                }

                // Do not keep writing the deprecated field into new saves.
                DeadDrop = null;
                changed = true;
            }

            if (ModVersion != ModInfo.VERSION)
            {
                ModVersion = ModInfo.VERSION;
                changed = true;
            }

            return changed;
        }
    }
}

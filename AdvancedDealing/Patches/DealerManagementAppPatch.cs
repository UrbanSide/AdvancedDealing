using AdvancedDealing.Economy;
using Loc = AdvancedDealing.Localization.LocalizationManager;
using AdvancedDealing.Persistence;
using AdvancedDealing.UI;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.UI.Phone.Messages;
#elif MONO
using ScheduleOne.Economy;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.Patches
{
    [HarmonyPatch(typeof(DealerManagementApp))]
    public class DealerManagementAppPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetDisplayedDealer")]
        public static void SetDisplayedDealerPostfix(DealerManagementApp __instance, Dealer dealer)
        {
            if (!SaveModifier.Instance.SavegameLoaded || !UIBuilder.HasBuild)
            {
                return;
            }

            DealerExtension dealerExtension = DealerExtension.GetDealer(dealer);
            if (dealerExtension == null)
            {
                return;
            }

            UIBuilder.ProductDeadDropSelector.ButtonLabel.text = ResolveDeadDropName(
                dealerExtension.ProductDeadDrop,
                UIBuilder.ProductDeadDropSelector.EmptySelectionLabel);

            UIBuilder.CashDeadDropSelector.ButtonLabel.text = ResolveDeadDropName(
                dealerExtension.CashDeadDrop,
                UIBuilder.CashDeadDropSelector.EmptySelectionLabel);

            UIBuilder.CustomersScrollView.TitleLabel.text = Loc.Get(
                "ui.customers.assigned_title",
                dealerExtension.Dealer.AssignedCustomers.Count,
                dealerExtension.MaxCustomers);

            UIBuilder.CustomersScrollView.AssignButton.SetActive(
                dealerExtension.Dealer.AssignedCustomers.Count < dealerExtension.MaxCustomers);
        }

        private static string ResolveDeadDropName(string guid, string fallback)
        {
            if (string.IsNullOrWhiteSpace(guid))
            {
                return fallback;
            }

            DeadDropExtension deadDrop = DeadDropExtension.GetDeadDrop(guid);
            return deadDrop?.DeadDrop?.DeadDropName ?? fallback;
        }
    }
}

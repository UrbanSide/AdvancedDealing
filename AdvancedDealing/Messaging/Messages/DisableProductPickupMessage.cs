using AdvancedDealing.Economy;
using Loc = AdvancedDealing.Localization.LocalizationManager;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    internal class DisableProductPickupMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => Loc.Get("messages.products.disable.option");

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited && _dealer.PickupProducts)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            _dealer.PickupProducts = false;

            _dealer.SendPlayerMessage(Loc.Get("messages.products.disable.player"));
            _dealer.SendMessage(Loc.Get("messages.products.disable.dealer"), false, true, 2f);
        }
    }
}

using AdvancedDealing.Economy;
using Loc = AdvancedDealing.Localization.LocalizationManager;
using AdvancedDealing.UI;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class EnableProductPickupMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => Loc.Get("messages.products.enable.option");

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited && !_dealer.PickupProducts)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            UIBuilder.SliderPopup.Open(Loc.Get("ui.slider.product_threshold_title", _dealer.Dealer.name), null, _dealer.ProductThreshold, 0f, 1000f, 10f, 0, OnSend, null, Loc.Get("formats.products"));
        }

        private void OnSend(float value)
        {
            _dealer.PickupProducts = true;
            _dealer.ProductThreshold = (int)value;

            _dealer.SendPlayerMessage(Loc.Get("messages.products.enable.player", value));
            _dealer.SendMessage(Loc.Get("messages.products.enable.dealer"), false, true, 2f);
        }
    }
}

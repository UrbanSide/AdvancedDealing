using AdvancedDealing.Economy;
using Loc = AdvancedDealing.Localization.LocalizationManager;
using AdvancedDealing.UI;
using System;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class EnableDeliverCashMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => Loc.Get("messages.cash.enable.option");

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited &&
                !_dealer.DeliverCash &&
                !string.IsNullOrWhiteSpace(_dealer.CashDeadDrop))
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            UIBuilder.SliderPopup.Open(Loc.Get("ui.slider.cash_threshold_title", _dealer.Dealer.name), null, _dealer.CashThreshold, 100f, 10000f, 50f, 0, OnSend, null, Loc.Get("formats.cash"));
        }

        private void OnSend(float value)
        {
            _dealer.DeliverCash = true;
            _dealer.CashThreshold = value;

            _dealer.SendPlayerMessage(Loc.Get("messages.cash.enable.player", value));
            _dealer.SendMessage(Loc.Get("messages.cash.enable.dealer"), false, true, 2f);
        }
    }
}

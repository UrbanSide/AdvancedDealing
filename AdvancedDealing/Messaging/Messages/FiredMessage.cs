using AdvancedDealing.Economy;
using Loc = AdvancedDealing.Localization.LocalizationManager;
using System;

#if IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.UI.Phone.Messages;
#elif MONO
using ScheduleOne.DevUtilities;
using ScheduleOne.Messaging;
using ScheduleOne.UI.Phone.Messages;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class FiredMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => Loc.Get("messages.fire.option");

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            PlayerSingleton<MessagesApp>.Instance.ConfirmationPopup.Open(Loc.Get("messages.fire.confirm_title"), Loc.Get("messages.fire.confirm_body"), S1Conversation, new Action<ConfirmationPopup.EResponse>(OnConfirmationResponse));
        }

        private void OnConfirmationResponse(ConfirmationPopup.EResponse response)
        {
            if (response == ConfirmationPopup.EResponse.Confirm)
            {
                _dealer.Fire();
                _dealer.SendPlayerMessage(Loc.Get("messages.fire.player"));
                _dealer.SendMessage(Loc.Get("messages.fire.dealer"), false, true, 0.5f);
            }
        }
    }
}

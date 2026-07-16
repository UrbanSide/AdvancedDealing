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
    public class AdjustSettingsMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => Loc.Get("messages.adjust_settings.option");

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited && ModConfig.SettingsMenu)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            UIBuilder.SettingsPopup.Open(_dealer);
        }
    }
}

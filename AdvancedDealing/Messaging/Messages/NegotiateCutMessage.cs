using AdvancedDealing.Economy;
using Loc = AdvancedDealing.Localization.LocalizationManager;
using AdvancedDealing.UI;
using System;
using AdvancedDealing.Persistence;

#if IL2CPP
using Il2CppScheduleOne.Messaging;
#elif MONO
using ScheduleOne.Messaging;
#endif

namespace AdvancedDealing.Messaging.Messages
{
    public class NegotiateCutMessage(DealerExtension dealerExtension) : MessageBase
    {
        private readonly DealerExtension _dealer = dealerExtension;

        public override string Text => Loc.Get("messages.negotiation.option");

        public override bool DisableDefaultSendBehaviour => true;

        public override bool ShouldShowCheck(SendableMessage sMsg)
        {
            if (_dealer.Dealer.IsRecruited && _dealer.DaysUntilNextNegotiation <= 0)
            {
                return true;
            }
            return false;
        }

        public override void OnSelected()
        {
            float current = (float)Math.Round(_dealer.Cut, 2);

            UIBuilder.SliderPopup.Open(Loc.Get("ui.slider.negotiation_title", _dealer.Dealer.name), Loc.Get("ui.slider.current", current), current, 0f, 1f, 0.01f, 2, OnSend, null, Loc.Get("formats.percent"));
        }

        private void OnSend(float value)
        {
            _dealer.SendPlayerMessage(Loc.Get("messages.negotiation.player_offer", value));

            if (value == _dealer.Cut)
            {
                _dealer.SendMessage(Loc.Get("messages.negotiation.same"), false, true, 2f);

                return;
            }
            else if (value > _dealer.Cut)
            {
                _dealer.SendMessage(Loc.Get("messages.negotiation.higher"), false, true, 2f);
            }
            else
            {
                bool accepted = CalculateResponse(_dealer.Cut, value);

                if (accepted)
                {
                    _dealer.SendMessage(Loc.Get("messages.negotiation.accepted"), false, true, 2f);
                }
                else
                {
                    _dealer.SendMessage(Loc.Get("messages.negotiation.rejected"), false, true, 2f);

                    _dealer.DaysUntilNextNegotiation = 3;

                    if (NetworkSynchronizer.IsSyncing)
                    {
                        NetworkSynchronizer.Instance.SendData(_dealer.FetchData());
                    }

                    return;
                }
            }

            _dealer.Cut = value;
            _dealer.DaysUntilNextNegotiation = 7;
            _dealer.HasChanged = true;

            if (NetworkSynchronizer.IsSyncing)
            {
                NetworkSynchronizer.Instance.SendData(_dealer.FetchData());
            }
        }

        private static bool CalculateResponse(float oldCut, float newCut)
        {
            float baseChance = ModConfig.NegotiationModifier;
            float chance = baseChance - (1f - (newCut * 100f / oldCut / 100f));

            return UnityEngine.Random.value <= chance;
        }
    }
}

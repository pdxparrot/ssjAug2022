using Godot;

using pdxpartyparrot.ssjAug2022.Collections;
using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public class NPCMessageDispatcher
    {
        private readonly PriorityQueue<Telegram> _messages = new PriorityQueue<Telegram>();

        public void DispatchDelayedMessages()
        {
            ulong now = Time.GetTicksMsec();

            while(_messages.TryPeek(out Telegram telegram)) {
                if(telegram.DispatchTime > now) {
                    break;
                }

                Discharge(telegram);

                _messages.Dequeue();
            }
        }

        private void Discharge(Telegram message)
        {
            if(message.Receiver == null) {
                foreach(var npc in NPCManager.Instance.NPCs) {
                    if(npc != message.Sender) {
                        npc.HandleMessage(message);
                    }
                }
            } else {
                message.Receiver.HandleMessage(message);
            }
        }

        public void DispatchMessageImmediate(SimpleNPC sender, SimpleNPC receiver, int message, object extraInfo = null)
        {
            var telegram = new Telegram(0, sender, receiver, message, extraInfo);
            Discharge(telegram);
        }

        public void BroadcastMessageImmediate(SimpleNPC sender, int message, object extraInfo = null)
        {
            DispatchMessageImmediate(sender, null, message, extraInfo);
        }

        public void DispatchMessage(ulong delay, SimpleNPC sender, SimpleNPC receiver, int message, object extraInfo = null)
        {
            var telegram = new Telegram(delay, sender, receiver, message, extraInfo);

            if(delay <= 0) {
                Discharge(telegram);
            } else {
                _messages.Enqueue(telegram);
            }
        }

        public void BroadcastMessage(ulong delay, SimpleNPC sender, int message, object extraInfo = null)
        {
            DispatchMessage(delay, sender, null, message, extraInfo);
        }
    }
}

using Godot;

using pdxpartyparrot.ssjAug2022.Collections;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public class NPCMessageDispatcher : SingletonNode<NPCMessageDispatcher>
    {
        private readonly PriorityQueue<Telegram> _messages = new PriorityQueue<Telegram>();

        #region Godot Lifecycle

        public override void _Process(float delta)
        {
            base._Process(delta);

            // TODO: we probably don't want to do this every frame
            DispatchDelayedMessages();
        }

        #endregion

        private void DispatchDelayedMessages()
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
            message.Receiver.HandleMessage(message);
        }

        public void DispatchMessageImmediate(SimpleNPC sender, SimpleNPC receiver, int message, object extraInfo = null)
        {
            var telegram = new Telegram(0, sender, receiver, message, extraInfo);
            Discharge(telegram);
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
    }
}

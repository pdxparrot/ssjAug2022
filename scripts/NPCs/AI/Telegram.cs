using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public class Telegram : IComparable
    {
        public SimpleNPC Sender { get; private set; }

        public SimpleNPC Receiver { get; private set; }

        public int Message { get; private set; }

        public ulong DispatchTime { get; private set; }

        public object ExtraInfo { get; private set; }

        public Telegram(ulong delay, SimpleNPC sender, SimpleNPC receiver, int message, object extraInfo)
        {
            Sender = sender;
            Receiver = receiver;
            Message = message;
            DispatchTime = Time.GetTicksMsec() + delay;
            ExtraInfo = extraInfo;
        }

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj == null) {
                return 1;
            }

            if(!(obj is Telegram other)) {
                throw new ArgumentException("Object is not a Telegram");
            }

            return DispatchTime.CompareTo(other.DispatchTime);
        }

        #endregion
    }
}

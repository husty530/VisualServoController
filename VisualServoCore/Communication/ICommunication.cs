namespace VisualServoCore.Communication
{
    public interface ICommunication<TMessage>
    {

        public bool Send(TMessage sendmsg);

        public TMessage Receive();

        public void Dispose();

    }
}

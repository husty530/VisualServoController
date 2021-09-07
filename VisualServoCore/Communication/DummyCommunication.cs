namespace VisualServoCore.Communication
{
    public class DummyCommunication : ICommunication<string>
    {

        // ------ Fields ------ //

        private int _count;


        // ------ Constructors ------ //

        public DummyCommunication()
        {

        }


        // ------ Methods ------ //

        public bool Send(string sendmsg)
        {
            return true;
        }

        public string Receive()
        {
            return $"dummy [{_count++:d3}]";
        }

        public void Dispose()
        {

        }

    }
}

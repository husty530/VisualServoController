using Husty.TcpSocket;

namespace VisualServoCore.Communication
{
    public class SocketServer : ICommunication<string>
    {

        // ------ Fields ------ //

        private readonly Server _server;


        // ------ Constructors ------ //

        public SocketServer(string ip, int port)
        {
            _server = new(ip, port);
        }


        // ------ Methods ------ //

        public bool Send(string sendmsg)
        {
            try
            {
                _server.Send(sendmsg);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Receive()
        {
            return _server.Receive<string>();
        }

        public void Dispose()
        {

        }

    }
}

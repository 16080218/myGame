using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Reflection;

namespace ConsoleApp3
{
    public class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
        public int hp = -100;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float eulY = 0;

    }

    class MainClass
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        public static void Main(string[] args)
        {
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp);

            //listen
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");

            //checkread
            List<Socket> checkRead = new List<Socket>();
            //主循环
            while (true)
            {
                //填充checkread列表
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach (ClientState s in clients.Values)
                {
                    checkRead.Add(s.socket);
                }
                //select
                Socket.Select(checkRead, null, null, 1000);
                //检查可读对象
                foreach (Socket s in checkRead)
                {
                    if (s == listenfd)
                    {
                        ReadListenfd(s);
                    }
                    else
                    {
                        ReadClientfd(s);
                    }
                }


            }
  
        }

        public static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Accept!!");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }

        public static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            //接受
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("false!!" + ex);
                return false;
            }

            //客户端关闭
            if (count == 0)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Scoket Close");
                return false;
            }

            //广播
            string recvStr = Encoding.Default.GetString(state.readBuff, 0, count);
            string[] split = recvStr.Split('|');
            Console.WriteLine("Receive" + recvStr);
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
            object[] o = { state, msgArgs };
            mi.Invoke(null, o);
            return true;
        }
    }

}

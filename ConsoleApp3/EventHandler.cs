using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class EventHandler
    {
        public static void OnDisconnect(ClientState c)
        {
            string desc = c.socket.RemoteEndPoint.ToString();
            string sendStr = "Leave|" + desc + ",";
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach (ClientState cs in MainClass.clients.Values)
            {
                cs.socket.Send(sendBytes);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace ConsoleApp3
{
    class MsgHandler
    {
        public static void MsgEnter(ClientState c, string msgArgs)
        {
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);
            //赋值
            c.hp = 100;
            c.x = x;
            c.y = y;
            c.z = z;
            c.eulY = eulY;
            //广播
            string sendStr = "Enter|" + msgArgs;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach (ClientState cs in MainClass.clients.Values)
            {
                cs.socket.Send(sendBytes);
            }
            //test
        }

        public static void MsgList(ClientState c, string msgArgs)
        {
            string sendStr = "List|";
            foreach (ClientState cs in MainClass.clients.Values)
            {
                sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
                sendStr += cs.x.ToString() + ",";
                sendStr += cs.y.ToString() + ",";
                sendStr += cs.z.ToString() + ",";
                sendStr += cs.eulY.ToString() + ",";
                sendStr += cs.hp.ToString() + ",";
            }
            Console.WriteLine(sendStr);
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            c.socket.Send(sendBytes);
        }

        public static void MsgMove(ClientState c, string msgArgs)
        {
            //解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            //赋值
            c.x = x;
            c.y = y;
            c.z = z;
            //广播
            string sendStr = "Move|" + msgArgs;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach (ClientState cs in MainClass.clients.Values)
            {
                cs.socket.Send(sendBytes);
            }
        }

        public static void MsgAttack(ClientState c, string msgArgs)
        {
            string sendStr = "Attack|" + msgArgs;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach (ClientState cs in MainClass.clients.Values)
            {
                cs.socket.Send(sendBytes);
            }
        }

        public static void MsgHit(ClientState c, string msgArgs)
        {
            string[] split = msgArgs.Split(',');
            string hitDesc = split[0];
            string hurtDesc = split[1];

            ClientState hurtCs = null;
            foreach (ClientState cs in MainClass.clients.Values)
            {
                if (cs.socket.RemoteEndPoint.ToString() == hurtDesc)
                {
                    hurtCs = cs;
                    Console.WriteLine(111);
                }
            }

            if (hurtCs == null)
            {
                return;
            }

            hurtCs.hp -= 50;
            Console.WriteLine(hurtDesc + "  " + hurtCs.hp);
            //死亡
            if (hurtCs.hp <= 0)
            {
                string sendStr = "Die|" + hurtCs.socket.RemoteEndPoint.ToString();
                byte[] sendByte = System.Text.Encoding.Default.GetBytes(sendStr);
                foreach (ClientState cs in MainClass.clients.Values)
                {
                    cs.socket.Send(sendByte);
                }
            }

        }
    }
}
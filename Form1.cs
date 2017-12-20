using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using WeespinClub.Data;

namespace WeespinClub
{
    public partial class Form1 : Form
    {
        public static bool GetClubs = false;
        public static int accslog;
        static List<Utils.Proxyes> ProxyPool=new List<Utils.Proxyes>();
        public static Form Instance;
        static List<WeespinClub.Data.User> Accounts = new List<User>();
        public Form1()
        {
            InitializeComponent();
            Instance = this;
           
        }

     
        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var path = "./accs.txt";
            if (!File.Exists(path))
            {
              File.Create("./accs.txt");
            }
            var accs = Utils.GetAccountsFromFile("./accs.txt");
            foreach (var acc in accs)
            {                                 // ID 
                var namencommand = new string[] { acc.id.ToString(), "", "", "" };
                var items = new ListViewItem(namencommand);
                listView1.Items.Add(items);
                Accounts.Add(new User{auth_key = acc.auth_key,id=acc.id});
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var acc in Accounts)
            {
                Task.Run(() =>
                {
                    acc.Connect();
                });

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public static void ForceInit(Data.User account)
        {
            
                Task.Run(() =>
                {
                    try
                    {

                        ForceLoadProxy(account);
                        account.CreateConenction();
                        accslog++;
                        Debug.WriteLine(accslog+"/"+Accounts.Count);
                    }
                    catch (Exception)
                    {
                        ForceInit(account);

                    }
                });
            
        }
        private void button5_Click(object sender, EventArgs e)
        {
            foreach (var acc in Accounts)
            {

                Task.Run(() =>
                {
                    try
                    {


                        acc.CreateConenction();
                        accslog++;
                        Debug.WriteLine(accslog + "/" + Accounts.Count);
                    }
                    catch (Exception)
                    {
                        ForceLoadProxy(acc);

                    }
                });
                
            }
        }

        private void button6_Click(object sesnder, EventArgs e)
        {
            
                Accounts[0].GetClubs();
               GetClubs = true;
        }

       
        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            bool match = false;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Bounds.Contains(new Point(e.X, e.Y)))
                    {
                        MenuItem[] mi = new MenuItem[] { new MenuItem("Перекатить",Perekat),new MenuItem("Перекат к ID",RawPerekat), };
                        listView1.ContextMenu = new ContextMenu(mi);
                        match = true;
                        break;
                    }
                }
                if (match)
                {
                    
                    listView1.ContextMenu.Show(listView1, new Point(e.X, e.Y));
                  
                }
                else
                {
                  
                }

            }
        }
     
        private void RawPerekat(object sender, EventArgs e)
        {
            var b = Microsoft.VisualBasic.Interaction.InputBox("Куда?", "Введи номер группы", "Номер группы");
            foreach (ListViewItem acc in listView1.SelectedItems)
            {

                var vkid = acc.SubItems[0];
                var accc = Accounts.FirstOrDefault(n => n.id.ToString() == vkid.Text);
                if (acc == null)
                {

                }
                else
                {
                    if (b == "")
                    {
                        return;
                    }
                    accc.JoinClub(int.Parse(b));
                }
            }

        }

        private void Perekat(object sender, EventArgs e)
        {
          var b=  Microsoft.VisualBasic.Interaction.InputBox("Куда?", "Введи номер группы", "Номер группы");
            foreach (ListViewItem acc in listView1.SelectedItems)
            {

                var vkid= acc.SubItems[0];
                var accc=   Accounts.FirstOrDefault(n => n.id.ToString() == vkid.Text);
                if (acc == null)
                {

                }
                else
                {
                    var ss = Form1.listView2.Items.Cast<ListViewItem>()
                        .FirstOrDefault(x => x.SubItems[3].Text == b);
                    if (ss.SubItems[0].Text != null)
                    {
                        var clubs = ss.SubItems[0].Text;
                        accc.JoinClub(int.Parse(clubs));
                    }
                }
            }
      
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ProxyPool = Utils.LoadProxy("./proxy.dat");
            foreach (var account in Accounts)
            {
                Task.Run(() =>
                {
                    try
                    {


                        var currentprox = ProxyPool[new Random().Next(ProxyPool.Count)];
                        account.proxy = new Proxy(currentprox.IP, ushort.Parse(currentprox.port));
                        ProxyPool.Remove(currentprox);
                    }
                    catch (Exception)
                    {
                        ForceLoadProxy(account);
                    }
                });
            }
        }
        private static void ForceLoadProxy(WeespinClub.Data.User account)
        {
            try
            {

          
                Task.Run(() =>
                {
                    
                    var currentprox = ProxyPool[new Random().Next(ProxyPool.Count)];
                    account.proxy = new Proxy(currentprox.IP, ushort.Parse(currentprox.port));
                    ProxyPool.Remove(currentprox);
                    Debug.WriteLine(ProxyPool.Count);
                });
            }
            catch (Exception e)
            {
                ForceLoadProxy(account);
            }

        }

    }
    public static class ThreadHelperClass
    {
        delegate void SetTextCallback(Form f, Control ctrl, ListView s);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling form</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void SetText(Form form, Control ctrl, ListView s)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new object[] { form, ctrl, s });
            }
            else
            {
               
            }
        }
    }
    /*
     * 
     * using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SynchronousSocketClient
{
    public static void StartClient()
    {
        // Data buffer for incoming data.
        byte[] bytes = new byte[1000000]; //1 megabyte

        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            // This example uses port 11000 on the local computer.
            IPHostEntry ipHostInfo = Dns.Resolve("95.213.247.178");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2222);

            // Create a TCP/IP  socket.
            Socket sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.
              
                var data =
                    "{\"age\":28,\"system_id\":\"df6fddd41ba4b69753f8d341d3ee73b4\",\"referrer_id\":\"\",\"club_id\":\"0\",\"id\":\"456290422\",\"type\":\"login\",\"auth\":\"6f527f0518afb4fc4189b17dc00b42e7\",\"referrer_type\":\"unknown\"}";
               
                // Send the data through the socket.
                int bytesSent = sender.Send(Converter.ConvertToKOKData(data));
                Console.WriteLine("Sent Bytes = {0}",
                    bytesSent);
                // Receive the response from the remote device.
                int bytesRec = sender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));
                // Send the data through the socket.
                  sender.Send(Converter.ConvertToKOKData("{\"type\":\"list_clubs\"}"));
                Console.WriteLine("Sent Bytes = {0}",
                    bytesSent);
                // Receive the response from the remote device.
                int bytesRec0 = sender.Receive(bytes);
               
                Console.WriteLine("Echoed test = {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec0));
                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Console.Read();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static int Main(String[] args)
    {
     
        StartClient();
        return 0;
    }
    public class Converter
    {
       public static byte[] ConvertToKOKData(string data)
        {
            
            var checksum = BitConverter.GetBytes(Convert.ToInt16(data.Length));
           
            Array.Reverse(checksum, 0,checksum.Length);
            var databin = Encoding.ASCII.GetBytes(data);

            return Combine(checksum, databin);
        }

      

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }

   
}
     */
}

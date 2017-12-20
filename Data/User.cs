using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Knapcode.SocketToMe.Socks;
using Newtonsoft.Json;
using System.Web;
using Timer = System.Threading.Timer;


namespace WeespinClub.Data
{
    public class User
    {
        Data.JsonClasses.GetProfile UserProfile = new Data.JsonClasses.GetProfile();
        byte[] buffer = new byte[1000000]; //1 megabyte
        private string realbuffer = "";
        public Socket socket;
        public string Coins;
        public Proxy proxy;
        public int id;
        public string auth_key;
        private Dictionary<string, int> CurrentProxy;
        private bool connected;
        private State CurrentState;
        private int CurrentClubId;
        private Thread ReaderThread;

        public void CloseCon()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        public bool CreateConenction()
        {
            try
            {
          

                //IPHostEntry ipHostInfo = Dns.Resolve("95.213.247.178");
                
               // socket = new Socket(AddressFamily.InterNetwork,
                 //   SocketType.Stream, ProtocolType.Tcp);
                try
                {

               
               socket = proxy.GetConnection("95.213.247.178", 2222);
                }
                catch (Exception e)
                {
                    WeespinClub.Form1.ForceInit(this);
                    throw;
                }
              
                ReaderThread = new Thread(ReadLoop);
                ReaderThread.Start();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

       


        public bool Send(byte[] whattosend)
        {
            try
            {

         
            socket.Send(whattosend);
            }
            catch (Exception e)
            {
             Debug.WriteLine(e);
            WeespinClub.Form1.ForceInit(this);
                Connect();
            }
            return true;
        }

        public void Recierver(bool islong, int recbytes = 0)
        {
            var data = "";
            if (islong)
            {
                data = realbuffer;
                realbuffer = "";
            }
            else
            {
                if (recbytes == 0)
                {
                   
                }
                data = Encoding.ASCII.GetString(buffer,2, recbytes-2);
            }
           


                var s = JsonConvert.DeserializeObject<WeespinClub.Data.JsonClasses.SimpleJson>(data);
            
            switch (s.type)
            {
                case "login":
                    var b = JsonConvert.DeserializeObject<WeespinClub.Data.JsonClasses.GetProfile>(data);
                    if (Form1.listView1.InvokeRequired)
                    {
                        Form1.listView1.Invoke((MethodInvoker)delegate ()
                        {
                           
                          //RECIEVE DIALY BONUS
                          //todo
                            var ss=Form1.listView1.Items .Cast<ListViewItem>()
                                .FirstOrDefault(x => x.Text == b.profile.id);
                            ss.SubItems[1].Text = (b.profile.gold + b.daily_bonus).ToString();
                            ss.SubItems[2].Text = "Logined";
                            //ss.SubItems[3].Text = b.profile.;
                        });
                    }
                    break;
                case "list_clubs":
                    if (Form1.GetClubs)
                    {
                        Form1.listView2.Invoke((MethodInvoker) delegate()
                        {
                            //RECIEVE DIALY BONUS
                            //todo

                            Form1.listView2.Items.Clear();
                            //ss.SubItems[3].Text = b.profile.;
                        });
                        var clubs = JsonConvert.DeserializeObject<WeespinClub.Data.JsonClasses.ClubListRoot>(data);
                        for (var index = 0; index < clubs.clubs.Count; index++)
                        {
                            var club = clubs.clubs[index];
                            Form1.listView2.Invoke((MethodInvoker) delegate()
                            {
                                //RECIEVE DIALY BONUS
                                //todo
                                //todo
                                var namencommand = new string[]
                                    {club.id, club.title, club.population.ToString(), index.ToString()};
                                var items = new ListViewItem(namencommand);
                                Form1.listView2.Items.Add(items);
                                //ss.SubItems[3].Text = b.profile.;
                            });
                        }
                        Form1.GetClubs = false;
                    }
                    break;
                case "chat":    
                    var chatdata = JsonConvert.DeserializeObject<WeespinClub.Data.JsonClasses.SomeoneChatted>(data);
                    if (chatdata.to_id == id.ToString())
                    {

                        var mat = Reply[new Random().Next(Reply.Count)];
                     
                        var json = JsonConvert.SerializeObject(mat, new JsonSerializerSettings
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                        });
                        //Converting to Unicode from UTF8 bytes
                        json.Replace(@"\\",@"\");
                        //Getting string from Unicode bytes
                        json = json.Trim('"');
                        SendChatToUser(json, chatdata.clubber.id);
                        //SendChat()
                    }
                    break;
            }


        }
        List<string> Reply = new List<string>(){"Xуй соси","Мать твою eбал","Сверни себе шею","Лучше пойди проверь свою мертвую мать на свалке.", "экибастус eбанный xуй соси", "ну ты и ебаклак конечно", "Ты вонь подритузная","Попробуй отсосать у самого себя", "больше сюда не пищи, от тебя говном воняет, отсюда чувствую", "У тебя из ПИ3ДЫЫЫЫЫ… из xуя-то лезет гной!", "Пососи мать! Молофейку вылей ей! Ох пердеть, ох пердеть, ох пердеть она ведь будет. Ох пердеть, ох пердеть, да?" };
     

        public void ReadLoop()
        {
            try
            {


                var itslooong = false;
                ushort bytelenghtlong = 0;
                while (true)
                {
                    Thread.Sleep(5);

                    //Here we will read bytes.

                    if (socket.Available > 0)
                    {
                        int recbytes = socket.Receive(buffer);


                        ushort actualbytes = BitConverter.ToUInt16(new byte[2] {(byte) buffer[1], (byte) buffer[0]}, 0);

                        if (actualbytes > recbytes)
                        {
                            if (!itslooong)
                            {
                                itslooong = true;
                                bytelenghtlong = actualbytes;
                            }


                        }
                        else
                        {

                            Recierver(false, recbytes);

                        }
                        if (itslooong)
                        {
                            if (actualbytes < recbytes)
                            {
                                Recierver(false, recbytes);
                            }
                            if (realbuffer.Length == 0)
                            {
                                realbuffer += Encoding.ASCII.GetString(buffer, 2, recbytes - 2);
                            }
                            else
                            {
                                realbuffer += Encoding.ASCII.GetString(buffer, 0, recbytes);
                            }
                        }
                        var r = Encoding.ASCII.GetString(buffer, 2, recbytes - 2);
                        Debug.WriteLine("Recieved " + r);
                        if (realbuffer.Length > bytelenghtlong)
                        {
                           Debug.WriteLine("TCP Protocol died?");
                        }
                        if (realbuffer.Length == bytelenghtlong && itslooong)
                        {
                            itslooong = false;
                            bytelenghtlong = 0;
                            if (realbuffer.Length > 0)
                            {
                                Recierver(true);
                            }
                        }

                    }
                }
            }

            catch (Exception e)
            {
               Debug.WriteLine("Reader"+e);
               Form1.ForceInit(this);
               Connect();
            }



        
        }
        public bool Connect()
        {
            try
            {

         
            var query = "{\"age\":28,\"auth\":\"" + auth_key +
                        "\",\"referrer_type\":\"user_apps\",\"system_id\":\"\",\"club_id\":\"0\",\"id\":\"" + id +
                        "\",\"type\":\"login\",\"referrer_id\":\"\"}";
            var whattosend = Utils.Converter.ConvertToKOKData(query);
            Send(whattosend);
            }
            catch (Exception e)
            {
                Form1.ForceInit(this);
            
            }
            return false;
        }

        public bool JoinClub(int clubid)
        {
            var query = "{\"club_id\":\"" + clubid + "\",\"type\":\"goto\"}";

            var whattosend = Utils.Converter.ConvertToKOKData(query);
            Send(whattosend);
            return false;
        }

        public bool SetSong(int songid)
        {
            var query =
                "{\"song\":{\"author\":\"gle6\",\"id\":\"" + songid +
                "\",\"duration\":60,\"type\":\"vkaudio\",\"title\":\"Azino King\",\"genre\":0},\"type\":\"enqueue\"}";
            var whattosend = Utils.Converter.ConvertToKOKData(query);
            Send(whattosend);
            return false;
        }

        public bool SetVote(Ratings rating)
        {
            string query = string.Empty;
            switch (rating)
            {
                case Ratings.Dislike:
                    query = "{\"vote\":\"dislike\",\"type\":\"vote\"}";
                    break;
                case Ratings.Like:
                    query = "{\"vote\":\"like\",\"type\":\"vote\"}";
                    break;
                case Ratings.SuperLike:
                    query = "{\"vote\":\"superlike\",\"type\":\"vote\"}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rating), rating, null);
            }
            var whattosend = Utils.Converter.ConvertToKOKData(query);
            Send(whattosend);
            return false;
        }

        public bool SendChat(string text)
        {
            string query = "{\"type\":\"chat\",\"text\":\"\'"+text+"\'\"}";
            var whattosend = Utils.Converter.ConvertToKOKData(query);
            Send(whattosend);
            return false;
        }

        public bool SendChatToUser(string text,string id)
        {
            var obj = new JsonClasses.SendChat();
            obj.text = text;
            obj.type = "chat";
            obj.to_id = id;
            var chat = JsonConvert.SerializeObject(obj);
            chat=chat.Replace(@"\\", @"\");
            Send(Utils.Converter.ConvertToKOKData(chat));
            return false;
        }

        public bool SendGift(GiftType giftType)
        {
            string query = string.Empty;
            switch (giftType)
            {
                case GiftType.Snow:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"snow\"}";
                    break;
                case GiftType.Kiss:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"kiss\"}";
                    break;
                case GiftType.Topaz:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"gemstone\"}";
                    break;
                case GiftType.Flower:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"flower\"}";
                    break;
                case GiftType.Tomato:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"tomato\"}";
                    break;
                case GiftType.Berry:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"berry\"}";
                    break;
                case GiftType.Egg:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"egg\"}";
                    break;
                case GiftType.Pepper:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"pepper\"}";
                    break;
                case GiftType.Heart:
                    query = "{\"type\":\"send_gift\",\"receiver_id\":\"\",\"gift\":\"heart\"}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(giftType), giftType, null);
            }

            var whattosend = Utils.Converter.ConvertToKOKData(query);
            Send(whattosend);
            return false;
        }

        public void GetClubs()
        {

            string query = "{\"type\":\"list_clubs\"}";
            var whattosend = Utils.Converter.ConvertToKOKData(query);
            Send(whattosend);
            
        }
    }

    public enum GiftType
    {
        Snow,
        Kiss,
        Topaz,
        Flower,
        Tomato,
        Berry,
        Egg,
        Pepper,
        Heart
    }

    internal enum State
    {
        Offline,
        ProxyError,
        Ingame,
        Inmenu,
        Banned
    }

    public enum Ratings
    {
        Dislike,
        Like,
        SuperLike
    }

   
}

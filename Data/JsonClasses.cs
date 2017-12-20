using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeespinClub.Data
{
    class JsonClasses
    {
        public class SendChat
        {
            public string to_id { get; set; }
            public string type { get; set; }
            public string text { get; set; }
        }
        public class Clubber
        {
            public string name { get; set; }
            public string photo { get; set; }
            public bool online { get; set; }
            public bool male { get; set; }
            public string id { get; set; }
            public string special { get; set; }
        }

        public class SomeoneChatted
        {
            public Clubber clubber { get; set; }
            public string text { get; set; }
            public string to_id { get; set; }
            public string type { get; set; }
            public double ts { get; set; }
        }
        public class Song
        {
            public string title { get; set; }
            public string author { get; set; }
            public int genre { get; set; }
            public int duration { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public string icon { get; set; }
        }

        public class Club
        {
            public Song song { get; set; }
            public string title { get; set; }
            public bool pinned { get; set; }
            public string icon_vk { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public int population { get; set; }
        }

        public class ClubListRoot
        {
            public List<Club> clubs { get; set; }
            public string type { get; set; }
        }
        public class Profile
        {
            public bool verified { get; set; }
            public string name { get; set; }
            public int gold { get; set; }
            public string photo { get; set; }
            public int age { get; set; }
            public double login_ts { get; set; }
            public bool vip { get; set; }
            public double create_ts { get; set; }
            public int purchased_gold { get; set; }
            public bool male { get; set; }
            public string id { get; set; }
        }

        public class GetProfile
        {
            public Profile profile { get; set; }
            public int daily_bonus { get; set; }
            public string type { get; set; }
            public double ts { get; set; }
        }
        public class SimpleJson
        {
            public string type { get; set; }
        }
    }
}

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace LightBridge.TwitterService.DataContracts
{
    [DataContract]
    public class Status
    {
        [DataMember]
        public string contributors;

        public DateTimeOffset created_at_dt;

        [DataMember]
        public string favorited;

        [DataMember]
        public geo geo;

        [DataMember]
        public string id;

        [DataMember]
        public string in_reply_to_screen_name;

        [DataMember]
        public string in_reply_to_status_id;

        [DataMember]
        public string in_reply_to_user_id;

        [DataMember]
        public string source;

        [DataMember]
        public string text;

        [DataMember]
        public string truncated;

        [DataMember]
        public User user;

        [DataMember]
        public string created_at
        {
            get
            {
                return created_at_dt.ToString("ddd MMM dd HH:mm:ss zzz yyyy");
            }
            set
            {
                created_at_dt = DateTimeOffset.ParseExact(value, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
            }
        }
    }

    [DataContract]
    public class geo
    {
        [DataMember]
        public string[] coordinates;

        [DataMember]
        public string type;
    }
}
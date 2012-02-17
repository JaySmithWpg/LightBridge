using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace LightBridge.TwitterService.DataContracts
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string contributors_enabled;

        public DateTimeOffset created_at_dt;

        [DataMember]
        public string description;

        [DataMember]
        public string favourites_count;

        [DataMember]
        public string followers_count;

        [DataMember]
        public string following;

        [DataMember]
        public string friends_count;

        [DataMember]
        public string id;

        [DataMember]
        public string lang;

        [DataMember]
        public string location;

        [DataMember]
        public string name;

        [DataMember]
        public string notifications;

        [DataMember]
        public string profile_background_color;

        [DataMember]
        public string profile_background_image_url;

        [DataMember]
        public string profile_background_tile;

        [DataMember]
        public string profile_image_url;

        [DataMember]
        public string profile_link_color;

        [DataMember]
        public string profile_sidebar_border_color;

        [DataMember]
        public string profile_sidebar_fill_color;

        [DataMember]
        public string profile_text_color;

        [DataMember]
        public string @protected;

        [DataMember]
        public string screen_name;

        [DataMember]
        public string statuses_count;

        [DataMember]
        public string time_zone;

        [DataMember]
        public string url;

        [DataMember]
        public string utc_offset;

        [DataMember]
        public string verified;

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
}
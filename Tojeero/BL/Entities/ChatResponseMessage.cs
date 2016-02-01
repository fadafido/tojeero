using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tojeero.Core;

namespace Tojeero.Forms.BL.Entities
{
    public class ChatResponseMessage : IChatResponseMessage
    {
        public ChatResponseMessage(string channelID, string jsonContent)
        {
            ChannelID = channelID;
            JsonContent = jsonContent;
        }
        public string ChannelID { get; set; }
        public string JsonContent { get; set; }
        public T GetContent<T>()
        {
            var content = this.JsonContent != null ? JsonConvert.DeserializeObject<T>(JsonContent) : default(T);
            return content;
        }
    }

    public class ChatResponseMessage<T> : ChatResponseMessage, IChatResponseMessage<T> where T : IChatMessage
    {
        public ChatResponseMessage(string channelID, string jsonContent) : base(channelID, jsonContent)
        {
        }

        public T Content
        {
            get { return GetContent<T>(); }
        }
    }
}

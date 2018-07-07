using Events;
using Events.ClientToAllClients;
using Spectrum.API.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ColoredName
{
    static class Message
    {
        public static void SendMessage(string message)
        {
#pragma warning disable CS0618 // Le type ou le membre est obsolète
            StaticTransceivedEvent<ChatMessage.Data>.Broadcast(new ChatMessage.Data( message));
#pragma warning restore CS0618 // Le type ou le membre est obsolète
        }
    }
}

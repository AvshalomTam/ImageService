using ImageService.Commands;
using ImageService.Communication;
using ImageService.Communication.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    class ClientManager : IClientManager
    {
        private Dictionary<int, IHandlerCommand> commands;
        private List<Client> clients;
        private Object thisLock = new Object();

        public ClientManager(Dictionary<int, IHandlerCommand> commands)
        {
            this.commands = commands;
            clients = new List<Client>();
        }

        /// <summary>
        /// Handles the client.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        public void HandleClient(TcpClient tcpClient)
        {
            new Task(() =>
            {
                Client client = new Client(tcpClient);
                lock (thisLock)
                {
                    clients.Add(client);
                }

                while (true)
                {
                    try
                    {
                        string message = client.Reader.ReadString();
                        JObject jmessage = JObject.Parse(message);
                        int commandID = (int)jmessage["command"];

                        try
                        {
                            if (this.commands.TryGetValue(commandID, out IHandlerCommand command))
                            {
                                string result = command.execute((string)jmessage["arguments"]);
                                if (!String.IsNullOrEmpty(result))
                                    client.Writer.Write(result);
                            }
                        }
                        catch { }
                    }
                    catch (IOException)
                    {
                        RemoveClient(client);
                        break;
                    }
                }                                
            }).Start();
        }

        /// <summary>
        /// Removes the client.
        /// </summary>
        /// <param name="client">The client.</param>
        public void RemoveClient(Client client)
        {
            lock (thisLock)
            {
                if (clients.Contains(client))
                {
                    client?.tcpClient.Close();
                    clients.Remove(client);
                }
            }
        }

        /// <summary>
        /// Broadcasts the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="TransferEventArgs"/> instance containing the event data.</param>
        public void Broadcast(object sender, TransferEventArgs args)
        {
            lock (thisLock)
            {
                foreach (Client client in clients)
                {
                    client.Writer.Write(args.message);
                }
            }
        }

        /// <summary>
        /// Closes the clients.
        /// </summary>
        public void CloseClients()
        {
            lock (thisLock)
            {
                foreach (Client c in clients)
                {
                    c.tcpClient.Close();
                }
                clients = new List<Client>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;
using WebSocket4Net.Command;
using WebSocket4Net.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading;

namespace JsonApi.Client
{
    using Utils;
    using Classes;

    public class Client
    {
        private WebSocket _socket;
        private List<Subscription> _subscriptions = new List<Subscription>();

        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Hostname { get; set; }
        public int WebSocketsPort { get; set; }
        public int HttpPort { get; set; }

        private SHA256 KeyGenerator = SHA256.Create();
        internal string GetKey(string source_or_method) {
            return BitConverter.ToString(KeyGenerator.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}{3}", Username, source_or_method, Password, Salt)))).Replace("-", "").ToLower();
        }

        public Client()
        {
        }

        public void Init()
        {
            _socket = new WebSocket(string.Format("{0}://{1}:{2}", "ws", Hostname, WebSocketsPort));

            // Handle all incoming messages as JSON objects
            _socket.MessageReceived += (sender, e) =>
            {
                var data = JObject.Parse(e.Message);
                if (data.GetValue("result").ToObject<string>() != "success")
                {
                    // TODO: Proper error passing to the instance host
                    Debug.WriteLine("Got error message via websockets: {0}", data.GetValue("error").ToString());
                }

                // Give the stream message to every fitting subscriber
                foreach (var subscription in _subscriptions.Where(s => s.Source == data.GetValue("source").ToObject<string>()))
                    subscription.Pass(new StreamMessageEventArgs(data.GetValue("success").ToObject<StreamMessage>()));
            };

            _socket.Open();
        }

        #region HTTP: Synchronous requests
        public JToken Request(StandardAPIRequest request)
        {
            if (_socket == null)
                throw new InvalidOperationException("You need to call Init() first.");

            var obj = JObject.Parse(new WebClient().DownloadString(new Uri(new Uri(string.Format("{0}://{1}:{2}", "http", Hostname, HttpPort)), request.GenerateRequestString(this))));
            if (obj.SelectToken("success") == null)
                throw new Exception(string.Format("Request failed: {0}", obj.SelectToken("error").ToString()));

            return obj.SelectToken("success");
        }
        public JToken Request(params StandardAPIRequest[] requests)
        {
            return Request(new MultipleAPIRequest(requests));
        }
        public JToken Request(MultipleAPIRequest request)
        {
            if (_socket == null)
                throw new InvalidOperationException("You need to call Init() first.");

            var obj = JObject.Parse(new WebClient().DownloadString(new Uri(new Uri(string.Format("{0}://{1}:{2}", "http", Hostname, HttpPort)), request.GenerateRequestString(this))));
            if (obj.SelectToken("success") == null)
                throw new Exception(string.Format("Request failed: {0}", obj.SelectToken("error").ToString()));

            return obj.SelectToken("success");
        }
        public T Request<T>(StandardAPIRequest request)
        {
            return JsonConvert.DeserializeObject<T>(Request(request).ToString(), new UnixDateTimeConverter());
        }
        public T Request<T>(params StandardAPIRequest[] requests)
        {
            return Request<T>(new MultipleAPIRequest(requests));
        }
        public T Request<T>(MultipleAPIRequest request)
        {
            return JsonConvert.DeserializeObject<T>(Request(request).ToString(), new UnixDateTimeConverter());
        }
        #endregion

        #region WebSockets: Streams
        public void Subscribe(string source, bool showPrevious = true)
        {
            if (_socket == null)
                throw new InvalidOperationException("You need to call Init() first.");

            if (_subscriptions.Any(sub => sub.Source.Equals(source, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Already subscribed to this event, only one subscription per source possible.");

            _socket.Send(new StreamAPIRequest() { SourceName = source, ShowPrevious = showPrevious }.GenerateRequestString(this));
        }
        public void Unsubscribe(string source)
        {
            if (_socket == null)
                throw new InvalidOperationException("You need to call Init() first.");

            _socket.Send(new StreamAPIRequest() { SourceName = source, Unsubscribe = true }.GenerateRequestString(this));
        }
        #endregion

        /*
        internal void Request(IAPIRequest request)
        {
            string uri = request.GenerateRequestString(this);
            Console.WriteLine(uri);
            _socket.Send(uri);
        }

        internal JToken ReceiveSync()
        {
            _jsonDataSyncBarrier.Reset();
            _lastJsonDataReceived = null;
            _jsonDataSyncBarrier.Wait(10000);

            if (_lastJsonDataReceived == null)
                throw new TimeoutException();

            if (_lastJsonDataReceived.SelectToken("success", false) != null)
                return _lastJsonDataReceived.SelectToken("success");

            throw new Exception(_lastJsonDataReceived.ToString());
        }

        public void AddEnchantmentToPlayerInventorySlot(string playername, int slot, int enchantmentID, int enchantmentLevel)
        {
        }

        private string GenerateIdentifier()
        {
            return Convert.ToBase64String(BitConverter.GetBytes(DateTime.Now.ToBinary()));
        }

        public T Request<T>(string method, params object[] arguments)
        {
            Request(GenerateIdentifiedRequest(method, arguments));
            return ReceiveSync().ToObject<T>();
        }

        private StandardAPIRequest GenerateIdentifiedRequest(string method, params object[] arguments)
        {
            var id = GenerateIdentifier();
            _jsonExpectIdentifier = id;
            return new StandardAPIRequest(id, method, arguments);
        }

        private StandardAPIRequest GenerateStandardRequest(string method, params object[] arguments)
        {
            return new StandardAPIRequest(method, arguments);
        }

        public Block GetBlock(string worldname, int x, int y, int z)
        {
            Request(GenerateStandardRequest("getBlock", worldname, x, y, z));
            return ReceiveSync().ToObject<Block>();
        }

        public int GetPlayerCount()
        {
            Request(GenerateStandardRequest("getPlayerCount"));
            return ReceiveSync().ToObject<int>();
        }

        public string[] GetPlayerNames()
        {
            Request(GenerateStandardRequest("getPlayers"));
            return ReceiveSync().ToObject<string[]>();
        }

        public string[] GetOpList()
        {
            Request(GenerateStandardRequest("getOpList"));
            return ReceiveSync().ToObject<string[]>();
        }

        public string[] GetOnlinePlayerNamesInWorld(string world)
        {
            Request(GenerateStandardRequest("getOnlinePlayerNamesInWorld", world));
            return ReceiveSync().ToObject<string[]>();
        }

        public string[] GetOfflinePlayerNames()
        {
            Request(GenerateStandardRequest("getOfflinePlayerNames"));
            return ReceiveSync().ToObject<string[]>();
        }

        public string[] GetOfflinePlayerNamesInWorld(string world)
        {
            Request(GenerateStandardRequest("getOfflinePlayerNamesInWorld", world));
            return ReceiveSync().ToObject<string[]>();
        }

        public int GetPlayerLimit()
        {
            Request(GenerateStandardRequest("getPlayerLimit"));
            return ReceiveSync().ToObject<int>();
        }

        public void KickPlayer(string name, string message)
        {
            Request(GenerateStandardRequest("kickPlayer", name, message));
        }

        public void OpPlayer(string name)
        {
            Request(GenerateStandardRequest("opPlayer", name));
        }

        public void RunConsoleCommand(string cmd)
        {
            Request(GenerateStandardRequest("runConsoleCommand", cmd));
        }

        public void SaveMap()
        {
            Request(GenerateStandardRequest("saveMap"));
        }

        public void SaveOff()
        {
            Request(GenerateStandardRequest("saveOff"));
        }

        public void SaveOn()
        {
            Request(GenerateStandardRequest("saveOn"));
        }

        public void SendMessage(string player, string message)
        {
            Request(GenerateStandardRequest("sendMessage", player, message));
            ReceiveSync();
        }

        public void ReloadServer()
        {
            Request(GenerateStandardRequest("reloadServer"));
            ReceiveSync();
        }

        public void SetBlockData(string world, int x, int y, int z, int data)
        {
            Request(GenerateStandardRequest("setBlockData", world, x, y, z, data));
            ReceiveSync();
        }

        public void SetBlockType(string world, int x, int y, int z, int type)
        {
            Request(GenerateStandardRequest("setBlockType", world, x, y, z, type));
            ReceiveSync();
        }

        public bool SetPlayerExperience(string player, int xp)
        {
            Request(GenerateStandardRequest("setPlayerExperience", player, xp));
            return ReceiveSync().ToObject<bool>();
        }

        public bool SetPlayerFoodLevel(string player, int level)
        {
            Request(GenerateStandardRequest("setPlayerFoodLevel", player, level));
            return ReceiveSync().ToObject<bool>();
        }

        public bool SetPlayerHealth(string player, int hp)
        {
            Request(GenerateStandardRequest("setPlayerHealth", player, hp));
            return ReceiveSync().ToObject<bool>();
        }

        public void SetPlayerGameMode(string player, int mode)
        {
            Request(GenerateStandardRequest("setPlayerGameMode", player, mode));
            ReceiveSync();
        }
         */
    }
}

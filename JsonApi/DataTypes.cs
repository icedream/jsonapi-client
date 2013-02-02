using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonApi.Client.Classes;

namespace JsonApi.Client.DataTypes
{
    [JsonObject()]
    public class Block
    {
        /// <summary>
        /// The block data
        /// </summary>
        [JsonProperty("data")]
        public uint Data { get; private set; }

        /// <summary>
        /// The block type
        /// </summary>
        [JsonProperty("type")]
        public uint Type { get; private set; }
    }

        [JsonObject()]
    public class Connection
    {
        /// <summary>
        /// The last connection time
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }

        /// <summary>
        /// The name of the player who was playing via this connection
        /// </summary>
        [JsonProperty("player")]
        public string Player { get; private set; }

        /// <summary>
        /// Can be either "disconnected" or "connected"
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }
    }

    [JsonObject()]
    public class Message
    {
        /// <summary>
        /// The text of the message
        /// </summary>
        [JsonProperty("message")]
        public string MessageText { get; private set; }

        /// <summary>
        /// The last connection time
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }

        /// <summary>
        /// The name of the player who was playing via this connection
        /// </summary>
        [JsonProperty("player")]
        public string Player { get; private set; }
    }

    [JsonObject()]
    public class LogLine
    {
        /// <summary>
        /// The text of the log line
        /// </summary>
        [JsonProperty("line")]
        public string Line { get; private set; }

        /// <summary>
        /// The last connection time
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }
    }

    [JsonObject()]
    public class Player
    {
        [JsonProperty("whitelisted")]
        public bool IsWhitelisted { get; private set; }

        [JsonProperty("firstPlayed")]
        public DateTime FirstPlayedTime { get; private set; }

        [JsonProperty("worldInfo")]
        public World WorldInfo { get; private set; }

        [JsonProperty("op")]
        public bool IsOp { get; private set; }

        [JsonProperty("location")]
        public Location Location { get; private set; }

        [JsonProperty("exhaustion")]
        public float Exhaustion { get; private set; }

        [JsonProperty("lastPlayed")]
        public DateTime LastPlayedTime { get; private set; }

        [JsonProperty("sleeping")]
        public bool IsSleeping { get; private set; }

        [JsonProperty("health")]
        public int Health { get; private set; }

        [JsonProperty("banned")]
        public bool IsBanned { get; private set; }

        [JsonProperty("ip")]
        public string IP { get; private set; }

        [JsonProperty("gameMode")]
        public GameMode GameMode { get; private set; }

        [JsonProperty("inVehicle")]
        public bool IsInVehicle { get; private set; }

        [JsonProperty("level")]
        public int Level { get; private set; }

        [JsonProperty("inventory")]
        public Inventory Inventory { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("foodLevel")]
        public int FoodLevel { get; private set; }

        [JsonProperty("experience")]
        public long Experience { get; private set; }

        [JsonProperty("itemInHand")]
        public Item ItemInHand { get; private set; }

        [JsonProperty("sneaking")]
        public bool IsSneaking { get; private set; }

        [JsonProperty("world")]
        public int World { get; private set; }

        [JsonProperty("sprinting")]
        public bool IsSprinting { get; private set; }

        [JsonProperty("enderchest")]
        public Item[] Enderchest { get; private set; }
    }

    [JsonObject()]
    public class Armor
    {
        [JsonProperty("helmet")]
        public Item Helmet { get; private set; }

        [JsonProperty("boots")]
        public Item Boots { get; private set; }

        [JsonProperty("leggings")]
        public Item Leggings { get; private set; }

        [JsonProperty("chestplate")]
        public Item Chestplate { get; private set; }
    }

    [JsonObject()]
    public class Inventory
    {
        [JsonProperty("armor")]
        public Armor Armor { get; private set; }

        [JsonProperty("hand")]
        public Item Hand { get; private set; }

        [JsonProperty("inventory")]
        public Item[] InventoryContent { get; private set; }
    }

    [JsonObject()]
    public class Item
    {
        [JsonProperty("enchantments")]
        public Dictionary<string, int> Enchantments { get; private set; }

        [JsonProperty("amount")]
        public int Amount { get; private set; }

        [JsonProperty("durability")]
        public int Durability { get; private set; }

        [JsonProperty("type")]
        public int Type { get; private set; }

        [JsonProperty("dataValue")]
        public int DataValue { get; private set; }
    }

    [JsonObject()]
    public class Location
    {
        [JsonProperty("yaw")]
        public double Yaw { get; private set; }

        [JsonProperty("pitch")]
        public double Pitch { get; private set; }

        [JsonProperty("z")]
        public double Z { get; private set; }

        [JsonProperty("y")]
        public double Y { get; private set; }

        [JsonProperty("x")]
        public double X { get; private set; }
    }

    [JsonObject()]
    public class World
    {
        [JsonProperty("remainingWeatherTicks")]
        public ushort RemainingWeatherTicks { get; private set; }

        [JsonProperty("hasStorm")]
        public bool HasStorm { get; private set; }

        [JsonProperty("time")]
        public uint Time { get; private set; }

        [JsonProperty("environment")]
        public string Environment { get; private set; }

        [JsonProperty("isThundering")]
        public bool IsThundering { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("fullTime")]
        public ulong FullTime { get; private set; }
    }

    public enum GameMode
    {
        Survival = 0,
        Creative = 1,
        Adventure = 2
    }

    [JsonObject()]
    public class Plugin
    {
        [JsonProperty("enabled")]
        public bool IsEnabled { get; private set; }

        [JsonProperty("authors")]
        public string[] Authors { get; private set; }

        [JsonProperty("website")]
        public string Website { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("version")]
        public string Version { get; private set; }
    }

    [JsonObject()]
    public class Enchantment
    {
        [JsonProperty("id")]
        public int ID { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("maxLevel")]
        public int MaxLevel { get; private set; }

        [JsonProperty("startLevel")]
        public int StartLevel { get; private set; }

        [JsonProperty("itemTarget")]
        public EnchantmentTarget ItemTarget { get; private set; }
    }

    [JsonObject()]
    public class EnchantmentTarget
    {
        [JsonProperty("includes")]
        public bool Includes { get; private set; } // Uhm, wtf... nvm, got this from Bukkit JD, hopefully it's correct.
    }

    [JsonObject()]
    public class Server
    {
        [JsonProperty("port")]
        public ushort Port { get; private set; }

        [JsonProperty("maxPlayers")]
        public int MaxPlayers { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("players")]
        public Player[] Players { get; private set; }

        [JsonProperty("worlds")]
        public World[] Worlds { get; private set; }

        [JsonProperty("serverName")]
        public string ServerName { get; private set; }

        [JsonProperty("version")]
        public string Version { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonApi.Client.Classes;

namespace JsonApi.Client.DataTypes
{
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

        [JsonProperty("armor")]
        public Armor Armor { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("foodLevel")]
        public int FoodLevel { get; private set; }

        [JsonProperty("experience")]
        public long Experience { get; private set; }

        [JsonProperty("itemInHand")]
        public Item ItemInHand { get; private set; }

        [JsonProperty("sneaking")]
        public bool IsSneaking { get; set; }

        [JsonProperty("world")]
        public int World { get; set; }

        [JsonProperty("sprinting")]
        public bool IsSprinting { get; set; }
    }

    public class Armor
    {
        [JsonProperty("helmet")]
        public Item Helmet { get; set; }

        [JsonProperty("boots")]
        public Item Boots { get; set; }

        [JsonProperty("leggings")]
        public Item Leggings { get; set; }

        [JsonProperty("chestplate")]
        public Item Chestplate { get; set; }
    }

    public class Inventory
    {
        [JsonProperty("hand")]
        public Item Hand { get; set; }

        [JsonProperty("inventory")]
        public Item[] InventoryContent { get; set; }
    }

    public class Item
    {
        [JsonProperty("enchantments")]
        public object Enchantments { get; private set; } // TODO: Find out the structure of Enchantments

        [JsonProperty("amount")]
        public ushort Amount { get; private set; }

        [JsonProperty("durability")]
        public int Durability { get; private set; }

        [JsonProperty("type")]
        public int Type { get; private set; }

        [JsonProperty("dataValue")]
        public long DataValue { get; private set; }
    }

    public class Location
    {
        [JsonProperty("yaw")]
        public double Yaw { get; set; }

        [JsonProperty("pitch")]
        public double Pitch { get; set; }

        [JsonProperty("z")]
        public double Z { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }
    }

    public class World
    {
        [JsonProperty("remainingWeatherTicks")]
        public ushort RemainingWeatherTicks { get; set; }

        [JsonProperty("hasStorm")]
        public bool HasStorm { get; set; }

        [JsonProperty("time")]
        public uint Time { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }

        [JsonProperty("isThundering")]
        public bool IsThundering { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fullTime")]
        public ulong FullTime { get; set; }
    }

    public enum GameMode
    {
        Survival = 0,
        Creative = 1,
        Adventure = 2
    }
}

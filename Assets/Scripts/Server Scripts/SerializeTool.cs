using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

namespace Castling.Server
{
    public class SerializeTool
    {
        public static byte[] Serialize<T>(T obj) where T : class
        {
            try
            {
                string json = JsonSerialize(obj);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                return bytes;
            }
            catch(Exception e)
            {
                Debug.LogError($"Serialization erro: {e.Message}");

                return new byte[0];
            }
        }

        public static T Deserialize<T>(byte[] bytes) where T : class
        {
            try
            {
                string json = Encoding.UTF8.GetString(bytes);
                T obj = JsonDeserialize<T>(json);

                return obj;
            }
            catch(Exception e)
            {
                Debug.LogError($"Deserialization error: {e.Message}");

                return null;
            }
        }

        public static string JsonSerialize<T>(T obj) where T : class
        {
            string json = JsonConvert.SerializeObject(obj);

            return json;
        }

        public static T JsonDeserialize<T>(string json)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(json);

                return obj;
            }
            catch(JsonException ex)
            {
                Debug.LogError($"JSON Deserialization failed: {ex.Message}");

                return default(T);
            }
        }
    }
}
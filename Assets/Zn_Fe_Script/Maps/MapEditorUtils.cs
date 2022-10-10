using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Zn_Fe.Maps
{
    public class MapEditorUtils
    {
        public static List<T> GetListFormEnum<T>()
        {
            var result = new List<T>();
            Array enums = Enum.GetValues(typeof(T));
            foreach (T e in enums)
            {
                result.Add(e);
            }

            return result;
        }

        public static string GetSerializeJson(SaveMapItem saveMapItem) => JsonMapper.ToJson(saveMapItem);

        public static T DeserializeObject<T>(string s) => JsonMapper.ToObject<T>(s);
        
    }
}
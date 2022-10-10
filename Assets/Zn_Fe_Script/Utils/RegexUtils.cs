using System.Text.RegularExpressions;
using UnityEngine;

namespace Zn_Fe_Script.Utils
{
    public static class RegexUtils
    {
        public static string RegexMapX(string info)
        {
            string patternMapX = "(?<=\"x\"\\:)\\s*\\d*[0-9]";
            var mc = Regex.Match(info,patternMapX);
            return mc.Value;
        }
        
        public static string RegexMapY(string info)
        {
            string patternMapY = "(?<=\"y\"\\:)\\s*\\d*[0-9]";
            var mc = Regex.Match(info,patternMapY);
            return mc.Value;
        }

        public static string RegexMapName(string info)
        {
            string patternMapName = "(?<=\"mapName\"\\:)\\s*\\D*[\\u4e00-\\u9fa5]";
            var mc = Regex.Match(info,patternMapName);
            var result = mc.Value.Replace("\"", "").Trim();
            return result;
        }
    }
}
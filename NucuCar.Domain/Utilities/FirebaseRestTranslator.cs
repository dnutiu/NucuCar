using System;
using System.Collections.Generic;

namespace NucuCar.Domain.Utilities
{
    /// <summary>
    /// This class is used to translate C# dictionaries into firebase json format. 
    /// </summary>
    public static class FirebaseRestTranslator
    {
        public static Dictionary<string, object> Translate(string name, Dictionary<string, object> dict)
        {
            return BuildRoot(name, dict);
        }

        private static Dictionary<string, object> BuildRoot(string name, Dictionary<string, object> dict)
        {
            var root = new Dictionary<string, object>();
            if (name != null)
            {
                root["name"] = name;
            }

            root["fields"] = new Dictionary<string, object>();
            // iterate through fields and build leaf
            foreach (var entry in dict)
            {
                var fields = (Dictionary<string, object>) root["fields"];
                fields[entry.Key] = BuildNode(entry.Value);
            }

            return root;
        }

        private static Dictionary<string, object> BuildNode(object value)
        {
            switch (value)
            {
                case string v:
                {
                    return BuildString(v);
                }
                case int v:
                {
                    return BuildInteger(v);
                }
                case double v:
                {
                    return BuildDouble(v);
                }
                case bool v:
                {
                    return BuildBool(v);
                }
                case DateTime v:
                {
                    return BuildTimestamp(v);
                }
                case List<Dictionary<string, object>> v:
                {
                    return BuildArray(v);
                }
                case Dictionary<string, object>[] v:
                {
                    return BuildArray(new List<Dictionary<string, object>>(v));
                }
                case Dictionary<string, object> v:
                {
                    return BuildMap(v);
                }
                default:
                {
                    if (value.GetType().IsEnum)
                    {
                        return BuildInteger((int) value);
                    }
                    break;
                }
            }
            throw new ArgumentException($"Can't build leaf! Unknown type for: {value}");
        }

        private static Dictionary<string, object> BuildSimpleValue(string type, object value)
        {
            return new Dictionary<string, object>()
            {
                [type] = value
            };
        }

        private static Dictionary<string, object> BuildString(string value)
        {
            return BuildSimpleValue("stringValue", value);
        }

        private static Dictionary<string, object> BuildInteger(int value)
        {
            return BuildSimpleValue("integerValue", value);
        }

        private static Dictionary<string, object> BuildTimestamp(DateTime value)
        {
            return BuildSimpleValue("timestampValue", value);
        }

        private static Dictionary<string, object> BuildDouble(double value)
        {
            return BuildSimpleValue("doubleValue", value);
        }

        private static Dictionary<string, object> BuildBool(bool value)
        {
            return BuildSimpleValue("booleanValue", value);
        }

        private static Dictionary<string, object> BuildArray(List<Dictionary<string, object>> array)
        {
            var values = new List<Dictionary<string, object>>();
            var root = new Dictionary<string, object>
            {
                ["arrayValue"] = new Dictionary<string, object>
                {
                    ["values"] = values
                }
            };

            foreach (var entry in array)
            {
                values.Add(BuildNode(entry));
            }

            return root;
        }

        private static Dictionary<string, object> BuildMap(Dictionary<string, object> map)
        {
            var fields = new Dictionary<string, object>();
            var root = new Dictionary<string, object>
            {
                ["mapValue"] = new Dictionary<string, object>
                {
                    ["fields"] = fields
                }
            };
            foreach (var entry in map)
            {
                fields[entry.Key] = BuildNode(entry.Value);
            }

            return root;
        }
    }
}
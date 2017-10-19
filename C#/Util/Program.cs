using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTRider.FluidSql;

namespace Util
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new JObject();
            var definitions = new JObject();
            root.Add("definitions", definitions);



            var assembly = typeof(Sql).GetTypeInfo().Assembly;

            //get all types derived from Token
            var tokenType = typeof(Token);

            var interfaces = new HashSet<Type>() { typeof(Token) };
            var types = new HashSet<Type>() { typeof(Token) };
            var enums = new HashSet<Type>();

            foreach (var type in assembly.GetTypes()
                .Where(tp => tokenType.IsAssignableFrom(tp))
                .Where(tp => tokenType != tp)
                .OrderBy(tp => tp.FullName))
            {
                types.Add(type);
            }

            foreach (var type in assembly.GetTypes()
                .Where(tp => tokenType.IsAssignableFrom(tp))
                .Where(tp => tokenType != tp)
                .OrderBy(tp => tp.FullName))
            {
                var parents = new JArray();
                var obj = new JObject
                {
                    {"allOf", parents }
                };

                var baseType = type.GetTypeInfo().BaseType;
                if (baseType != typeof(object))
                {
                    parents.Add(new JObject
                    {
                        {"$ref", $"#/definitions/{baseType.Name}" }
                    });
                }

                foreach (var i in type.GetInterfaces()
                    .Where(it => it.GetTypeInfo().Assembly.Equals(assembly)))
                {

                    interfaces.Add(i);
                    parents.Add(new JObject
                    {
                        {"$ref", $"#/definitions/{i.Name}" }
                    });
                }

                var props = new JObject();
                var def = new JObject
                {
                    {"type", "object"},
                    { "properties",props}
                };
                parents.Add(def);

                foreach (var propertyInfo in type.GetProperties()
                    .Where(p => p.CanRead && p.CanWrite)
                    .OrderBy(p => p.Name))
                {
                    // if property is a dictionary: create additionalProperties
                    // if property is a list/array - create array
                    // if property is a simle type - streate simple type
                    var prop = new JObject();
                    props.Add(propertyInfo.Name, prop);

                    var pt = propertyInfo.PropertyType;

                    if (pt.IsConstructedGenericType)
                    {
                        var gt = pt.GetGenericTypeDefinition();

                        if (gt == typeof(List<>))
                        {
                            var items = new JObject();
                            prop.Add("type", "array");
                            prop.Add("items", items);
                            pt = pt.GenericTypeArguments[0];
                            prop = items;
                        }


                        if (gt == typeof(Nullable<>))
                        {
                            prop.Add("nullable", "true");
                            pt = pt.GenericTypeArguments[0];
                        }
                    }

                    if (pt == typeof(string))
                    {
                        prop.Add("type", "string");
                    }
                    else if (pt == typeof(bool))
                    {
                        prop.Add("type", "boolean");
                    }
                    else if (pt == typeof(int))
                    {
                        prop.Add("type", "integer");
                        prop.Add("format", "int32");
                    }
                    else if (pt == typeof(long))
                    {
                        prop.Add("type", "integer");
                        prop.Add("format", "int64");
                    }
                    else if (pt == typeof(byte))
                    {
                        prop.Add("type", "integer");
                        prop.Add("format", "int8");
                    }
                    else if (pt == typeof(short))
                    {
                        prop.Add("type", "integer");
                        prop.Add("format", "int16");
                    }
                    else if (pt == typeof(DateTime))
                    {
                        prop.Add("type", "string");
                        prop.Add("format", "date-time");
                    }
                    else if (pt == typeof(TimeSpan))
                    {
                        prop.Add("type", "string");
                        prop.Add("format", "time-span");
                    }
                    else if (pt == typeof(object))
                    {
                        prop.Add("type", "object");
                    }
                    else if (pt.GetTypeInfo().IsEnum)
                    {
                        enums.Add(pt);
                        prop.Add("$ref", "#/definitions/" + pt.Name);
                    }
                    else
                    {
                        prop.Add("$ref", "#/definitions/" + pt.Name);
                    }
                }


                definitions.Add(type.Name, obj);
            }

            foreach (var type in enums
                .OrderBy(tp => tp.FullName))
            {
                var vt = new JArray();

                var et = new JObject
                {
                    {"type", "integer"},
                    {"enum", vt}
                };


                definitions.Add(type.Name, et);

                var names = Enum.GetNames(type);

                var values = Enum.GetValues(type);

                for (var i = 0; i < names.Length && i < values.Length; i++)
                {
                    vt.Add($"{names[i]}:{(int)values.GetValue(i)}");
                }

                if (type.GetTypeInfo().GetCustomAttribute<FlagsAttribute>() != null)
                {
                    et.Add("decoration","flags");
                }

            }

            foreach (var type in interfaces
                .OrderBy(tp => tp.FullName))
            {
                definitions.Add(type.Name, new JObject
                {
                    {"type", "object"},
                    {"decoration", "interface"}
                });

            }

            File.WriteAllText(@"c:\temp\definitions.json", JsonConvert.SerializeObject(root, Formatting.Indented));
        }
    }
}
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace UI_Test
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonRepoAttribute : Attribute
    {
        public string Name { get; set; }
        public string Encoder { get; set; }
        public string Decoder { get; set; }
        public bool NotNull { get; set; } = false;
    }

    public class JsonAnot
    {
        //Singleton
        static public JsonAnot Inst = new JsonAnot();

        public Logger log = Logger.GetLogger("Json");
        public static void Decode(Object target, JObject parent)
        {
            Logger log = Logger.GetLogger("json");
            Type type = target.GetType();
            var fields = type.GetProperties().ToList();
            foreach (var field in fields)
            {
                JsonRepoAttribute attribute = field.GetCustomAttribute(typeof(JsonRepoAttribute)) as JsonRepoAttribute;
                try
                {

                    if (attribute != null)
                    {
                        var Name = attribute.Name;

                        //  log.info("## JSON.DECODE {0} ==> {1}  ({2})  ", Name, parent[Name], field.PropertyType.FullName);
                        if (!parent.ContainsKey(Name))
                        {
                            Debug.WriteLine("JSON.WARNING.NAME =" + attribute.Name);
                            continue;

                        }

                        //if ( field.PropertyType.FullName.Contains("Visibility")) {
                        //    var value = (Visibility)Enum.Parse(typeof(Visibility), field.PropertyType.FullName);
                        //    Debug.WriteLine(Name);
                        //}
                        
                        switch (field.PropertyType.FullName)
                        {
                            case "Visiblity":
                                var value2 = (Visibility)Enum.Parse(typeof(Visibility), field.PropertyType.FullName);
                                field.SetValue(target, value2);
                                break;
                            case "System.Boolean":
                                field.SetValue(target, (bool)parent[Name]);
                                break;
                            case "System.Double":
                                field.SetValue(target, (Double)parent[Name]);
                                break;
                            case "System.Int32":
                                field.SetValue(target, (int)parent[Name]);
                                break;
                            case "System.String":
                                field.SetValue(target, (string)parent[Name]);
                                break;
                            default:
                                if (field.PropertyType.IsEnum)
                                {
                                    var sValue = (string)parent[Name];
                                    if (sValue != null && sValue.Length > 0)
                                    {
                                        Object value = Enum.Parse(field.PropertyType, sValue);
                                        field.SetValue(target, value);
                                    }
                                    break;
                                }
                                if (field.PropertyType.IsClass)
                                {
                                    JObject child = (JObject)parent[Name];

                                    Object nTarget = field.GetValue(target);
                                    if (nTarget != null)
                                    {
                                        log.Info("==> CLASS decode " + Name);
                                        Decode(nTarget, child);
                                    }
                                    else
                                    {
                                        log.error("decode error class=" + Name + " is null");
                                    }
                                }
                                else
                                {
                                    log.error("JSON.DECODE ERROR : name={0} type={1}", Name, field.PropertyType.FullName);
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debug.WriteLine("ERROR.NAME =" + attribute.Name);
                }

            }
        }
        static public void Encode(Object target, JObject parent)
        {
            Logger log = Logger.GetLogger("Json");
            Type type = target.GetType();
            var fields = type.GetProperties().ToList();
            foreach (var field in fields)
            {
                JsonRepoAttribute attribute = field.GetCustomAttribute(typeof(JsonRepoAttribute)) as JsonRepoAttribute;
                if (attribute != null)
                {
                    var Name = attribute.Name;
                    var Value = field.GetValue(target);
                    log.Info(
                        String.Format("## JSON.ENCODE {0} <== {1}  " +
                        "\nFull = ({2}) " +
                        "\nSimple = {3} ", Name, Value, field.PropertyType.FullName, field.PropertyType.Name));
                    if (attribute.NotNull && Value == null)
                    {
                        Debug.Fail("Jason Null name = " + attribute.Name);
                        continue;
                    }

                    switch (field.PropertyType.Name)
                    {
                        case "ObservableCollection`1":
                            Inst.Encode_List(Name,  (IList)Value, parent);
                            continue;
                    }

                    switch (field.PropertyType.FullName)
                    {
                        case "System.Boolean":
                            parent.Add(Name, (bool)field.GetValue(target));
                            break;
                        case "System.Double":
                            parent.Add(Name, (Double)field.GetValue(target));
                            break;
                        case "System.Int32":
                            parent.Add(Name, (int)field.GetValue(target));
                            break;
                        case "System.String":
                            parent.Add(Name, (string)field.GetValue(target));
                            break;
                        default:
                            Object myObj = field.GetValue(target);
                            if (myObj != null)
                            {
                                Type t = myObj.GetType();
                                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) // 새로운 클래스
                                {
                                    Inst.Encode_Dictionary("name",
                                        myObj as System.Collections.IDictionary,
                                        parent);
                                }
                                if (myObj.GetType().IsEnum)
                                {
                                    parent.Add(Name, myObj.ToString());
                                    break;
                                }
                                if (myObj.GetType().IsClass) // 새로운 클래스
                                {
                                    JObject child = new JObject();
                                    parent.Add(Name, child);
                                    Encode(myObj, child);
                                }

                            }
                            else
                            {
                                log.Info(
                                    String.Format("JSON.Encode ERROR : name={0} type={1}", Name, field.PropertyType.FullName));
                            }
                            break;
                    }
                }
                else
                {
                    // too much log.Info("NO JSON ATTRIBUTE !!1");
                }
            }

        }


        // =========================================================================================
        // Encode
        // =========================================================================================

        public void Encode_Dictionary(string Name, IDictionary target, JObject jobj)
        {
            Type t = target.GetType();
            Type keyType = t.GetGenericArguments()[0];
            Type valueType = t.GetGenericArguments()[1];
            JObject child = new JObject();
            foreach (object key in target.Keys)
            {
                object value = target[key];
                Encode_Value(key.ToString(),
                     value
                    , child);
            }
            jobj.Add(Name, child);
            //  Debug.WriteLine(jobj.ToString());
        }

        public void Encode_List(string Name, IList vList, JObject jobj)
        {
            Debug.WriteLine("List.Count = ", vList.Count);
            JArray jArray = new JArray();
            foreach (object element in vList)
            {
                if (element == null)
                {
                    jobj.Add(new JObject());
                }
                else
                {
                    JObject child = new JObject();
                    Encode_Value(null, element, child);
                    jArray.Add(child);
                }
            }
            jobj.Add(Name, jArray);
        }
        public void Encode_Value(string Name, Object Value, JObject jobj, int indent = 0)
        {
            if (Value == null)
            {
                log.Info("Value = null, Name=" + Name);
                return;
            }
            indent++;
            OutMsg(indent, "name[ {0} ] , {1} ,", Name, Value.GetType().Name);
            Type ValueType = Value.GetType();
            /*
             *  Simply Name id First
             */
            string stype = ValueType.Name;
            switch (stype)
            {
                case "String":
                    jobj.Add(Name, (string)Value);
                    break;
                case "Boolean":
                    jobj.Add(Name, (bool)Value);
                    break;
                case "Int32":
                    jobj.Add(Name, (int)Value);
                    break;
                case "Int64":
                    jobj.Add(Name, (long)Value);
                    break;
                case "Double":
                    jobj.Add(Name, (double)Value);
                    break;
                case "Single":
                    jobj.Add(Name, (float)Value);
                    break;
                case "Dictionary`2":
                    //   Encode_Dictionary(Name, Value as IDictionary, jobj);
                    break;
                case "ObservableCollection`1":
                case "List`1":
                    Debug.WriteLine("aa");
                    Encode_List(Name, Value as IList, jobj);
                    break;
            }

            if (ValueType.IsArray) // Array : 원소생성, 
            {
                JArray jarray = new JArray();
                Array array = (Array)Value;
                foreach (Object ele in array)
                {
                    if (ele != null)
                    {
                        JObject child = new JObject();
                        Encode_Value(null, ele, child, indent);
                        jarray.Add(child);
                    }
                    else
                    {
                        jarray.Add(new JObject());
                    }

                }
                jobj.Add(Name, jarray);
                return;
            }

            if (ValueType.IsEnum)
            {
                jobj.Add(Name, Value.ToString());
                return;
            }
            if (ValueType.IsClass)
            {
                Encode_Class(Value, jobj, indent);
                return;
            }


        }
        public void Encode_Class(object Target, JObject jobj, int indent = 0)
        {
            indent++;
            Type type = Target.GetType();
            var fields = type.GetProperties().ToList();
            foreach (var field in fields)
            {
                JsonRepoAttribute attribute = field.GetCustomAttribute(typeof(JsonRepoAttribute)) as JsonRepoAttribute;
                if (attribute != null)
                {
                    var Name = attribute.Name;
                    var Value = field.GetValue(Target);
                    if (!IsSingle(Value))
                    {
                        continue;
                    }
                    log.Info(
                         String.Format(
                             "...........................................".Substring(0, indent * 2) +
                             "name {0}, type= {1}", Name, field.PropertyType.Name)
                         );
                    if (attribute.NotNull && Value == null)
                    {
                        Debug.Fail("Jason Null name = " + attribute.Name);
                        continue;
                    }
                    Encode_Value(Name, Value, jobj, indent);
                }
            }
            foreach (var field in fields)
            {
                JsonRepoAttribute attribute = field.GetCustomAttribute(typeof(JsonRepoAttribute)) as JsonRepoAttribute;
                if (attribute != null)
                {
                    var Name = attribute.Name;
                    var Value = field.GetValue(Target);

                    if (IsSingle(Value))
                    {
                        continue;
                    }
                    log.Info(
                         String.Format(
                             "...........................................".Substring(0, indent * 2) +
                             "name {0}, type= {1}", Name, field.Name)
                         );
                    if (attribute.NotNull && Value == null)
                    {
                        Debug.Fail("Jason Null name = " + attribute.Name);
                        continue;
                    }
                    Encode_Value(Name, Value, jobj, indent);
                }
            }
        }

        public bool IsSingle(Object Value)
        {
            if (Value == null) return true;
            switch (Value.GetType().Name)
            {
                case "String":
                case "Int32":
                case "Int64":
                case "Single":
                    return true;
            }

            return false;
        }

        public void OutMsg(int indent, string fmt, params object[] args)
        {
            string spc = "...........................................".Substring(0, indent * 2);
            // Debug.WriteLine(spc + fmt, args);
        }

        // =========================================================================================
        // DECODE
        // =========================================================================================

        //public Object Decode_Value_xx(string Name, Type ValueType, JObject jobj)
        //{
        //    //  Object Value =  Activator.CreateInstance(VType);


        //    string stype = ValueType.Name;
        //    log.Info("293.stype = " + stype);

        //    if (ValueType.IsArray)
        //    {
        //        Type itemType = ValueType.GetElementType();
        //        //  log.Info(String.Format("-> Name='{0}', Type={1}[?]",Name, itemType.Name));
        //        List<Object> list = new List<Object>();
        //        JArray jArray = (JArray)jobj[Name];
        //        Array array = Array.CreateInstance(itemType, jArray.Count);
        //        int Count = 0;
        //        foreach (JObject jele in jArray)
        //        {
        //            log.Info(String.Format("-> Name='{0}'[{1}]", Name, Count));
        //            Object target = Activator.CreateInstance(itemType);
        //            Decode_Class(target, jele);
        //            array.SetValue(target, Count++);
        //        }
        //        return array;
        //    }

        //    if (ValueType.IsClass)
        //    {
        //        switch (stype)
        //        {
        //            case "Dictionary`2":
        //                return Decode_Dictionary(Name, ValueType, jobj);
        //            case "List`1":
        //                return Decode_List(Name, ValueType, jobj);
        //            default:
        //                log.Fatal("stype " + stype);
        //                break;
        //        }

        //    }

        //    {
        //        //string stype = value.GetType().Name;
        //        //if (value.GetType().IsEnum)
        //        //{
        //        //    Debug.WriteLine("  ClassValue: enum"); // elso class
        //        //    jobj.Add(Name, value.ToString());
        //        //    return;
        //        //}
        //        //if (value.GetType().IsClass) // 새로운 클래스
        //        //{
        //        //    Debug.WriteLine("  ClassValue: " + stype);
        //        //    switch (stype)
        //        //    {
        //        //        case "String":
        //        //            jobj.Add(Name, (string)value);
        //        //            break;
        //        //        case "Boolean":
        //        //            jobj.Add(Name, (bool)value);
        //        //            break;
        //        //        case "Double":
        //        //            jobj.Add(Name, (double)value);
        //        //            break;
        //        //        case "Dictionary`2":
        //        //            Encode_Dictionary(Name, value as IDictionary, jobj);
        //        //            break;
        //        //        default:
        //        //            Encode_Class(Name, value, jobj);
        //        //            break;
        //        //    }
        //        //}
        //    }

        //    return null;
        //}




        //public Object Decode_List(string Name, Type ValueType, JObject jobj)
        //{
        //    Type eType = ValueType.GetGenericArguments()[0];
        //    IList DataList = Activator.CreateInstance(ValueType) as IList;
        //    JArray jArray = (JArray)jobj[Name];
        //    foreach (JValue jele in jArray)
        //    {
        //        Object value = Decode_Value3(eType, jele);
        //        DataList.Add(value);
        //    }
        //    return DataList;
        //}

        public Object Decode_Dictionary(string Name, Type ValueType, JObject jobj, int indent)
        {
            Type keyType = ValueType.GetGenericArguments()[0];
            Type eType = ValueType.GetGenericArguments()[1];
            IDictionary PreDic = Activator.CreateInstance(ValueType) as IDictionary;
            JObject child = (JObject)jobj[Name];
            if (child != null)
            {
                foreach (var token in child)
                {
                    string name = token.Key;
                    JToken JValue = token.Value;
                    Object KeyValue = Decode_Value3(ValueType, jobj[Name], null, indent);
                    Object eValue = Activator.CreateInstance(eType, name);
                    Decode_Class(eValue, (JObject)JValue);
                    PreDic[name] = eValue;
                }
            }
            return PreDic;
        }

        public object Decode_Value3(Type ValueType, JToken jobj, Object gValue, int indent)
        {
            int Count = 0;
            indent++;
            if (ValueType.IsClass)
            {
                if (ValueType.IsArray) // Array : 원소생성, 
                {
                    try
                    {
                        Type itemType = ValueType.GetElementType();
                        Array array = gValue == null ? Array.CreateInstance(itemType, ((JArray)jobj).Count) : (Array)gValue;

                        // 배열내용체우기
                        Count = 0;
                        foreach (JObject jele in (JArray)jobj)
                        {
                            Object eValue = array.GetValue(Count);
                            if (eValue == null)
                            {
                                Debug.WriteLine("[Cretae.itemType ]= " + itemType.Name);
                                eValue = Activator.CreateInstance(itemType);
                                Decode_Class(eValue, jele, indent);

                                array.SetValue(eValue, Count);
                            }
                            else
                            {
                                Decode_Class(eValue, jele, indent);
                            }
                            Count++; // must
                        }
                        return array;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Decode_Array ex = " + ex.Message);
                        Debug.WriteLine("Decode_Array ex = " + ex.StackTrace);
                    }
                }
                OutMsg(indent, "-2  {0} ", ValueType.Name);
                switch (ValueType.Name)
                {
                    case "String":
                        return (string)jobj;
                    case "Int32":
                        return (int)jobj;
                    //case "Dictionary`2":
                    //    return Decode_Dictionary(Name, ValueType, jobj);
                    case "ObservableCollection`1":
                    case "List`1":
                        Type eType = ValueType.GetGenericArguments()[0];
                        Debug.WriteLine("EE == List." + eType.Name);
                        IList DataList = Activator.CreateInstance(ValueType) as IList;
                        JArray jArray = (JArray)jobj;
                        foreach (JObject jele in jArray)
                        {
                            Object value = Decode_Value3(eType, jele, null, indent);
                            DataList.Add(value);
                        }
                        return DataList;
                }
                {  // When Class
                   //  Debug.WriteLine("Create ValueObject " + ValueType.FullName);
                    OutMsg(indent, "-3  {0} ", ValueType.Name);
                    Object Value = Activator.CreateInstance(ValueType);
                    Decode_Class(Value, (JObject)jobj);
                    return Value;
                }

            }
            else
            {
                string sname = ValueType.Name;
                switch (sname) // non class
                {
                    case "Boolean":
                        return (bool)jobj;
                    case "Single":
                        return (float)jobj;
                    case "Double":
                        return (double)jobj;
                    case "Int32":
                        return (int)jobj;
                    case "Int64":
                        return (long)jobj;
                    default:
                        throw new Exception("JsonDecoder Type1=" + sname);
                }
            }
        }
        public void Decode_Class(Object Target, JObject jobj, int indent = 0)
        {
            Type type = Target.GetType();
            var fields = type.GetProperties().ToList();
            indent++;
            foreach (var field in fields)
            {
                JsonRepoAttribute attribute = field.GetCustomAttribute(typeof(JsonRepoAttribute)) as JsonRepoAttribute;
                if (attribute != null)
                {
                    var Name = attribute.Name;


                    //log.Info(String.Format(
                    //    "## JSON.ENCODE {0} <== {1}  ({2})  ", Name, "val", field.PropertyType.FullName));
                    // 우선 타입을 알아야 한다.
                    Type ValueType = field.PropertyType;
                    if (jobj.ContainsKey(Name))
                    {
                        Object Value = field.GetValue(Target);
                        Value = Decode_Value3(ValueType, jobj[Name], Value, indent);
                        if (Value != null)
                        {
                            field.SetValue(Target, Value);
                            OutMsg(indent, "{0} = {1} ", Name, Value.GetType().Name);
                        }
                        else
                        {
                            OutMsg(indent, "{0} = null  expect={1}", Name, field.PropertyType.Name);
                        }





                    }
                }
            }
        }

        static public void Save(string Name, Object Target, string FileName)
        {
            string path = @"C:/DIT/Vision_Controller/";
            JObject jRoot = new JObject();
            JsonAnot x = new JsonAnot();
            x.Encode_Value(Name, Target, jRoot);
            File.WriteAllText(path + FileName, jRoot.ToString());
        }
        static public Object Load(string Name, Type ValueType, string FileName)
        {
            Logger log = Logger.GetLogger("Json");
            try
            {
                string TextContent;
                using (StreamReader sr = new System.IO.StreamReader(FileName))
                {
                    TextContent = sr.ReadToEnd();
                    sr.Close();
                }
                JObject jRoot = JObject.Parse(TextContent);
                JsonAnot x = new JsonAnot();
                log.Info("Load " + FileName);
                log.Info(jRoot.ToString());
                return x.Decode_Value3(ValueType, jRoot[Name], null, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("JsonAnno.Load::" + ex.ToString());
            }

            return null;
        }
        internal static void Save3(Object Target, string FileName)
        {
            Logger log = Logger.GetLogger("Json");
            JsonAnot Converter = new JsonAnot();
            JObject jRoot = new JObject();
            Converter.Encode_Class(Target, jRoot);
            File.WriteAllText(FileName, jRoot.ToString());
        }

        internal static void Load3(Object Target, string FileName)
        {
            try
            {
                Logger log = Logger.GetLogger("Json");
                string TextContent;
                using (StreamReader sr = new System.IO.StreamReader(FileName))
                {
                    TextContent = sr.ReadToEnd();
                    sr.Close();
                }
                JObject jRoot = JObject.Parse(TextContent);
                JsonAnot Converter = new JsonAnot();
                log.Info("Load " + FileName);
                log.Info(jRoot.ToString());
                Converter.Decode_Class(Target, jRoot);
            }
            catch (Exception ex)
            {
                Debug.Fail("JsonAnno.Load::" + ex.ToString());
            }

        }




    }

    //    if (ValueType.IsEnum)
    //    {
    //        var sValue = (string)jobj[Name];
    //        if (sValue != null && sValue.Length > 0)
    //        {
    //            Object value = Enum.Parse(field.PropertyType, sValue);
    //            field.SetValue(Target, value);
    //        }
    //        continue;

}
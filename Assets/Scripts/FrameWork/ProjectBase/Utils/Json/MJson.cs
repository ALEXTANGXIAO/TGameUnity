using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class JsonData : IJsonWrapper,IList, IOrderedDictionary, IDictionary, ICollection, IEnumerable, IEquatable<JsonData>
{
    private IList<JsonData> inst_array;
    private bool inst_boolean;
    private double inst_double;
    private int inst_int;
    private long inst_long;
    private IDictionary<string, JsonData> inst_object;
    private string inst_string;
    private string json;
    private IList<KeyValuePair<string, JsonData>> object_list;
    private JsonType type;


    public JsonData Deserialize(string json)
    {
        if (json == null)
        {
            return null;
        }

        var jsonData = JsonMapper.ToObject(json);

        return jsonData;
    }

    public string Serialize(JsonData jsonData)
    {
        if (jsonData.json == null)
        {
            StringWriter writer = new StringWriter();
            JsonWriter writer2 = new JsonWriter(writer);
            writer2.Validate = false;
            WriteJson(this, writer2);
            this.json = writer.ToString();
        }
        return this.json;
    }

    /// <summary>转换为list JsonData </summary>
    public List<JsonData> ToArrayJsonData()
    {
        List<JsonData> array = new List<JsonData>();
        try
        {
            for (int i = 0; i < this.Count; i++)
            {
                array.Add(this[i]);
            }
        }
        catch (Exception)
        {
            Debug.LogError("非list<int>类型错误");
        }
        return array;
    }
    /// <summary>转换为list Int </summary>
    public List<int> ToArrayInt()
    {
        List<int> array = new List<int>();
        try
        {
            for (int i = 0; i < this.Count; i++)
            {
                array.Add((int)this[i]);
            }
        }
        catch (Exception)
        {
            Debug.LogError("非list<int>类型错误");
        }
        return array;
    }
    /// <summary>转换为list String </summary>
    public List<string> ToArrayString()
    {
        List<string> array = new List<string>();
        try
        {
            for (int i = 0; i < this.Count; i++)
            {
                array.Add(this[i].ToString());
            }
        }
        catch (Exception)
        {
            Debug.LogError("非list<string>类型错误");
        }
        return array;
    }
    // Methods
    public JsonData()
    {
    }

    public JsonData(bool boolean)
    {
        this.type = JsonType.Boolean;
        this.inst_boolean = boolean;
    }

    public JsonData(double number)
    {
        this.type = JsonType.Double;
        this.inst_double = number;
    }

    public JsonData(int number)
    {
        this.type = JsonType.Int;
        this.inst_int = number;
    }

    public JsonData(long number)
    {
        this.type = JsonType.Long;
        this.inst_long = number;
    }

    public JsonData(object obj)
    {
        if (obj is bool)
        {
            this.type = JsonType.Boolean;
            this.inst_boolean = (bool)obj;
        }
        else if (obj is double)
        {
            this.type = JsonType.Double;
            this.inst_double = (double)obj;
        }
        else if (obj is int)
        {
            this.type = JsonType.Int;
            this.inst_int = (int)obj;
        }
        else if (obj is long)
        {
            this.type = JsonType.Long;
            this.inst_long = (long)obj;
        }
        else
        {
            if (!(obj is string))
            {
                throw new ArgumentException("Unable to wrap the given object with JsonData");
            }
            this.type = JsonType.String;
            this.inst_string = (string)obj;
        }
    }

    public JsonData(string str)
    {
        this.type = JsonType.String;
        this.inst_string = str;
    }

    public int Add(object value)
    {
        JsonData data = this.ToJsonData(value);
        this.json = null;
        return this.EnsureList().Add(data);
    }

    public void Clear()
    {
        if (this.IsObject)
        {
            ((IDictionary)this).Clear();
        }
        else if (this.IsArray)
        {
            ((IList)this).Clear();
        }
    }

    private ICollection EnsureCollection()
    {
        if (this.type == JsonType.Array)
        {
            return (ICollection)this.inst_array;
        }
        if (this.type != JsonType.Object)
        {
            throw new InvalidOperationException("The JsonData instance has to be initialized first");
        }
        return (ICollection)this.inst_object;
    }

    private IDictionary EnsureDictionary()
    {
        if (this.type != JsonType.Object)
        {
            if (this.type != JsonType.None)
            {
                throw new InvalidOperationException("Instance of JsonData is not a dictionary");
            }
            this.type = JsonType.Object;
            this.inst_object = new Dictionary<string, JsonData>();
            this.object_list = new List<KeyValuePair<string, JsonData>>();
        }
        return (IDictionary)this.inst_object;
    }

    private IList EnsureList()
    {
        if (this.type != JsonType.Array)
        {
            if (this.type != JsonType.None)
            {
                throw new InvalidOperationException("Instance of JsonData is not a list");
            }
            this.type = JsonType.Array;
            this.inst_array = new List<JsonData>();
        }
        return (IList)this.inst_array;
    }

    public bool Equals(JsonData x)
    {
        if (x != null)
        {
            if (x.type != this.type)
            {
                return false;
            }
            switch (this.type)
            {
                case JsonType.None:
                    return true;

                case JsonType.Object:
                    return this.inst_object.Equals(x.inst_object);

                case JsonType.Array:
                    return this.inst_array.Equals(x.inst_array);

                case JsonType.String:
                    return this.inst_string.Equals(x.inst_string);

                case JsonType.Int:
                    return this.inst_int.Equals(x.inst_int);

                case JsonType.Long:
                    return this.inst_long.Equals(x.inst_long);

                case JsonType.Double:
                    return this.inst_double.Equals(x.inst_double);

                case JsonType.Boolean:
                    return this.inst_boolean.Equals(x.inst_boolean);
            }
        }
        return false;
    }

    public JsonType GetJsonType()
    {
        return this.type;
    }

    bool IJsonWrapper.GetBoolean()
    {
        if (this.type != JsonType.Boolean)
        {
            throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
        }
        return this.inst_boolean;
    }

    double IJsonWrapper.GetDouble()
    {
        if (this.type != JsonType.Double)
        {
            throw new InvalidOperationException("JsonData instance doesn't hold a double");
        }
        return this.inst_double;
    }

    int IJsonWrapper.GetInt()
    {
        if (this.type != JsonType.Int)
        {
            throw new InvalidOperationException("JsonData instance doesn't hold an int");
        }
        return this.inst_int;
    }

    long IJsonWrapper.GetLong()
    {
        if (this.type != JsonType.Long)
        {
            throw new InvalidOperationException("JsonData instance doesn't hold a long");
        }
        return this.inst_long;
    }

    string IJsonWrapper.GetString()
    {
        if (this.type != JsonType.String)
        {
            throw new InvalidOperationException("JsonData instance doesn't hold a string");
        }
        return this.inst_string;
    }

    void IJsonWrapper.SetBoolean(bool val)
    {
        this.type = JsonType.Boolean;
        this.inst_boolean = val;
        this.json = null;
    }

    void IJsonWrapper.SetDouble(double val)
    {
        this.type = JsonType.Double;
        this.inst_double = val;
        this.json = null;
    }

    void IJsonWrapper.SetInt(int val)
    {
        this.type = JsonType.Int;
        this.inst_double = this.inst_long = this.inst_int = val;
        this.json = null;
    }

    void IJsonWrapper.SetLong(long val)
    {
        this.type = JsonType.Long;
        this.inst_double = this.inst_long = val;
        this.json = null;
    }

    void IJsonWrapper.SetString(string val)
    {
        this.type = JsonType.String;
        this.inst_string = val;
        this.json = null;
    }

    string IJsonWrapper.ToJson()
    {
        return this.ToJson();
    }

    void IJsonWrapper.ToJson(JsonWriter writer)
    {
        this.ToJson(writer);
    }

    public static explicit operator bool(JsonData data)
    {
        if (data.type != JsonType.Boolean)
        {
            throw new InvalidCastException("Instance of JsonData doesn't hold a double");
        }
        return data.inst_boolean;
    }

    public static explicit operator double(JsonData data)
    {
        if (((data.type != JsonType.Double) && (data.type != JsonType.Int)) && (data.type != JsonType.Long))
        {
            throw new InvalidCastException("Instance of JsonData doesn't hold a double");
        }
        return data.inst_double;
    }

    public static explicit operator int(JsonData data)
    {
        if (data.type != JsonType.Int)
        {
            throw new InvalidCastException("Instance of JsonData doesn't hold an int");
        }
        return data.inst_int;
    }

    public static explicit operator long(JsonData data)
    {
        if ((data.type != JsonType.Long) && (data.type != JsonType.Int))
        {
            throw new InvalidCastException("Instance of JsonData doesn't hold an int");
        }
        return data.inst_long;
    }

    public static explicit operator string(JsonData data)
    {
        if (data.type != JsonType.String)
        {
            throw new InvalidCastException("Instance of JsonData doesn't hold a string");
        }
        return data.inst_string;
    }

    public static implicit operator JsonData(bool data)
    {
        return new JsonData(data);
    }

    public static implicit operator JsonData(double data)
    {
        return new JsonData(data);
    }

    public static implicit operator JsonData(int data)
    {
        return new JsonData(data);
    }

    public static implicit operator JsonData(long data)
    {
        return new JsonData(data);
    }

    public static implicit operator JsonData(string data)
    {
        return new JsonData(data);
    }

    public void SetJsonType(JsonType type)
    {
        if (this.type != type)
        {
            switch (type)
            {
                case JsonType.Object:
                    this.inst_object = new Dictionary<string, JsonData>();
                    this.object_list = new List<KeyValuePair<string, JsonData>>();
                    break;

                case JsonType.Array:
                    this.inst_array = new List<JsonData>();
                    break;

                case JsonType.String:
                    this.inst_string = null;
                    break;

                case JsonType.Int:
                    this.inst_int = 0;
                    break;

                case JsonType.Long:
                    this.inst_long = 0L;
                    break;

                case JsonType.Double:
                    this.inst_double = 0.0;
                    break;

                case JsonType.Boolean:
                    this.inst_boolean = false;
                    break;
            }
            this.type = type;
        }
    }

    void ICollection.CopyTo(Array array, int index)
    {
        this.EnsureCollection().CopyTo(array, index);
    }

    void IDictionary.Add(object key, object value)
    {
        JsonData data = this.ToJsonData(value);
        this.EnsureDictionary().Add(key, data);
        KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, data);
        this.object_list.Add(item);
        this.json = null;
    }

    void IDictionary.Clear()
    {
        this.EnsureDictionary().Clear();
        this.object_list.Clear();
        this.json = null;
    }

    bool IDictionary.Contains(object key)
    {
        return this.EnsureDictionary().Contains(key);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IOrderedDictionary)this).GetEnumerator();
    }

    void IDictionary.Remove(object key)
    {
        this.EnsureDictionary().Remove(key);
        for (int i = 0; i < this.object_list.Count; i++)
        {
            KeyValuePair<string, JsonData> pair = this.object_list[i];
            if (pair.Key == ((string)key))
            {
                this.object_list.RemoveAt(i);
                break;
            }
        }
        this.json = null;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.EnsureCollection().GetEnumerator();
    }

    int IList.Add(object value)
    {
        return this.Add(value);
    }

    void IList.Clear()
    {
        this.EnsureList().Clear();
        this.json = null;
    }

    bool IList.Contains(object value)
    {
        return this.EnsureList().Contains(value);
    }

    int IList.IndexOf(object value)
    {
        return this.EnsureList().IndexOf(value);
    }

    void IList.Insert(int index, object value)
    {
        this.EnsureList().Insert(index, value);
        this.json = null;
    }

    void IList.Remove(object value)
    {
        this.EnsureList().Remove(value);
        this.json = null;
    }

    void IList.RemoveAt(int index)
    {
        this.EnsureList().RemoveAt(index);
        this.json = null;
    }

    IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
    {
        this.EnsureDictionary();
        return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
    }

    void IOrderedDictionary.Insert(int idx, object key, object value)
    {
        string str = (string)key;
        JsonData data = this.ToJsonData(value);
        this[str] = data;
        KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(str, data);
        this.object_list.Insert(idx, item);
    }

    void IOrderedDictionary.RemoveAt(int idx)
    {
        this.EnsureDictionary();
        KeyValuePair<string, JsonData> pair = this.object_list[idx];
        this.inst_object.Remove(pair.Key);
        this.object_list.RemoveAt(idx);
    }

    public string ToJson()
    {
        if (this.json == null)
        {
            StringWriter writer = new StringWriter();
            JsonWriter writer2 = new JsonWriter(writer);
            writer2.Validate = false;
            WriteJson(this, writer2);
            this.json = writer.ToString();
        }
        return this.json;
    }

    public void ToJson(JsonWriter writer)
    {
        bool validate = writer.Validate;
        writer.Validate = false;
        WriteJson(this, writer);
        writer.Validate = validate;
    }

    private JsonData ToJsonData(object obj)
    {
        if (obj == null)
        {
            return null;
        }
        if (obj is JsonData)
        {
            return (JsonData)obj;
        }
        return new JsonData(obj);
    }

    /// <summary>return jsont type，is not json to string,please use ToJson() 返回的是type，不是json转换string </summary>
    public override string ToString()
    {
        //   Debug.Log("jsondata.tostring    :" + this.ToJson());
        switch (this.type)
        {
            case JsonType.Object:
                return "JsonData object";

            case JsonType.Array:
                return "JsonData array";

            case JsonType.String:
                return this.inst_string;

            case JsonType.Int:
                return this.inst_int.ToString();

            case JsonType.Long:
                return this.inst_long.ToString();

            case JsonType.Double:
                return this.inst_double.ToString();

            case JsonType.Boolean:
                return this.inst_boolean.ToString();
        }
        return "Uninitialized JsonData";
    }

    private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
    {
        if (obj.IsString)
        {
            writer.Write(obj.GetString());
        }
        else if (obj.IsBoolean)
        {
            writer.Write(obj.GetBoolean());
        }
        else if (obj.IsDouble)
        {
            writer.Write(obj.GetDouble());
        }
        else if (obj.IsInt)
        {
            writer.Write(obj.GetInt());
        }
        else if (obj.IsLong)
        {
            writer.Write(obj.GetLong());
        }
        else if (obj.IsArray)
        {
            writer.WriteArrayStart();
            foreach (object obj2 in obj)
            {
                WriteJson((JsonData)obj2, writer);
            }
            writer.WriteArrayEnd();
        }
        else if (obj.IsObject)
        {
            writer.WriteObjectStart();
            foreach (DictionaryEntry entry in obj)
            {
                writer.WritePropertyName((string)entry.Key);
                WriteJson((JsonData)entry.Value, writer);
            }
            writer.WriteObjectEnd();
        }
    }

    // Properties
    public int Count
    {
        get
        {
            return this.EnsureCollection().Count;
        }
    }

    public IDictionary<string, JsonData> Inst_Object
    {
        get
        {
            if (this.type == JsonType.Object)
            {
                return this.inst_object;
            }
            return null;
        }
    }

    public bool IsArray
    {
        get
        {
            return (this.type == JsonType.Array);
        }
    }

    public bool IsBoolean
    {
        get
        {
            return (this.type == JsonType.Boolean);
        }
    }

    public bool IsDouble
    {
        get
        {
            return (this.type == JsonType.Double);
        }
    }

    public bool IsInt
    {
        get
        {
            return (this.type == JsonType.Int);
        }
    }

    public bool IsLong
    {
        get
        {
            return (this.type == JsonType.Long);
        }
    }

    public bool IsObject
    {
        get
        {
            return (this.type == JsonType.Object);
        }
    }

    public bool IsString
    {
        get
        {
            return (this.type == JsonType.String);
        }
    }

    public JsonData this[int index]
    {
        get
        {
            this.EnsureCollection();
            if (this.type == JsonType.Array)
            {
                return this.inst_array[index];
            }
            KeyValuePair<string, JsonData> pair = this.object_list[index];
            return pair.Value;
        }
        set
        {
            this.EnsureCollection();
            if (this.type == JsonType.Array)
            {
                this.inst_array[index] = value;
            }
            else
            {
                KeyValuePair<string, JsonData> pair = this.object_list[index];
                KeyValuePair<string, JsonData> pair2 = new KeyValuePair<string, JsonData>(pair.Key, value);
                this.object_list[index] = pair2;
                this.inst_object[pair.Key] = value;
            }
            this.json = null;
        }
    }

    public JsonData this[string prop_name]
    {
        get
        {
            this.EnsureDictionary();
            return this.inst_object[prop_name];
        }
        set
        {
            this.EnsureDictionary();
            KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(prop_name, value);
            if (this.inst_object.ContainsKey(prop_name))
            {
                for (int i = 0; i < this.object_list.Count; i++)
                {
                    KeyValuePair<string, JsonData> pair2 = this.object_list[i];
                    if (pair2.Key == prop_name)
                    {
                        this.object_list[i] = item;
                        break;
                    }
                }
            }
            else
            {
                this.object_list.Add(item);
            }
            this.inst_object[prop_name] = value;
            this.json = null;
        }
    }

    bool IJsonWrapper.IsArray
    {
        get
        {
            return this.IsArray;
        }
    }

    bool IJsonWrapper.IsBoolean
    {
        get
        {
            return this.IsBoolean;
        }
    }

    bool IJsonWrapper.IsDouble
    {
        get
        {
            return this.IsDouble;
        }
    }

    bool IJsonWrapper.IsInt
    {
        get
        {
            return this.IsInt;
        }
    }

    bool IJsonWrapper.IsLong
    {
        get
        {
            return this.IsLong;
        }
    }

    bool IJsonWrapper.IsObject
    {
        get
        {
            return this.IsObject;
        }
    }

    bool IJsonWrapper.IsString
    {
        get
        {
            return this.IsString;
        }
    }

    int ICollection.Count
    {
        get
        {
            return this.Count;
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            return this.EnsureCollection().IsSynchronized;
        }
    }

    object ICollection.SyncRoot
    {
        get
        {
            return this.EnsureCollection().SyncRoot;
        }
    }

    bool IDictionary.IsFixedSize
    {
        get
        {
            return this.EnsureDictionary().IsFixedSize;
        }
    }

    bool IDictionary.IsReadOnly
    {
        get
        {
            return this.EnsureDictionary().IsReadOnly;
        }
    }

    object IDictionary.this[object key]
    {
        get
        {
            return this.EnsureDictionary()[key];
        }
        set
        {
            if (!(key is string))
            {
                throw new ArgumentException("The key has to be a string");
            }
            JsonData data = this.ToJsonData(value);
            this[(string)key] = data;
        }
    }

    ICollection IDictionary.Keys
    {
        get
        {
            this.EnsureDictionary();
            IList<string> list = new List<string>();
            foreach (KeyValuePair<string, JsonData> pair in this.object_list)
            {
                list.Add(pair.Key);
            }
            return (ICollection)list;
        }
    }

    ICollection IDictionary.Values
    {
        get
        {
            this.EnsureDictionary();
            IList<JsonData> list = new List<JsonData>();
            foreach (KeyValuePair<string, JsonData> pair in this.object_list)
            {
                list.Add(pair.Value);
            }
            return (ICollection)list;
        }
    }

    bool IList.IsFixedSize
    {
        get
        {
            return this.EnsureList().IsFixedSize;
        }
    }

    bool IList.IsReadOnly
    {
        get
        {
            return this.EnsureList().IsReadOnly;
        }
    }

    object IList.this[int index]
    {
        get
        {
            return this.EnsureList()[index];
        }
        set
        {
            this.EnsureList();
            JsonData data = this.ToJsonData(value);
            this[index] = data;
        }
    }

    object IOrderedDictionary.this[int idx]
    {
        get
        {
            this.EnsureDictionary();
            KeyValuePair<string, JsonData> pair = this.object_list[idx];
            return pair.Value;
        }
        set
        {
            this.EnsureDictionary();
            JsonData data = this.ToJsonData(value);
            KeyValuePair<string, JsonData> pair = this.object_list[idx];
            this.inst_object[pair.Key] = data;
            KeyValuePair<string, JsonData> pair2 = new KeyValuePair<string, JsonData>(pair.Key, data);
            this.object_list[idx] = pair2;
        }
    }
}

internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
{
    // Fields
    private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;

    // Methods
    public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
    {
        this.list_enumerator = enumerator;
    }

    public bool MoveNext()
    {
        return this.list_enumerator.MoveNext();
    }

    public void Reset()
    {
        this.list_enumerator.Reset();
    }

    // Properties
    public object Current
    {
        get
        {
            return this.Entry;
        }
    }

    public DictionaryEntry Entry
    {
        get
        {
            KeyValuePair<string, JsonData> current = this.list_enumerator.Current;
            return new DictionaryEntry(current.Key, current.Value);
        }
    }

    public object Key
    {
        get
        {
            return this.list_enumerator.Current.Key;
        }
    }

    public object Value
    {
        get
        {
            return this.list_enumerator.Current.Value;
        }
    }
}

public interface IJsonWrapper : IList, IOrderedDictionary, IDictionary, ICollection, IEnumerable
{
    // Methods
    bool GetBoolean();
    double GetDouble();
    int GetInt();
    JsonType GetJsonType();
    long GetLong();
    string GetString();
    void SetBoolean(bool val);
    void SetDouble(double val);
    void SetInt(int val);
    void SetJsonType(JsonType type);
    void SetLong(long val);
    void SetString(string val);
    string ToJson();
    void ToJson(JsonWriter writer);

    // Properties
    bool IsArray { get; }
    bool IsBoolean { get; }
    bool IsDouble { get; }
    bool IsInt { get; }
    bool IsLong { get; }
    bool IsObject { get; }
    bool IsString { get; }
}

public enum JsonToken
{
    None,
    ObjectStart,
    PropertyName,
    ObjectEnd,
    ArrayStart,
    ArrayEnd,
    Int,
    Long,
    Double,
    String,
    Boolean,
    Null
}

public enum JsonType
{
    None,
    Object,
    Array,
    String,
    Int,
    Long,
    Double,
    Boolean
}
internal enum Condition
{
    InArray,
    InObject,
    NotAProperty,
    Property,
    Value
}

public class JsonWriter
{
    // Fields
    private WriterContext context;
    private Stack<WriterContext> ctx_stack;
    private bool has_reached_end;
    private char[] hex_seq;
    private int indent_value;
    private int indentation;
    private StringBuilder inst_string_builder;
    private static NumberFormatInfo number_format = NumberFormatInfo.InvariantInfo;
    private bool pretty_print;
    private bool validate;
    private TextWriter writer;

    // Methods
    public JsonWriter()
    {
        this.inst_string_builder = new StringBuilder();
        this.writer = new StringWriter(this.inst_string_builder);
        this.Init();
    }

    public JsonWriter(TextWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException("writer");
        }
        this.writer = writer;
        this.Init();
    }

    public JsonWriter(StringBuilder sb) : this(new StringWriter(sb))
    {
    }

    private void DoValidation(Condition cond)
    {
        if (!this.context.ExpectingValue)
        {
            this.context.Count++;
        }
        if (this.validate)
        {
            if (this.has_reached_end)
            {
                throw new JsonException("A complete JSON symbol has already been written");
            }
            switch (cond)
            {
                case Condition.InArray:
                    if (!this.context.InArray)
                    {
                        throw new JsonException("Can't close an array here");
                    }
                    return;

                case Condition.InObject:
                    if (!this.context.InObject || this.context.ExpectingValue)
                    {
                        throw new JsonException("Can't close an object here");
                    }
                    return;

                case Condition.NotAProperty:
                    if (this.context.InObject && !this.context.ExpectingValue)
                    {
                        throw new JsonException("Expected a property");
                    }
                    return;

                case Condition.Property:
                    if (!this.context.InObject || this.context.ExpectingValue)
                    {
                        throw new JsonException("Can't add a property here");
                    }
                    return;

                case Condition.Value:
                    if (!this.context.InArray && (!this.context.InObject || !this.context.ExpectingValue))
                    {
                        throw new JsonException("Can't add a value here");
                    }
                    return;
            }
        }
    }

    private void Indent()
    {
        if (this.pretty_print)
        {
            this.indentation += this.indent_value;
        }
    }

    private void Init()
    {
        this.has_reached_end = false;
        this.hex_seq = new char[4];
        this.indentation = 0;
        this.indent_value = 4;
        this.pretty_print = false;
        this.validate = true;
        this.ctx_stack = new Stack<WriterContext>();
        this.context = new WriterContext();
        this.ctx_stack.Push(this.context);
    }

    private static void IntToHex(int n, char[] hex)
    {
        for (int i = 0; i < 4; i++)
        {
            int num = n % 0x10;
            if (num < 10)
            {
                hex[3 - i] = (char)(0x30 + num);
            }
            else
            {
                hex[3 - i] = (char)(0x41 + (num - 10));
            }
            n = n >> 4;
        }
    }

    private void Put(string str)
    {
        if (this.pretty_print && !this.context.ExpectingValue)
        {
            for (int i = 0; i < this.indentation; i++)
            {
                this.writer.Write(' ');
            }
        }
        this.writer.Write(str);
    }

    private void PutNewline()
    {
        this.PutNewline(true);
    }

    private void PutNewline(bool add_comma)
    {
        if ((add_comma && !this.context.ExpectingValue) && (this.context.Count > 1))
        {
            this.writer.Write(',');
        }
        if (this.pretty_print && !this.context.ExpectingValue)
        {
            this.writer.Write('\n');
        }
    }

    private void PutString(string str)
    {
        this.Put(string.Empty);
        this.writer.Write('"');
        int length = str.Length;
        for (int i = 0; i < length; i++)
        {
            switch (str[i])
            {
                case '\b':
                    {
                        this.writer.Write(@"\b");
                        continue;
                    }
                case '\t':
                    {
                        this.writer.Write(@"\t");
                        continue;
                    }
                case '\n':
                    {
                        this.writer.Write(@"\n");
                        continue;
                    }
                case '\f':
                    {
                        this.writer.Write(@"\f");
                        continue;
                    }
                case '\r':
                    {
                        this.writer.Write(@"\r");
                        continue;
                    }
                case '"':
                case '\\':
                    {
                        this.writer.Write('\\');
                        this.writer.Write(str[i]);
                        continue;
                    }
            }
            this.writer.Write(str[i]);
        }
        this.writer.Write('"');
    }

    public void Reset()
    {
        this.has_reached_end = false;
        this.ctx_stack.Clear();
        this.context = new WriterContext();
        this.ctx_stack.Push(this.context);
        if (this.inst_string_builder != null)
        {
            this.inst_string_builder.Remove(0, this.inst_string_builder.Length);
        }
    }

    public override string ToString()
    {
        if (this.inst_string_builder == null)
        {
            return string.Empty;
        }
        return this.inst_string_builder.ToString();
    }

    private void Unindent()
    {
        if (this.pretty_print)
        {
            this.indentation -= this.indent_value;
        }
    }

    public void Write(bool boolean)
    {
        this.DoValidation(Condition.Value);
        this.PutNewline();
        this.Put(boolean ? "true" : "false");
        this.context.ExpectingValue = false;
    }

    public void Write(decimal number)
    {
        this.DoValidation(Condition.Value);
        this.PutNewline();
        this.Put(Convert.ToString(number, number_format));
        this.context.ExpectingValue = false;
    }

    public void Write(double number)
    {
        this.DoValidation(Condition.Value);
        this.PutNewline();
        string str = Convert.ToString(number, number_format);
        this.Put(str);
        if ((str.IndexOf('.') == -1) && (str.IndexOf('E') == -1))
        {
            this.writer.Write(".0");
        }
        this.context.ExpectingValue = false;
    }

    public void Write(int number)
    {
        this.DoValidation(Condition.Value);
        this.PutNewline();
        this.Put(Convert.ToString(number, number_format));
        this.context.ExpectingValue = false;
    }

    public void Write(long number)
    {
        this.DoValidation(Condition.Value);
        this.PutNewline();
        this.Put(Convert.ToString(number, number_format));
        this.context.ExpectingValue = false;
    }

    public void Write(string str)
    {
        this.DoValidation(Condition.Value);
        this.PutNewline();
        if (str == null)
        {
            this.Put("null");
        }
        else
        {
            this.PutString(str);
        }
        this.context.ExpectingValue = false;
    }

    [CLSCompliant(false)]
    public void Write(ulong number)
    {
        this.DoValidation(Condition.Value);
        this.PutNewline();
        this.Put(Convert.ToString(number, number_format));
        this.context.ExpectingValue = false;
    }

    public void WriteArrayEnd()
    {
        this.DoValidation(Condition.InArray);
        this.PutNewline(false);
        this.ctx_stack.Pop();
        if (this.ctx_stack.Count == 1)
        {
            this.has_reached_end = true;
        }
        else
        {
            this.context = this.ctx_stack.Peek();
            this.context.ExpectingValue = false;
        }
        this.Unindent();
        this.Put("]");
    }

    public void WriteArrayStart()
    {
        this.DoValidation(Condition.NotAProperty);
        this.PutNewline();
        this.Put("[");
        this.context = new WriterContext();
        this.context.InArray = true;
        this.ctx_stack.Push(this.context);
        this.Indent();
    }

    public void WriteObjectEnd()
    {
        this.DoValidation(Condition.InObject);
        this.PutNewline(false);
        this.ctx_stack.Pop();
        if (this.ctx_stack.Count == 1)
        {
            this.has_reached_end = true;
        }
        else
        {
            this.context = this.ctx_stack.Peek();
            this.context.ExpectingValue = false;
        }
        this.Unindent();
        this.Put("}");
    }

    public void WriteObjectStart()
    {
        this.DoValidation(Condition.NotAProperty);
        this.PutNewline();
        this.Put("{");
        this.context = new WriterContext();
        this.context.InObject = true;
        this.ctx_stack.Push(this.context);
        this.Indent();
    }

    public void WritePropertyName(string property_name)
    {
        this.DoValidation(Condition.Property);
        this.PutNewline();
        this.PutString(property_name);
        if (this.pretty_print)
        {
            if (property_name.Length > this.context.Padding)
            {
                this.context.Padding = property_name.Length;
            }
            for (int i = this.context.Padding - property_name.Length; i >= 0; i--)
            {
                this.writer.Write(' ');
            }
            this.writer.Write(": ");
        }
        else
        {
            this.writer.Write(':');
        }
        this.context.ExpectingValue = true;
    }

    // Properties
    public int IndentValue
    {
        get
        {
            return this.indent_value;
        }
        set
        {
            this.indentation = (this.indentation / this.indent_value) * value;
            this.indent_value = value;
        }
    }

    public bool PrettyPrint
    {
        get
        {
            return this.pretty_print;
        }
        set
        {
            this.pretty_print = value;
        }
    }

    public TextWriter TextWriter
    {
        get
        {
            return this.writer;
        }
    }

    public bool Validate
    {
        get
        {
            return this.validate;
        }
        set
        {
            this.validate = value;
        }
    }
}

public class JsonReader
{
    // Fields
    private Stack<int> automaton_stack;
    private int current_input;
    private int current_symbol;
    private bool end_of_input;
    private bool end_of_json;
    private Lexer lexer;
    private static IDictionary<int, IDictionary<int, int[]>> parse_table;
    private bool parser_in_string;
    private bool parser_return;
    private bool read_started;
    private TextReader reader;
    private bool reader_is_owned;
    private JsonToken token;
    private object token_value;

    // Methods
    static JsonReader()
    {
        PopulateParseTable();
    }

    public JsonReader(TextReader reader) : this(reader, false)
    {
    }

    public JsonReader(string json_text) : this(new StringReader(json_text), true)
    {
    }

    private JsonReader(TextReader reader, bool owned)
    {
        if (reader == null)
        {
            throw new ArgumentNullException("reader");
        }
        this.parser_in_string = false;
        this.parser_return = false;
        this.read_started = false;
        this.automaton_stack = new Stack<int>();
        this.automaton_stack.Push(0x10011);
        this.automaton_stack.Push(0x10007);
        this.lexer = new Lexer(reader);
        this.end_of_input = false;
        this.end_of_json = false;
        this.reader = reader;
        this.reader_is_owned = owned;
    }

    public void Close()
    {
        if (!this.end_of_input)
        {
            this.end_of_input = true;
            this.end_of_json = true;
            if (this.reader_is_owned)
            {
                this.reader.Close();
            }
            this.reader = null;
        }
    }

    private static void PopulateParseTable()
    {
        parse_table = new Dictionary<int, IDictionary<int, int[]>>();
        TableAddRow(ParserToken.Array);
        TableAddCol(ParserToken.Array, 0x5b, new int[] { 0x5b, 0x1000d });
        TableAddRow(ParserToken.ArrayPrime);
        TableAddCol(ParserToken.ArrayPrime, 0x22, new int[] { 0x1000e, 0x1000f, 0x5d });
        TableAddCol(ParserToken.ArrayPrime, 0x5b, new int[] { 0x1000e, 0x1000f, 0x5d });
        TableAddCol(ParserToken.ArrayPrime, 0x5d, new int[] { 0x5d });
        TableAddCol(ParserToken.ArrayPrime, 0x7b, new int[] { 0x1000e, 0x1000f, 0x5d });
        TableAddCol(ParserToken.ArrayPrime, 0x10001, new int[] { 0x1000e, 0x1000f, 0x5d });
        TableAddCol(ParserToken.ArrayPrime, 0x10002, new int[] { 0x1000e, 0x1000f, 0x5d });
        TableAddCol(ParserToken.ArrayPrime, 0x10003, new int[] { 0x1000e, 0x1000f, 0x5d });
        TableAddCol(ParserToken.ArrayPrime, 0x10004, new int[] { 0x1000e, 0x1000f, 0x5d });
        TableAddRow(ParserToken.Object);
        TableAddCol(ParserToken.Object, 0x7b, new int[] { 0x7b, 0x10009 });
        TableAddRow(ParserToken.ObjectPrime);
        TableAddCol(ParserToken.ObjectPrime, 0x22, new int[] { 0x1000a, 0x1000b, 0x7d });
        TableAddCol(ParserToken.ObjectPrime, 0x7d, new int[] { 0x7d });
        TableAddRow(ParserToken.Pair);
        TableAddCol(ParserToken.Pair, 0x22, new int[] { 0x10010, 0x3a, 0x1000e });
        TableAddRow(ParserToken.PairRest);
        TableAddCol(ParserToken.PairRest, 0x2c, new int[] { 0x2c, 0x1000a, 0x1000b });
        TableAddCol(ParserToken.PairRest, 0x7d, new int[] { 0x10012 });
        TableAddRow(ParserToken.String);
        TableAddCol(ParserToken.String, 0x22, new int[] { 0x22, 0x10005, 0x22 });
        TableAddRow(ParserToken.Text);
        TableAddCol(ParserToken.Text, 0x5b, new int[] { 0x1000c });
        TableAddCol(ParserToken.Text, 0x7b, new int[] { 0x10008 });
        TableAddRow(ParserToken.Value);
        TableAddCol(ParserToken.Value, 0x22, new int[] { 0x10010 });
        TableAddCol(ParserToken.Value, 0x5b, new int[] { 0x1000c });
        TableAddCol(ParserToken.Value, 0x7b, new int[] { 0x10008 });
        TableAddCol(ParserToken.Value, 0x10001, new int[] { 0x10001 });
        TableAddCol(ParserToken.Value, 0x10002, new int[] { 0x10002 });
        TableAddCol(ParserToken.Value, 0x10003, new int[] { 0x10003 });
        TableAddCol(ParserToken.Value, 0x10004, new int[] { 0x10004 });
        TableAddRow(ParserToken.ValueRest);
        TableAddCol(ParserToken.ValueRest, 0x2c, new int[] { 0x2c, 0x1000e, 0x1000f });
        TableAddCol(ParserToken.ValueRest, 0x5d, new int[] { 0x10012 });
    }

    private void ProcessNumber(string number)
    {
        double num;
        if ((((number.IndexOf('.') != -1) || (number.IndexOf('e') != -1)) || (number.IndexOf('E') != -1)) && double.TryParse(number, out num))
        {
            this.token = JsonToken.Double;
            this.token_value = num;
        }
        else
        {
            int num2;
            if (int.TryParse(number, out num2))
            {
                this.token = JsonToken.Int;
                this.token_value = num2;
            }
            else
            {
                long num3;
                if (long.TryParse(number, out num3))
                {
                    this.token = JsonToken.Long;
                    this.token_value = num3;
                }
                else
                {
                    this.token = JsonToken.Int;
                    this.token_value = 0;
                }
            }
        }
    }

    private void ProcessSymbol()
    {
        if (this.current_symbol == 0x5b)
        {
            this.token = JsonToken.ArrayStart;
            this.parser_return = true;
        }
        else if (this.current_symbol == 0x5d)
        {
            this.token = JsonToken.ArrayEnd;
            this.parser_return = true;
        }
        else if (this.current_symbol == 0x7b)
        {
            this.token = JsonToken.ObjectStart;
            this.parser_return = true;
        }
        else if (this.current_symbol == 0x7d)
        {
            this.token = JsonToken.ObjectEnd;
            this.parser_return = true;
        }
        else if (this.current_symbol == 0x22)
        {
            if (this.parser_in_string)
            {
                this.parser_in_string = false;
                this.parser_return = true;
            }
            else
            {
                if (this.token == JsonToken.None)
                {
                    this.token = JsonToken.String;
                }
                this.parser_in_string = true;
            }
        }
        else if (this.current_symbol == 0x10005)
        {
            this.token_value = this.lexer.StringValue;
        }
        else if (this.current_symbol == 0x10003)
        {
            this.token = JsonToken.Boolean;
            this.token_value = false;
            this.parser_return = true;
        }
        else if (this.current_symbol == 0x10004)
        {
            this.token = JsonToken.Null;
            this.parser_return = true;
        }
        else if (this.current_symbol == 0x10001)
        {
            this.ProcessNumber(this.lexer.StringValue);
            this.parser_return = true;
        }
        else if (this.current_symbol == 0x1000a)
        {
            this.token = JsonToken.PropertyName;
        }
        else if (this.current_symbol == 0x10002)
        {
            this.token = JsonToken.Boolean;
            this.token_value = true;
            this.parser_return = true;
        }
    }

    public bool Read()
    {
        if (this.end_of_input)
        {
            return false;
        }
        if (this.end_of_json)
        {
            this.end_of_json = false;
            this.automaton_stack.Clear();
            this.automaton_stack.Push(0x10011);
            this.automaton_stack.Push(0x10007);
        }
        this.parser_in_string = false;
        this.parser_return = false;
        this.token = JsonToken.None;
        this.token_value = null;
        if (!this.read_started)
        {
            this.read_started = true;
            if (!this.ReadToken())
            {
                return false;
            }
        }
        while (true)
        {
            if (this.parser_return)
            {
                if (this.automaton_stack.Peek() == 0x10011)
                {
                    this.end_of_json = true;
                }
                return true;
            }
            this.current_symbol = this.automaton_stack.Pop();
            this.ProcessSymbol();
            if (this.current_symbol == this.current_input)
            {
                if (!this.ReadToken())
                {
                    if (this.automaton_stack.Peek() != 0x10011)
                    {
                        throw new JsonException("Input doesn't evaluate to proper JSON text");
                    }
                    return this.parser_return;
                }
            }
            else
            {
                int[] numArray;
                try
                {
                    numArray = parse_table[this.current_symbol][this.current_input];
                }
                catch (KeyNotFoundException exception)
                {
                    throw new JsonException((ParserToken)this.current_input, exception);
                }
                if (numArray[0] != 0x10012)
                {
                    for (int i = numArray.Length - 1; i >= 0; i--)
                    {
                        this.automaton_stack.Push(numArray[i]);
                    }
                }
            }
        }
    }

    private bool ReadToken()
    {
        if (this.end_of_input)
        {
            return false;
        }
        this.lexer.NextToken();
        if (this.lexer.EndOfInput)
        {
            this.Close();
            return false;
        }
        this.current_input = this.lexer.Token;
        return true;
    }

    private static void TableAddCol(ParserToken row, int col, params int[] symbols)
    {
        parse_table[(int)row].Add(col, symbols);
    }

    private static void TableAddRow(ParserToken rule)
    {
        parse_table.Add((int)rule, new Dictionary<int, int[]>());
    }

    // Properties
    public bool AllowComments
    {
        get
        {
            return this.lexer.AllowComments;
        }
        set
        {
            this.lexer.AllowComments = value;
        }
    }

    public bool AllowSingleQuotedStrings
    {
        get
        {
            return this.lexer.AllowSingleQuotedStrings;
        }
        set
        {
            this.lexer.AllowSingleQuotedStrings = value;
        }
    }

    public bool EndOfInput
    {
        get
        {
            return this.end_of_input;
        }
    }

    public bool EndOfJson
    {
        get
        {
            return this.end_of_json;
        }
    }

    public JsonToken Token
    {
        get
        {
            return this.token;
        }
    }

    public object Value
    {
        get
        {
            return this.token_value;
        }
    }
}

internal delegate void ExporterFunc(object obj, JsonWriter writer);


[StructLayout(LayoutKind.Sequential)]
internal struct PropertyMetadata
{
    public MemberInfo Info;
    public bool IsField;
    public Type Type;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ObjectMetadata
{
    private Type element_type;
    private bool is_dictionary;
    private IDictionary<string, PropertyMetadata> properties;
    public Type ElementType
    {
        get
        {
            if (this.element_type == null)
            {
                return typeof(JsonData);
            }
            return this.element_type;
        }
        set
        {
            this.element_type = value;
        }
    }
    public bool IsDictionary
    {
        get
        {
            return this.is_dictionary;
        }
        set
        {
            this.is_dictionary = value;
        }
    }
    public IDictionary<string, PropertyMetadata> Properties
    {
        get
        {
            return this.properties;
        }
        set
        {
            this.properties = value;
        }
    }
}
public delegate void ExporterFunc<T>(T obj, JsonWriter writer);

internal delegate object ImporterFunc(object input);
public delegate TValue ImporterFunc<TJson, TValue>(TJson input);

public class JsonMapper
{
    // Fields
    private static IDictionary<Type, ArrayMetadata> array_metadata = new Dictionary<Type, ArrayMetadata>();
    private static readonly object array_metadata_lock = new object();
    private static IDictionary<Type, ExporterFunc> base_exporters_table = new Dictionary<Type, ExporterFunc>();
    private static IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
    private static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
    private static readonly object conv_ops_lock = new object();
    private static IDictionary<Type, ExporterFunc> custom_exporters_table = new Dictionary<Type, ExporterFunc>();
    private static IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
    private static IFormatProvider datetime_format = DateTimeFormatInfo.InvariantInfo;
    private static int max_nesting_depth = 100;
    private static IDictionary<Type, ObjectMetadata> object_metadata = new Dictionary<Type, ObjectMetadata>();
    private static readonly object object_metadata_lock = new object();
    private static JsonWriter static_writer = new JsonWriter();
    private static readonly object static_writer_lock = new object();
    private static IDictionary<Type, IList<PropertyMetadata>> type_properties = new Dictionary<Type, IList<PropertyMetadata>>();
    private static readonly object type_properties_lock = new object();

    // Methods
    static JsonMapper()
    {
        RegisterBaseExporters();
        RegisterBaseImporters();
    }

    /// <summary>
    /// 序列化JSON
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static JsonData Deserialize(string json)
    {
        return (JsonData)ToWrapper(delegate {
            return new JsonData();
        }, json);
    }

    /// <summary>
    /// TODO!!!反序列化JSON
    /// </summary>
    /// <param name="jsonData"></param>
    /// <returns></returns>
    public static string Serialize(JsonData jsonData)
    {
        return null;
    }

    private static void AddArrayMetadata(Type type)
    {
        if (!array_metadata.ContainsKey(type))
        {
            object obj2;
            ArrayMetadata metadata = new ArrayMetadata();
            metadata.IsArray = type.IsArray;
            if (type.GetInterface("System.Collections.IList") != null)
            {
                metadata.IsList = true;
            }
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (!(info.Name != "Item"))
                {
                    ParameterInfo[] indexParameters = info.GetIndexParameters();
                    if ((indexParameters.Length == 1) && (indexParameters[0].ParameterType == typeof(int)))
                    {
                        metadata.ElementType = info.PropertyType;
                    }
                }
            }
            Monitor.Enter(obj2 = array_metadata_lock);
            try
            {
                array_metadata.Add(type, metadata);
            }
            catch (ArgumentException)
            {
            }
            finally
            {
                Monitor.Exit(obj2);
            }
        }
    }

    private static void AddObjectMetadata(Type type)
    {
        if (!object_metadata.ContainsKey(type))
        {
            object obj2;
            ObjectMetadata metadata = new ObjectMetadata();
            if (type.GetInterface("System.Collections.IDictionary") != null)
            {
                metadata.IsDictionary = true;
            }
            metadata.Properties = new Dictionary<string, PropertyMetadata>();
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (info.Name == "Item")
                {
                    ParameterInfo[] indexParameters = info.GetIndexParameters();
                    if ((indexParameters.Length == 1) && (indexParameters[0].ParameterType == typeof(string)))
                    {
                        metadata.ElementType = info.PropertyType;
                    }
                }
                else
                {
                    PropertyMetadata metadata2 = new PropertyMetadata();
                    metadata2.Info = info;
                    metadata2.Type = info.PropertyType;
                    metadata.Properties.Add(info.Name, metadata2);
                }
            }
            foreach (FieldInfo info2 in type.GetFields())
            {
                PropertyMetadata metadata3 = new PropertyMetadata();
                metadata3.Info = info2;
                metadata3.IsField = true;
                metadata3.Type = info2.FieldType;
                metadata.Properties.Add(info2.Name, metadata3);
            }
            Monitor.Enter(obj2 = object_metadata_lock);
            try
            {
                object_metadata.Add(type, metadata);
            }
            catch (ArgumentException)
            {
            }
            finally
            {
                Monitor.Exit(obj2);
            }
        }
    }

    private static void AddTypeProperties(Type type)
    {
        if (!type_properties.ContainsKey(type))
        {
            object obj2;
            IList<PropertyMetadata> list = new List<PropertyMetadata>();
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (!(info.Name == "Item"))
                {
                    PropertyMetadata item = new PropertyMetadata();
                    item.Info = info;
                    item.IsField = false;
                    list.Add(item);
                }
            }
            foreach (FieldInfo info2 in type.GetFields())
            {
                PropertyMetadata metadata2 = new PropertyMetadata();
                metadata2.Info = info2;
                metadata2.IsField = true;
                list.Add(metadata2);
            }
            Monitor.Enter(obj2 = type_properties_lock);
            try
            {
                type_properties.Add(type, list);
            }
            catch (ArgumentException)
            {
            }
            finally
            {
                Monitor.Exit(obj2);
            }
        }
    }

    private static MethodInfo GetConvOp(Type t1, Type t2)
    {
        object obj3;
        lock (conv_ops_lock)
        {
            if (!conv_ops.ContainsKey(t1))
            {
                conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
            }
        }
        if (conv_ops[t1].ContainsKey(t2))
        {
            return conv_ops[t1][t2];
        }
        MethodInfo method = t1.GetMethod("op_Implicit", new Type[] { t2 });
        Monitor.Enter(obj3 = conv_ops_lock);
        try
        {
            conv_ops[t1].Add(t2, method);
        }
        catch (ArgumentException)
        {
            return conv_ops[t1][t2];
        }
        finally
        {
            Monitor.Exit(obj3);
        }
        return method;
    }

    private static IJsonWrapper ReadValue(WrapperFactory factory, JsonReader reader)
    {
        reader.Read();
        if ((reader.Token == JsonToken.ArrayEnd) || (reader.Token == JsonToken.Null))
        {
            return null;
        }
        IJsonWrapper wrapper = factory();
        if (reader.Token == JsonToken.String)
        {
            wrapper.SetString((string)reader.Value);
            return wrapper;
        }
        if (reader.Token == JsonToken.Double)
        {
            wrapper.SetDouble((double)reader.Value);
            return wrapper;
        }
        if (reader.Token == JsonToken.Int)
        {
            wrapper.SetInt((int)reader.Value);
            return wrapper;
        }
        if (reader.Token == JsonToken.Long)
        {
            wrapper.SetLong((long)reader.Value);
            return wrapper;
        }
        if (reader.Token == JsonToken.Boolean)
        {
            wrapper.SetBoolean((bool)reader.Value);
            return wrapper;
        }
        if (reader.Token == JsonToken.ArrayStart)
        {
            wrapper.SetJsonType(JsonType.Array);
            while (true)
            {
                IJsonWrapper wrapper2 = ReadValue(factory, reader);
                if ((reader.Token == JsonToken.ArrayEnd) && (wrapper2 == null))
                {
                    return wrapper;
                }
                wrapper.Add(wrapper2);
            }
        }
        if (reader.Token == JsonToken.ObjectStart)
        {
            wrapper.SetJsonType(JsonType.Object);
            while (true)
            {
                reader.Read();
                if (reader.Token == JsonToken.ObjectEnd)
                {
                    return wrapper;
                }
                string str = (string)reader.Value;
                wrapper[str] = ReadValue(factory, reader);
            }
        }
        return wrapper;
    }

    private static object ReadValue(Type inst_type, JsonReader reader)
    {
        IList list;
        Type elementType;
        object obj3;
        reader.Read();
        if (reader.Token == JsonToken.ArrayEnd)
        {
            return null;
        }
        if (reader.Token == JsonToken.Null)
        {
            if (!inst_type.IsClass)
            {
                throw new JsonException(string.Format("Can't assign null to an instance of type {0}", inst_type));
            }
            return null;
        }
        if (((reader.Token == JsonToken.Double) || (reader.Token == JsonToken.Int)) || (((reader.Token == JsonToken.Long) || (reader.Token == JsonToken.String)) || (reader.Token == JsonToken.Boolean)))
        {
            Type c = reader.Value.GetType();
            if (inst_type.IsAssignableFrom(c))
            {
                return reader.Value;
            }
            if (custom_importers_table.ContainsKey(c) && custom_importers_table[c].ContainsKey(inst_type))
            {
                ImporterFunc func = custom_importers_table[c][inst_type];
                return func(reader.Value);
            }
            if (base_importers_table.ContainsKey(c) && base_importers_table[c].ContainsKey(inst_type))
            {
                ImporterFunc func2 = base_importers_table[c][inst_type];
                return func2(reader.Value);
            }
            if (inst_type.IsEnum)
            {
                return Enum.ToObject(inst_type, reader.Value);
            }
            MethodInfo convOp = GetConvOp(inst_type, c);
            if (convOp == null)
            {
                throw new JsonException(string.Format("Can't assign value '{0}' (type {1}) to type {2}", reader.Value, c, inst_type));
            }
            return convOp.Invoke(null, new object[] { reader.Value });
        }
        object obj2 = null;
        if (reader.Token != JsonToken.ArrayStart)
        {
            goto Label_025A;
        }
        AddArrayMetadata(inst_type);
        ArrayMetadata metadata = array_metadata[inst_type];
        if (!metadata.IsArray && !metadata.IsList)
        {
            throw new JsonException(string.Format("Type {0} can't act as an array", inst_type));
        }
        if (!metadata.IsArray)
        {
            list = (IList)Activator.CreateInstance(inst_type);
            elementType = metadata.ElementType;
        }
        else
        {
            list = new ArrayList();
            elementType = inst_type.GetElementType();
        }
        Label_01CC:
        obj3 = ReadValue(elementType, reader);
        if (reader.Token == JsonToken.ArrayEnd)
        {
            list.Add(obj3);
            list.RemoveAt(list.Count - 1);
        }
        else
        {
            list.Add(obj3);
            goto Label_01CC;
        }
        if (!metadata.IsArray)
        {
            return list;
        }
        int count = list.Count;
        obj2 = Array.CreateInstance(elementType, count);
        for (int i = 0; i < count; i++)
        {
            ((Array)obj2).SetValue(list[i], i);
        }
        return obj2;
        Label_025A:
        if (reader.Token != JsonToken.ObjectStart)
        {
            return obj2;
        }
        AddObjectMetadata(inst_type);
        ObjectMetadata metadata2 = object_metadata[inst_type];
        obj2 = Activator.CreateInstance(inst_type);
        while (true)
        {
            reader.Read();
            if (reader.Token == JsonToken.ObjectEnd)
            {
                return obj2;
            }
            string key = (string)reader.Value;
            if (metadata2.Properties.ContainsKey(key))
            {
                PropertyMetadata metadata3 = metadata2.Properties[key];
                if (metadata3.IsField)
                {
                    ((FieldInfo)metadata3.Info).SetValue(obj2, ReadValue(metadata3.Type, reader));
                }
                else
                {
                    PropertyInfo info = (PropertyInfo)metadata3.Info;
                    if (info.CanWrite)
                    {
                        info.SetValue(obj2, ReadValue(metadata3.Type, reader), null);
                    }
                    else
                    {
                        ReadValue(metadata3.Type, reader);
                    }
                }
            }
            else
            {
                if (!metadata2.IsDictionary)
                {
                    throw new JsonException(string.Format("The type {0} doesn't have the property '{1}'", inst_type, key));
                }
                ((IDictionary)obj2).Add(key, ReadValue(metadata2.ElementType, reader));
            }
        }
    }

    private static void RegisterBaseExporters()
    {
        base_exporters_table[typeof(byte)] = delegate (object obj, JsonWriter writer) {
            writer.Write(Convert.ToInt32((byte)obj));
        };
        base_exporters_table[typeof(char)] = delegate (object obj, JsonWriter writer) {
            writer.Write(Convert.ToString((char)obj));
        };
        base_exporters_table[typeof(DateTime)] = delegate (object obj, JsonWriter writer) {
            writer.Write(Convert.ToString((DateTime)obj, datetime_format));
        };
        base_exporters_table[typeof(decimal)] = delegate (object obj, JsonWriter writer) {
            writer.Write((decimal)obj);
        };
        base_exporters_table[typeof(sbyte)] = delegate (object obj, JsonWriter writer) {
            writer.Write(Convert.ToInt32((sbyte)obj));
        };
        base_exporters_table[typeof(short)] = delegate (object obj, JsonWriter writer) {
            writer.Write(Convert.ToInt32((short)obj));
        };
        base_exporters_table[typeof(ushort)] = delegate (object obj, JsonWriter writer) {
            writer.Write(Convert.ToInt32((ushort)obj));
        };
        base_exporters_table[typeof(uint)] = delegate (object obj, JsonWriter writer) {
            writer.Write(Convert.ToUInt64((uint)obj));
        };
        base_exporters_table[typeof(ulong)] = delegate (object obj, JsonWriter writer) {
            writer.Write((ulong)obj);
        };
    }

    private static void RegisterBaseImporters()
    {
        ImporterFunc importer = delegate (object input) {
            return Convert.ToByte((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(byte), importer);
        importer = delegate (object input) {
            return Convert.ToUInt64((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(ulong), importer);
        importer = delegate (object input) {
            return Convert.ToSByte((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(sbyte), importer);
        importer = delegate (object input) {
            return Convert.ToInt16((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(short), importer);
        importer = delegate (object input) {
            return Convert.ToUInt16((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(ushort), importer);
        importer = delegate (object input) {
            return Convert.ToUInt32((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(uint), importer);
        importer = delegate (object input) {
            return Convert.ToSingle((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(float), importer);
        importer = delegate (object input) {
            return Convert.ToDouble((int)input);
        };
        RegisterImporter(base_importers_table, typeof(int), typeof(double), importer);
        importer = delegate (object input) {
            return Convert.ToDecimal((double)input);
        };
        RegisterImporter(base_importers_table, typeof(double), typeof(decimal), importer);
        importer = delegate (object input) {
            return Convert.ToUInt32((long)input);
        };
        RegisterImporter(base_importers_table, typeof(long), typeof(uint), importer);
        importer = delegate (object input) {
            return Convert.ToChar((string)input);
        };
        RegisterImporter(base_importers_table, typeof(string), typeof(char), importer);
        importer = delegate (object input) {
            return Convert.ToDateTime((string)input, datetime_format);
        };
        RegisterImporter(base_importers_table, typeof(string), typeof(DateTime), importer);
    }

    public static void RegisterExporter<T>(ExporterFunc<T> exporter)
    {
        ExporterFunc func = delegate (object obj, JsonWriter writer) {
            exporter((T)obj, writer);
        };
        custom_exporters_table[typeof(T)] = func;
    }

    public static void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
    {
        ImporterFunc func = delegate (object input) {
            return importer((TJson)input);
        };
        RegisterImporter(custom_importers_table, typeof(TJson), typeof(TValue), func);
    }

    private static void RegisterImporter(IDictionary<Type, IDictionary<Type, ImporterFunc>> table, Type json_type, Type value_type, ImporterFunc importer)
    {
        if (!table.ContainsKey(json_type))
        {
            table.Add(json_type, new Dictionary<Type, ImporterFunc>());
        }
        table[json_type][value_type] = importer;
    }

    public static string ToJson(object obj)
    {
        lock (static_writer_lock)
        {
            static_writer.Reset();
            WriteValue(obj, static_writer, true, 0);
            return static_writer.ToString();
        }
    }

    public static void ToJson(object obj, JsonWriter writer)
    {
        WriteValue(obj, writer, false, 0);
    }

    public static JsonData ToObject(JsonReader reader)
    {
        return (JsonData)ToWrapper(delegate {
            return new JsonData();
        }, reader);
    }

    public static T ToObject<T>(JsonReader reader)
    {
        return (T)ReadValue(typeof(T), reader);
    }

    public static JsonData ToObject(TextReader reader)
    {
        JsonReader reader2 = new JsonReader(reader);
        return (JsonData)ToWrapper(delegate {
            return new JsonData();
        }, reader2);
    }

    public static T ToObject<T>(TextReader reader)
    {
        JsonReader reader2 = new JsonReader(reader);
        return (T)ReadValue(typeof(T), reader2);
    }

    public static JsonData ToObject(string json)
    {
        return (JsonData)ToWrapper(delegate {
            return new JsonData();
        }, json);
    }

    public static T ToObject<T>(string json)
    {
        JsonReader reader = new JsonReader(json);
        return (T)ReadValue(typeof(T), reader);
    }

    public static IJsonWrapper ToWrapper(WrapperFactory factory, JsonReader reader)
    {
        return ReadValue(factory, reader);
    }

    public static IJsonWrapper ToWrapper(WrapperFactory factory, string json)
    {
        JsonReader reader = new JsonReader(json);
        return ReadValue(factory, reader);
    }

    public static void UnregisterExporters()
    {
        custom_exporters_table.Clear();
    }

    public static void UnregisterImporters()
    {
        custom_importers_table.Clear();
    }

    private static void WriteValue(object obj, JsonWriter writer, bool writer_is_private, int depth)
    {
        if (depth > max_nesting_depth)
        {
            throw new JsonException(string.Format("Max allowed object depth reached while trying to export from type {0}", obj.GetType()));
        }
        if (obj == null)
        {
            writer.Write((string)null);
        }
        else if (obj is IJsonWrapper)
        {
            if (writer_is_private)
            {
                writer.TextWriter.Write(((IJsonWrapper)obj).ToJson());
            }
            else
            {
                ((IJsonWrapper)obj).ToJson(writer);
            }
        }
        else if (obj is string)
        {
            writer.Write((string)obj);
        }
        else if (obj is double)
        {
            writer.Write((double)obj);
        }
        else if (obj is int)
        {
            writer.Write((int)obj);
        }
        else if (obj is bool)
        {
            writer.Write((bool)obj);
        }
        else if (obj is long)
        {
            writer.Write((long)obj);
        }
        else if (obj is Array)
        {
            writer.WriteArrayStart();
            foreach (object obj2 in (Array)obj)
            {
                WriteValue(obj2, writer, writer_is_private, depth + 1);
            }
            writer.WriteArrayEnd();
        }
        else if (obj is IList)
        {
            writer.WriteArrayStart();
            foreach (object obj3 in (IList)obj)
            {
                WriteValue(obj3, writer, writer_is_private, depth + 1);
            }
            writer.WriteArrayEnd();
        }
        else if (obj is IDictionary)
        {
            writer.WriteObjectStart();
            foreach (DictionaryEntry entry in (IDictionary)obj)
            {
                writer.WritePropertyName((string)entry.Key);
                WriteValue(entry.Value, writer, writer_is_private, depth + 1);
            }
            writer.WriteObjectEnd();
        }
        else
        {
            Type key = obj.GetType();
            if (custom_exporters_table.ContainsKey(key))
            {
                ExporterFunc func = custom_exporters_table[key];
                func(obj, writer);
            }
            else if (base_exporters_table.ContainsKey(key))
            {
                ExporterFunc func2 = base_exporters_table[key];
                func2(obj, writer);
            }
            else if (obj is Enum)
            {
                Type underlyingType = Enum.GetUnderlyingType(key);
                if (((underlyingType == typeof(long)) || (underlyingType == typeof(uint))) || (underlyingType == typeof(ulong)))
                {
                    writer.Write((ulong)obj);
                }
                else
                {
                    writer.Write((int)obj);
                }
            }
            else
            {
                AddTypeProperties(key);
                IList<PropertyMetadata> list = type_properties[key];
                writer.WriteObjectStart();
                foreach (PropertyMetadata metadata in list)
                {
                    if (metadata.IsField)
                    {
                        writer.WritePropertyName(metadata.Info.Name);
                        WriteValue(((FieldInfo)metadata.Info).GetValue(obj), writer, writer_is_private, depth + 1);
                    }
                    else
                    {
                        PropertyInfo info = (PropertyInfo)metadata.Info;
                        if (info.CanRead)
                        {
                            writer.WritePropertyName(metadata.Info.Name);
                            WriteValue(info.GetValue(obj, null), writer, writer_is_private, depth + 1);
                        }
                    }
                }
                writer.WriteObjectEnd();
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct ArrayMetadata
{
    private Type element_type;
    private bool is_array;
    private bool is_list;
    public Type ElementType
    {
        get
        {
            if (this.element_type == null)
            {
                return typeof(JsonData);
            }
            return this.element_type;
        }
        set
        {
            this.element_type = value;
        }
    }
    public bool IsArray
    {
        get
        {
            return this.is_array;
        }
        set
        {
            this.is_array = value;
        }
    }
    public bool IsList
    {
        get
        {
            return this.is_list;
        }
        set
        {
            this.is_list = value;
        }
    }
}
public delegate IJsonWrapper WrapperFactory();

public class JsonException : ApplicationException
{
    // Methods
    public JsonException()
    {
    }

    internal JsonException(ParserToken token) : base(string.Format("Invalid token '{0}' in input string", token))
    {
    }

    internal JsonException(int c) : base(string.Format("Invalid character '{0}' in input string", (char)c))
    {
    }

    public JsonException(string message) : base(message)
    {
    }

    internal JsonException(ParserToken token, Exception inner_exception) : base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
    {
    }

    internal JsonException(int c, Exception inner_exception) : base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
    {
    }

    public JsonException(string message, Exception inner_exception) : base(message, inner_exception)
    {
    }
}

internal class WriterContext
{
    // Fields
    public int Count;
    public bool ExpectingValue;
    public bool InArray;
    public bool InObject;
    public int Padding;
}

internal class Lexer
{
    // Fields
    private bool allow_comments = true;
    private bool allow_single_quoted_strings = true;
    private bool end_of_input = false;
    private FsmContext fsm_context;
    private static StateHandler[] fsm_handler_table;
    private static int[] fsm_return_table;
    private int input_buffer = 0;
    private int input_char;
    private TextReader reader;
    private int state = 1;
    private StringBuilder string_buffer = new StringBuilder(0x80);
    private string string_value;
    private int token;
    private int unichar;

    // Methods
    static Lexer()
    {
        PopulateFsmTables();
    }

    public Lexer(TextReader reader)
    {
        this.reader = reader;
        this.fsm_context = new FsmContext();
        this.fsm_context.L = this;
    }

    private bool GetChar()
    {
        this.input_char = this.NextChar();
        if (this.input_char != -1)
        {
            return true;
        }
        this.end_of_input = true;
        return false;
    }

    private static int HexValue(int digit)
    {
        switch (digit)
        {
            case 0x41:
            case 0x61:
                return 10;

            case 0x42:
            case 0x62:
                return 11;

            case 0x43:
            case 0x63:
                return 12;

            case 0x44:
            case 100:
                return 13;

            case 0x45:
            case 0x65:
                return 14;

            case 70:
            case 0x66:
                return 15;
        }
        return (digit - 0x30);
    }

    private int NextChar()
    {
        if (this.input_buffer != 0)
        {
            int num = this.input_buffer;
            this.input_buffer = 0;
            return num;
        }
        return this.reader.Read();
    }

    public bool NextToken()
    {
        this.fsm_context.Return = false;
        while (true)
        {
            StateHandler handler = fsm_handler_table[this.state - 1];
            if (!handler(this.fsm_context))
            {
                throw new JsonException(this.input_char);
            }
            if (this.end_of_input)
            {
                return false;
            }
            if (this.fsm_context.Return)
            {
                this.string_value = this.string_buffer.ToString();
                this.string_buffer.Remove(0, this.string_buffer.Length);
                this.token = fsm_return_table[this.state - 1];
                if (this.token == 0x10006)
                {
                    this.token = this.input_char;
                }
                this.state = this.fsm_context.NextState;
                return true;
            }
            this.state = this.fsm_context.NextState;
        }
    }

    private static void PopulateFsmTables()
    {
        fsm_handler_table = new StateHandler[] {
        new StateHandler(Lexer.State1), new StateHandler(Lexer.State2), new StateHandler(Lexer.State3), new StateHandler(Lexer.State4), new StateHandler(Lexer.State5), new StateHandler(Lexer.State6), new StateHandler(Lexer.State7), new StateHandler(Lexer.State8), new StateHandler(Lexer.State9), new StateHandler(Lexer.State10), new StateHandler(Lexer.State11), new StateHandler(Lexer.State12), new StateHandler(Lexer.State13), new StateHandler(Lexer.State14), new StateHandler(Lexer.State15), new StateHandler(Lexer.State16),
        new StateHandler(Lexer.State17), new StateHandler(Lexer.State18), new StateHandler(Lexer.State19), new StateHandler(Lexer.State20), new StateHandler(Lexer.State21), new StateHandler(Lexer.State22), new StateHandler(Lexer.State23), new StateHandler(Lexer.State24), new StateHandler(Lexer.State25), new StateHandler(Lexer.State26), new StateHandler(Lexer.State27), new StateHandler(Lexer.State28)
        };
        fsm_return_table = new int[] {
        0x10006, 0, 0x10001, 0x10001, 0, 0x10001, 0, 0x10001, 0, 0, 0x10002, 0, 0, 0, 0x10003, 0,
        0, 0x10004, 0x10005, 0x10006, 0, 0, 0x10005, 0x10006, 0, 0, 0, 0
        };
    }

    private static char ProcessEscChar(int esc_char)
    {
        switch (esc_char)
        {
            case 0x2f:
            case 0x5c:
            case 0x22:
            case 0x27:
                return Convert.ToChar(esc_char);

            case 0x62:
                return '\b';

            case 0x66:
                return '\f';

            case 0x72:
                return '\r';

            case 0x74:
                return '\t';

            case 110:
                return '\n';
        }
        return '?';
    }

    private static bool State1(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            if ((ctx.L.input_char == 0x20) || ((ctx.L.input_char >= 9) && (ctx.L.input_char <= 13)))
            {
                continue;
            }
            if ((ctx.L.input_char >= 0x31) && (ctx.L.input_char <= 0x39))
            {
                ctx.L.string_buffer.Append((char)ctx.L.input_char);
                ctx.NextState = 3;
                return true;
            }
            switch (ctx.L.input_char)
            {
                case 0x2c:
                case 0x3a:
                case 0x5b:
                case 0x5d:
                case 0x7b:
                case 0x7d:
                    ctx.NextState = 1;
                    ctx.Return = true;
                    return true;

                case 0x2d:
                    ctx.L.string_buffer.Append((char)ctx.L.input_char);
                    ctx.NextState = 2;
                    return true;

                case 0x2f:
                    if (ctx.L.allow_comments)
                    {
                        break;
                    }
                    return false;

                case 0x30:
                    ctx.L.string_buffer.Append((char)ctx.L.input_char);
                    ctx.NextState = 4;
                    return true;

                case 0x22:
                    ctx.NextState = 0x13;
                    ctx.Return = true;
                    return true;

                case 0x27:
                    if (!ctx.L.allow_single_quoted_strings)
                    {
                        return false;
                    }
                    ctx.L.input_char = 0x22;
                    ctx.NextState = 0x17;
                    ctx.Return = true;
                    return true;

                case 0x66:
                    ctx.NextState = 12;
                    return true;

                case 0x74:
                    ctx.NextState = 9;
                    return true;

                case 110:
                    ctx.NextState = 0x10;
                    return true;

                default:
                    return false;
            }
            ctx.NextState = 0x19;
            return true;
        }
        return true;
    }

    private static bool State10(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x75)
        {
            ctx.NextState = 11;
            return true;
        }
        return false;
    }

    private static bool State11(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x65)
        {
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
        }
        return false;
    }

    private static bool State12(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x61)
        {
            ctx.NextState = 13;
            return true;
        }
        return false;
    }

    private static bool State13(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x6c)
        {
            ctx.NextState = 14;
            return true;
        }
        return false;
    }

    private static bool State14(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x73)
        {
            ctx.NextState = 15;
            return true;
        }
        return false;
    }

    private static bool State15(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x65)
        {
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
        }
        return false;
    }

    private static bool State16(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x75)
        {
            ctx.NextState = 0x11;
            return true;
        }
        return false;
    }

    private static bool State17(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x6c)
        {
            ctx.NextState = 0x12;
            return true;
        }
        return false;
    }

    private static bool State18(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x6c)
        {
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
        }
        return false;
    }

    private static bool State19(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            switch (ctx.L.input_char)
            {
                case 0x22:
                    ctx.L.UngetChar();
                    ctx.Return = true;
                    ctx.NextState = 20;
                    return true;

                case 0x5c:
                    ctx.StateStack = 0x13;
                    ctx.NextState = 0x15;
                    return true;
            }
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
        }
        return true;
    }

    private static bool State2(FsmContext ctx)
    {
        ctx.L.GetChar();
        if ((ctx.L.input_char >= 0x31) && (ctx.L.input_char <= 0x39))
        {
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
            ctx.NextState = 3;
            return true;
        }
        if (ctx.L.input_char == 0x30)
        {
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
            ctx.NextState = 4;
            return true;
        }
        return false;
    }

    private static bool State20(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x22)
        {
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
        }
        return false;
    }

    private static bool State21(FsmContext ctx)
    {
        ctx.L.GetChar();
        switch (ctx.L.input_char)
        {
            case 0x2f:
            case 0x5c:
            case 0x22:
            case 0x27:
            case 0x62:
            case 0x66:
            case 0x72:
            case 0x74:
            case 110:
                ctx.L.string_buffer.Append(ProcessEscChar(ctx.L.input_char));
                ctx.NextState = ctx.StateStack;
                return true;

            case 0x75:
                ctx.NextState = 0x16;
                return true;
        }
        return false;
    }

    private static bool State22(FsmContext ctx)
    {
        int num = 0;
        int num2 = 0x1000;
        ctx.L.unichar = 0;
        while (ctx.L.GetChar())
        {
            if ((((ctx.L.input_char < 0x30) || (ctx.L.input_char > 0x39)) && ((ctx.L.input_char < 0x41) || (ctx.L.input_char > 70))) && ((ctx.L.input_char < 0x61) || (ctx.L.input_char > 0x66)))
            {
                return false;
            }
            ctx.L.unichar += HexValue(ctx.L.input_char) * num2;
            num++;
            num2 /= 0x10;
            if (num == 4)
            {
                ctx.L.string_buffer.Append(Convert.ToChar(ctx.L.unichar));
                ctx.NextState = ctx.StateStack;
                return true;
            }
        }
        return true;
    }

    private static bool State23(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            switch (ctx.L.input_char)
            {
                case 0x27:
                    ctx.L.UngetChar();
                    ctx.Return = true;
                    ctx.NextState = 0x18;
                    return true;

                case 0x5c:
                    ctx.StateStack = 0x17;
                    ctx.NextState = 0x15;
                    return true;
            }
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
        }
        return true;
    }

    private static bool State24(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x27)
        {
            ctx.L.input_char = 0x22;
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
        }
        return false;
    }

    private static bool State25(FsmContext ctx)
    {
        ctx.L.GetChar();
        switch (ctx.L.input_char)
        {
            case 0x2a:
                ctx.NextState = 0x1b;
                return true;

            case 0x2f:
                ctx.NextState = 0x1a;
                return true;
        }
        return false;
    }

    private static bool State26(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            if (ctx.L.input_char == 10)
            {
                ctx.NextState = 1;
                return true;
            }
        }
        return true;
    }

    private static bool State27(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            if (ctx.L.input_char == 0x2a)
            {
                ctx.NextState = 0x1c;
                return true;
            }
        }
        return true;
    }

    private static bool State28(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            if (ctx.L.input_char != 0x2a)
            {
                if (ctx.L.input_char == 0x2f)
                {
                    ctx.NextState = 1;
                    return true;
                }
                ctx.NextState = 0x1b;
                return true;
            }
        }
        return true;
    }

    private static bool State3(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            if ((ctx.L.input_char >= 0x30) && (ctx.L.input_char <= 0x39))
            {
                ctx.L.string_buffer.Append((char)ctx.L.input_char);
                continue;
            }
            if ((ctx.L.input_char == 0x20) || ((ctx.L.input_char >= 9) && (ctx.L.input_char <= 13)))
            {
                ctx.Return = true;
                ctx.NextState = 1;
                return true;
            }
            int num = ctx.L.input_char;
            if (num <= 0x45)
            {
                switch (num)
                {
                    case 0x2c:
                        goto Label_00BE;

                    case 0x2d:
                        goto Label_0125;

                    case 0x2e:
                        ctx.L.string_buffer.Append((char)ctx.L.input_char);
                        ctx.NextState = 5;
                        return true;

                    case 0x45:
                        goto Label_00FF;
                }
                goto Label_0125;
            }
            if (num != 0x5d)
            {
                if (num == 0x65)
                {
                    goto Label_00FF;
                }
                if (num != 0x7d)
                {
                    goto Label_0125;
                }
            }
            Label_00BE:
            ctx.L.UngetChar();
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
            Label_00FF:
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
            ctx.NextState = 7;
            return true;
            Label_0125:
            return false;
        }
        return true;
    }

    private static bool State4(FsmContext ctx)
    {
        ctx.L.GetChar();
        if ((ctx.L.input_char == 0x20) || ((ctx.L.input_char >= 9) && (ctx.L.input_char <= 13)))
        {
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
        }
        int num = ctx.L.input_char;
        if (num <= 0x45)
        {
            switch (num)
            {
                case 0x2c:
                    goto Label_0085;

                case 0x2d:
                    goto Label_00EC;

                case 0x2e:
                    ctx.L.string_buffer.Append((char)ctx.L.input_char);
                    ctx.NextState = 5;
                    return true;

                case 0x45:
                    goto Label_00C6;
            }
            goto Label_00EC;
        }
        if (num != 0x5d)
        {
            if (num == 0x65)
            {
                goto Label_00C6;
            }
            if (num != 0x7d)
            {
                goto Label_00EC;
            }
        }
        Label_0085:
        ctx.L.UngetChar();
        ctx.Return = true;
        ctx.NextState = 1;
        return true;
        Label_00C6:
        ctx.L.string_buffer.Append((char)ctx.L.input_char);
        ctx.NextState = 7;
        return true;
        Label_00EC:
        return false;
    }

    private static bool State5(FsmContext ctx)
    {
        ctx.L.GetChar();
        if ((ctx.L.input_char >= 0x30) && (ctx.L.input_char <= 0x39))
        {
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
            ctx.NextState = 6;
            return true;
        }
        return false;
    }

    private static bool State6(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            if ((ctx.L.input_char >= 0x30) && (ctx.L.input_char <= 0x39))
            {
                ctx.L.string_buffer.Append((char)ctx.L.input_char);
                continue;
            }
            if ((ctx.L.input_char == 0x20) || ((ctx.L.input_char >= 9) && (ctx.L.input_char <= 13)))
            {
                ctx.Return = true;
                ctx.NextState = 1;
                return true;
            }
            int num = ctx.L.input_char;
            if (num <= 0x45)
            {
                switch (num)
                {
                    case 0x2c:
                        goto Label_00AE;

                    case 0x45:
                        goto Label_00C9;
                }
                goto Label_00EF;
            }
            if (num != 0x5d)
            {
                if (num == 0x65)
                {
                    goto Label_00C9;
                }
                if (num != 0x7d)
                {
                    goto Label_00EF;
                }
            }
            Label_00AE:
            ctx.L.UngetChar();
            ctx.Return = true;
            ctx.NextState = 1;
            return true;
            Label_00C9:
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
            ctx.NextState = 7;
            return true;
            Label_00EF:
            return false;
        }
        return true;
    }

    private static bool State7(FsmContext ctx)
    {
        ctx.L.GetChar();
        if ((ctx.L.input_char >= 0x30) && (ctx.L.input_char <= 0x39))
        {
            ctx.L.string_buffer.Append((char)ctx.L.input_char);
            ctx.NextState = 8;
            return true;
        }
        switch (ctx.L.input_char)
        {
            case 0x2b:
            case 0x2d:
                ctx.L.string_buffer.Append((char)ctx.L.input_char);
                ctx.NextState = 8;
                return true;
        }
        return false;
    }

    private static bool State8(FsmContext ctx)
    {
        while (ctx.L.GetChar())
        {
            if ((ctx.L.input_char >= 0x30) && (ctx.L.input_char <= 0x39))
            {
                ctx.L.string_buffer.Append((char)ctx.L.input_char);
            }
            else
            {
                if ((ctx.L.input_char == 0x20) || ((ctx.L.input_char >= 9) && (ctx.L.input_char <= 13)))
                {
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
                }
                int num = ctx.L.input_char;
                if (((num != 0x2c) && (num != 0x5d)) && (num != 0x7d))
                {
                    return false;
                }
                ctx.L.UngetChar();
                ctx.Return = true;
                ctx.NextState = 1;
                return true;
            }
        }
        return true;
    }

    private static bool State9(FsmContext ctx)
    {
        ctx.L.GetChar();
        if (ctx.L.input_char == 0x72)
        {
            ctx.NextState = 10;
            return true;
        }
        return false;
    }

    private void UngetChar()
    {
        this.input_buffer = this.input_char;
    }

    // Properties
    public bool AllowComments
    {
        get
        {
            return this.allow_comments;
        }
        set
        {
            this.allow_comments = value;
        }
    }

    public bool AllowSingleQuotedStrings
    {
        get
        {
            return this.allow_single_quoted_strings;
        }
        set
        {
            this.allow_single_quoted_strings = value;
        }
    }

    public bool EndOfInput
    {
        get
        {
            return this.end_of_input;
        }
    }

    public string StringValue
    {
        get
        {
            return this.string_value;
        }
    }

    public int Token
    {
        get
        {
            return this.token;
        }
    }

    // Nested Types
    private delegate bool StateHandler(FsmContext ctx);
}
internal class FsmContext
{
    // Fields
    public Lexer L;
    public int NextState;
    public bool Return;
    public int StateStack;
}

internal enum ParserToken
{
    Array = 0x1000c,
    ArrayPrime = 0x1000d,
    Char = 0x10006,
    CharSeq = 0x10005,
    End = 0x10011,
    Epsilon = 0x10012,
    False = 0x10003,
    None = 0x10000,
    Null = 0x10004,
    Number = 0x10001,
    Object = 0x10008,
    ObjectPrime = 0x10009,
    Pair = 0x1000a,
    PairRest = 0x1000b,
    String = 0x10010,
    Text = 0x10007,
    True = 0x10002,
    Value = 0x1000e,
    ValueRest = 0x1000f
}

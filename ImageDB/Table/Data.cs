using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Linq;
using System.Diagnostics;

namespace ImageDB.Table
{
    public interface IData
    {
        public Dictionary<string, object> Parameter { get; }
        object[] Value();
        string[] Key();
    }
    public class Data : IData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Dictionary<string, object> Parameter { protected set; get; }
        public object[] Value() =>
            [.. Parameter.Values];
        public string[] Key() =>
            [.. Parameter.Keys];

        public Data(Dictionary<string, object> parameter) =>
            Parameter = parameter;
        public Data() => Parameter = [];
        public void AddOrUpdate(string key, object value)
        {
            if (Parameter.ContainsKey(key))
                Parameter[key] = value;
            else
                Parameter.Add(key, value);
        }
        public static bool operator ==(Data left, Data right) =>
            left.Parameter == right.Parameter;
        public static bool operator !=(Data left, Data right) =>
            left.Parameter != right.Parameter;
        public override bool Equals(object obj) =>
            obj.GetType() == GetType() &&
            Parameter.SequenceEqual((obj as Data).Parameter);
        public override int GetHashCode()
            => Parameter.GetHashCode();
    }
}

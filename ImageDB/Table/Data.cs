using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Linq;

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
        public static bool operator == (Data left, Data right) =>
            right is not null && left.Parameter == right.Parameter;
        public static bool operator != (Data left, Data right) =>
			right is not null && left.Parameter != right.Parameter;
        public override bool Equals(object obj) =>
            obj.GetType() == GetType() &&
            Parameter.SequenceEqual((obj as Data).Parameter);
        public override int GetHashCode()
            => Parameter.GetHashCode();
    }
}

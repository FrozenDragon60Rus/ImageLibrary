using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel.DataAnnotations.Schema;

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
            Parameter.Values.ToArray();
        public string[] Key() =>
            Parameter.Keys.ToArray();

        public Data(Dictionary<string, object> parameter) =>
            Parameter = parameter;
        public Data() => Parameter = [];
        public void Set(Dictionary<string, object> parameter) => Parameter = parameter;
    }
}

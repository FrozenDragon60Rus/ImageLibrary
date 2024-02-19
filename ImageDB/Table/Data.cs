using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageDB.Table
{
    public interface IData
    {
        Dictionary<string, object> parameter { set; get; }
        object[] Value();
        string[] Key();
        string[] ParameterName();
    }
    public class Data : IData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Dictionary<string, object> parameter { get; set; }
        public object[] Value() =>
            parameter.Values.ToArray();
        public string[] Key() =>
            parameter.Keys.ToArray();
        public string[] ParameterName()
        {
            string[] key = parameter.Keys.ToArray();
            for (int i = 0; i < key.Length; i++)
                key[i] = "@" + key[i];
            return key;
        }

        public Data(Dictionary<string, object> parameter) =>
            this.parameter = parameter;
        public Data()
        {
            parameter = new Dictionary<string, object>();
        }
    }
}

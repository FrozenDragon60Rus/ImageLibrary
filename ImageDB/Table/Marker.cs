using System;
using System.Collections.Generic;

namespace ImageDB.Table
{
    internal class Marker : Data
    {
        public int Id { get => Convert.ToInt32(Parameter[nameof(Id)]); }
        public string Name { get => Parameter[nameof(Name)].ToString(); }
        public Marker(int Id, string Name) : base() =>
            Parameter = new Dictionary<string, object>() {
                { nameof(Id), Id },
                { nameof(Name), Name }
            };
        public Marker(Dictionary<string, object> parameter) : base(parameter) =>
            Parameter = parameter;
        public Marker() =>
            Parameter = Empty.Parameter;

        public static Marker Empty =>
            new(-1, string.Empty);
    }
}

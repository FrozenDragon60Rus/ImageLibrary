using System;
using System.Collections.Generic;

namespace ImageDB.Table
{
    internal class Marker : Data
    {
        public int Id { get => (int)parameter[nameof(Id)]; }
        public string Name { get => parameter[nameof(Name)].ToString(); }
        public Marker(int Id, string Name) : base() =>
            parameter = new Dictionary<string, object>() {
                { nameof(Id), Id },
                { nameof(Name), Name }
            };
        public Marker(Dictionary<string, object> parameter) : base(parameter) =>
            this.parameter = parameter;
        public Marker() =>
            parameter = Empty.parameter;

        public static Marker Empty =>
            new Marker(-1, string.Empty);
    }
}

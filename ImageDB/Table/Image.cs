using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows;

namespace ImageDB.Table
{
    public class Image : Data
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get => (int)parameter[nameof(Id)]; }
        public string Address { get => parameter[nameof(Address)].ToString(); }
        public string Tag { get => parameter[nameof(Tag)].ToString(); }
        public string Character { get => parameter[nameof(Character)].ToString(); }
        public string Author { get => parameter[nameof(Author)].ToString(); }
        public byte Rating { get => System.Convert.ToByte(parameter[nameof(Rating)]); }

        public Image(int Id, string Address, byte Rating) : base() =>
            parameter = new Dictionary<string, object>() {
                { nameof(Id), Id },
                { nameof(Address), Address },
                { nameof(Rating), Rating }
            };
        public Image(Dictionary<string, object> parameter) : base(parameter) =>
            this.parameter = parameter;
        public Image(string Address) : base() =>
            parameter = new Dictionary<string, object>() {
                { "Id", -1 },
                { "Address", Address },
                { "Rating", 0 },
                { "Tag", string.Empty },
                { "Character", string.Empty },
                { "Author", string.Empty }
            };
        public Image() =>
            parameter = Empty.parameter;

        public static Image Empty =>
            new Image(string.Empty);
    }
}

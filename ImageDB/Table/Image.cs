using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;

namespace ImageDB.Table
{
    public class Image : Data
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get => System.Convert.ToInt32(Parameter[nameof(Id)]); }
        public string Address { get => Path.GetFileName(Parameter[nameof(Address)].ToString()); }
        public string Tag { get => Parameter[nameof(Tag)].ToString(); }
        public string Character { get => Parameter[nameof(Character)].ToString(); }
        public string Author { get => Parameter[nameof(Author)].ToString(); }
        public byte Rating { 
            get => System.Convert.ToByte(Parameter[nameof(Rating)]);
			set 
			{
				_ = byte.TryParse(value.ToString(), out byte number);

                Parameter[nameof(Rating)] = number < 11 ? number
                                                        : 10;
			}
		}

        public Image(int Id, string Address, byte Rating) : base() =>
            Parameter = new() {
                { nameof(Id), Id },
                { nameof(Address), Address },
                { nameof(Rating), Rating },
				{ "Tag", string.Empty },
				{ "Character", string.Empty },
				{ "Author", string.Empty }
			};
        public Image(Dictionary<string, object> parameter) : base(parameter) { }
        public Image(string Address) : base() =>
            Parameter = new() {
                { "Id", -1 },
                { "Address", Address },
                { "Rating", 0 },
                { "Tag", string.Empty },
                { "Character", string.Empty },
                { "Author", string.Empty }
            };
        public Image() : base() => Parameter = Empty.Parameter;

        public static Image Empty =>
            new(string.Empty);
    }
}

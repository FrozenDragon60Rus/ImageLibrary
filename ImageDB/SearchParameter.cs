using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    public class SearchParameter
    {
        public enum RatingStatus
        {
            From,
            Standart
        }
        public List<string> tag;
        public List<string> character;
        public List<string> author;
        static List<byte> rating;
        RatingStatus status;

        public SearchParameter()
        {
            tag = new List<string>();
            character = new List<string>();
            author = new List<string>();
            rating = new List<byte>();
            status = RatingStatus.Standart;
        }
    }
}

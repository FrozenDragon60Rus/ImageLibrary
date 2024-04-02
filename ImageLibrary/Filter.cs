using System.Collections.Generic;

namespace ImageLibrary
{
    public class Filter
    {
        public enum RatingStatus
        {
            From,
            Standart
        }
        public List<string> Tag { 
            get => Marker["Tag"]; 
            set => Marker["Tag"] = value; }
        public List<string> Character { 
            get => Marker["Character"]; 
            set => Marker["Character"] = value; }
		public List<string> Author { 
            get => Marker["Author"]; 
            set => Marker["Author"] = value; }
		public List<byte> Rating { get; set; }
        public readonly Dictionary<string, List<string>> Marker = [];
		public RatingStatus status;

        public Filter()
        {
            Marker.Add("Tag", []);
			Marker.Add("Character", []);
			Marker.Add("Author", []);
			Rating = [];

            status = RatingStatus.Standart;
        }
    }
}

using Newtonsoft.Json;
namespace Shares.Model
{
    public class BSE_Metadata
    {
        public string code{ get; set; }
        public string name { get; set; } = "Symbol not found";
        public string description{ get; set; }
        public string refreshed_at{ get; set; }
        public string from_date{ get; set; }
        public string to_date{ get; set; }

    }
}

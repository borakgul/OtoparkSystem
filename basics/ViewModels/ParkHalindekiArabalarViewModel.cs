using basics.Models;

namespace basics.ViewModels
{
    public class ParkHalindekiArabalarViewModel
    {
        public IEnumerable<BölgeGiris> ParkHalindekiArabalar { get; set; }
        public IEnumerable<BosKapasiteViewModel> BosKapasiteler { get; set; }
    }

    public class BosKapasiteViewModel
    {
        public int BölgeID { get; set; }
        public string OtoparkAdi { get; set; }
        public int KücükBos { get; set; }
        public int OrtaBos { get; set; }
        public int BüyükBos { get; set; }
    }
}
    
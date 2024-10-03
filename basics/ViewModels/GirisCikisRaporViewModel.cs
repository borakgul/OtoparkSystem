namespace basics.ViewModels
{
    public class GirisCikisRaporViewModel
    {
        public int ArabaId { get; set; }
        public string ArabaPlaka { get; set; }
        public string ArabaBoyut { get; set; }
        public string OtoparkAdi { get; set; }
        public double Ucret { get; set; }
        public DateTime GirisZamani { get; set; }
        public DateTime CikisZamani { get; set; }
    }

    public class OtoparkKazancRaporuViewModel
    {
        public string OtoparkAdi { get; set; }
        public double ToplamKazanc { get; set; }
    }
}
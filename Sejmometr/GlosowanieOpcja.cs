namespace Sejmometr
{
    public class GlosowanieOpcja
    {
        public int VotingNumber { get; set; } 
        public string Title { get; set; }
        public string Date { get; set; }

        public string OpisWyswietlany => $"[{VotingNumber}] {Title} ({Date})";
    }
}
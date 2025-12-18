using System.Collections.Generic;

namespace Sejmometr
{
    public class Glosowanie
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public List<Glos> Votes { get; set; }
    }

    public class Glos
    {
        public int MP { get; set; }
        public string Vote { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sejmometr
{
    public class Posel
    {
        public int Id { get; set; }

        public string FirstLastName { get; set; }
        public string Club { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {FirstLastName} ({Club})";
        }
    }
}

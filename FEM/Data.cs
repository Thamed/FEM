using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{
    public class Data
    {
        public double rmin;
        public double rmax; //promień maksymalny wsadu
        public double alfaair; // współczynnik kkonwekcyjnej wymiany ciepła
        public double tempbegin; // temperatura poczatkowa
        public double tempair; // temperatura otoczenia
        public double c; // efektywne ciepło właściwe
        public double ro; // gęstość metalu
        public double K;  // współczynnik przewodzenia ciepła
        public double taumax; // czas procesu
        public double a;
    }
}

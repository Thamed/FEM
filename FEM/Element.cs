using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{
    public class Element
    {
        public Node node1;
        public Node node2;
        public double[,] localH = new double[2,2];
        public double[] localP = new double[2];
        public double c;
        public double ro;
        public double k;
    }
}

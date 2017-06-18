using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNancy.Generic.Data.Core
{
    public class TwoResultSets<T1, T2>
    {
        public List<T1> ResultSet1 { get; set; }
        public List<T2> ResultSet2 { get; set; }
    }
}

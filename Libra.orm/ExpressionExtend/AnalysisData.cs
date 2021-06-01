using System;
using System.Collections.Generic;
using System.Text;

namespace Libra.orm.ExpressionExtend
{
    public class AnalysisData
    {
        public Dictionary<string, AnalysisTable> TableList { get; set; }

        public List<string> StackList { get; set; }

        public Dictionary<string, object> ParamList { get; set; }
    }
}

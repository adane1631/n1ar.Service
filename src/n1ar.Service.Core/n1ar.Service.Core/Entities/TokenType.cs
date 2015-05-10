using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n1ar.Service.Core.Entities {
    public enum TokenType {
        Invalid,
        Number,
        Function,
        FunctionArgSeparator,
        Operator,
        LeftParenthesis,
        RightParenthesis
    }
}

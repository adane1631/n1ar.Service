using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n1ar.Service.Core.Evaluators {
    public interface IEvaluator<T> {
        string Evaluate(T expression);
    }
}

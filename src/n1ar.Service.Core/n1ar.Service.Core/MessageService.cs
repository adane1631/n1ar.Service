using n1ar.Service.Core.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace n1ar.Service.Core {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class MessageService : IMessageService {
        public string GetFormattedFormula(string value) {
            try {
                IEvaluator<string> evaluator = new OrderOfOperationRPNEvaluator();
                string ouput = evaluator.Evaluate(value);

                return string.Format("You entered: {0}; Output was: {1}", value, ouput);
            }
            catch (ArgumentException ex) {
                return string.Format("An error has ocurred: {0}", ex.Message);
            }
        }
    }
}

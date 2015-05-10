using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace n1ar.Service.Core {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IMessageService {
        [OperationContract]
        string GetFormattedFormula(string value);

        // TODO: Add your service operations here
    }
}

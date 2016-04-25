using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hik.Communication.Scs.Client;

namespace Eucalyptus
{
    public class EucaClient
    {
        public IScsClient Client;
        public List<IMessageExecutor> Executors = new List<IMessageExecutor>();
        public List<IQuerySender> Senders = new List<IQuerySender>();
        public long Address = -1, LastSenderAddress = -1, DialogSenderAddress = -1;

        public EucaClient()
        {
            if (!new FileInfo(Environment.CurrentDirectory + "\\name.cfg").Exists)
                DataManager.SetName("система");
        }

    }

}

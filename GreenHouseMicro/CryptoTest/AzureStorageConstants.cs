using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTest
{
    public static class AzureStorageConstants
    {
        public static readonly string SharedKeyAuthorizationScheme = "SharedKeyLite";       


        public static readonly string Account = "oleksandr1xxxxxxxx";

        //https://STORAGE_ACCOUNT.table.core.windows.net/
        public static readonly string TableEndPoint = string.Format( "https://{0}.table.core.windows.net/",Account);


        public static readonly string StorageAccountKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
    }
}

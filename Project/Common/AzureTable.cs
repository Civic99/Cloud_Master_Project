using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class AzureTable
    {
        public static TableClient GetTableClient(string tableName)
        {
            var connectionString = "UseDevelopmentStorage=true";
            var serviceClient = new TableServiceClient(connectionString);

            return serviceClient.GetTableClient(tableName);
        }
    }
}

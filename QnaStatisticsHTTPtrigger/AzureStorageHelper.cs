using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QnaStatisticsHTTPtrigger
{
    /// <summary>
    /// Contains Azure table storage utility methods.
    /// </summary>
    public static class AzureStorageHelper
    {
        /// <summary>
        /// Tries to resolve a table in the storage defined by the given connection string and table name.
        /// </summary>
        /// <param name="connectionString">The Azure storage connection string.</param>
        /// <param name="tableName">The name of the table to resolve and return.</param>
        /// <returns>The resolved table or null in case of an error.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static CloudTable GetTable(string connectionString, string tableName)
        {
            CloudStorageAccount cloudStorageAccount = null;

            try
            {
                cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch
            {
                throw;
            }

            CloudTableClient cloudTableClient = cloudStorageAccount?.CreateCloudTableClient();
            return cloudTableClient?.GetTableReference(tableName);
        }

        
    }
}
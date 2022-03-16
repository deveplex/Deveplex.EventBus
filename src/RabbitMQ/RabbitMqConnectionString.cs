/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus.RabbitMQ
{
    internal sealed class RabbitMqConnectionString : EventBusConnectionString
    {
        // keys must be lowercase!
        internal static class KEY
        {
            internal const string Server = "server";
            internal const string Timeout = "timeout";
            internal const string Connection_Reset = "connection reset";
            internal const string Encrypt = "encrypt";
            internal const string Enlist = "enlist";
            internal const string Initial_Catalog = "initial catalog";
            internal const string Integrated_Security = "integrated security";
            internal const string MARS = "multipleactiveresultsets";
            internal const string Max_Pool_Size = "max pool size";
            internal const string Min_Pool_Size = "min pool size";
            internal const string MultiSubnetFailover = "multisubnetfailover";
            internal const string Packet_Size = "packet size";
            internal const string Password = "password";
            internal const string Pooling = "pooling";
            internal const string User_ID = "userid";
            internal const string Connect_Retry_Count = "retrycount";
        }

        // Constant for the number of duplicate options in the connnection string
        private static class SYNONYM
        {
            // connect timeout
            internal const string CONNECTION_TIMEOUT = "connection timeout";
            internal const string CONNECT_TIMEOUT = "connect timeout";
            // current language
            internal const string LANGUAGE = "language";
            // data source
            internal const string ADDR = "addr";
            internal const string ADDRESS = "address";
            internal const string DATA_SOURCE = "data source";
            internal const string NETWORK_ADDRESS = "network address";
            // initial catalog
            internal const string DATABASE = "database";
            // password
            internal const string Pwd = "pwd";
            // user id
            internal const string UID = "uid";
            internal const string User = "user";

            // make sure to update SynonymCount value below when adding or removing synonyms
        }

        private static Dictionary<string, string> _sqlClientSynonyms;

        internal RabbitMqConnectionString(string connectionString) : base(connectionString, GetParseSynonyms())
        {
        }

        // this hashtable is meant to be read-only translation of parsed string
        // keywords/synonyms to a known keyword string
        internal static IDictionary<string, string> GetParseSynonyms()
        {

            Dictionary<string, string> hash = _sqlClientSynonyms;
            if (null == hash)
            {
                hash = new Dictionary<string, string>();
                hash.Add(KEY.Connection_Reset, KEY.Connection_Reset);
                hash.Add(KEY.Encrypt, KEY.Encrypt);
                hash.Add(KEY.Enlist, KEY.Enlist);
                //hash.Add(KEY.Initial_Catalog, KEY.Initial_Catalog);
                hash.Add(KEY.Integrated_Security, KEY.Integrated_Security);
                hash.Add(KEY.MARS, KEY.MARS);
                hash.Add(KEY.Max_Pool_Size, KEY.Max_Pool_Size);
                hash.Add(KEY.Min_Pool_Size, KEY.Min_Pool_Size);
                hash.Add(KEY.MultiSubnetFailover, KEY.MultiSubnetFailover);
                hash.Add(KEY.Packet_Size, KEY.Packet_Size);
                hash.Add(KEY.Password, KEY.Password);
                hash.Add(KEY.Pooling, KEY.Pooling);
                hash.Add(KEY.Server, KEY.Server);
                hash.Add(KEY.Timeout, KEY.Timeout);
                hash.Add(KEY.User_ID, KEY.User_ID);
                hash.Add(KEY.Connect_Retry_Count, KEY.Connect_Retry_Count);

                hash.Add(SYNONYM.CONNECTION_TIMEOUT, KEY.Timeout);
                hash.Add(SYNONYM.CONNECT_TIMEOUT, KEY.Timeout);
                hash.Add(SYNONYM.ADDR, KEY.Server);
                hash.Add(SYNONYM.ADDRESS, KEY.Server);
                hash.Add(SYNONYM.NETWORK_ADDRESS, KEY.Server);
                hash.Add(SYNONYM.DATA_SOURCE, KEY.Server);
                //hash.Add(SYNONYM.DATABASE, KEY.Initial_Catalog);
                hash.Add(SYNONYM.Pwd, KEY.Password);
                hash.Add(SYNONYM.UID, KEY.User_ID);
                hash.Add(SYNONYM.User, KEY.User_ID);
                //Debug.Assert(SqlConnectionStringBuilder.KeywordsCount + SynonymCount == hash.Count, "incorrect initial ParseSynonyms size");
                _sqlClientSynonyms = hash;
            }
            return hash;
        }
    }
}

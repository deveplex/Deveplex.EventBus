/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Deveplex.EventBus
{
    public class EventBusConnectionString
    {
        private const string ConnectionStringValidKeyPattern = "^(?![;\\s])[^\\p{Cc}]+(?<!\\s)$"; // key not allowed to start with semi-colon or space or contain non-visible characters or end with space
        private const string ConnectionStringValidValuePattern = "^[^\u0000]*$";                    // value not allowed to contain embedded null
        internal const string DataDirectory = "|datadirectory|";

        private static readonly Regex _connectionStringValidKeyRegex = new Regex(ConnectionStringValidKeyPattern, RegexOptions.Compiled);
        private static readonly Regex _connectionStringValidValueRegex = new Regex(ConnectionStringValidValuePattern, RegexOptions.Compiled);

        private readonly string _usersConnectionString;
        private readonly Dictionary<string, string> _parsetable;

        public EventBusConnectionString(string connectionString, IDictionary<string, string> synonyms)
        {
            _usersConnectionString = ((null != connectionString) ? connectionString : "");
            _parsetable = new Dictionary<string, string>();

            // first pass on parsing, initial syntax check
            if (0 < _usersConnectionString.Length)
            {
                ParseInternal(_parsetable, _usersConnectionString, true, synonyms, false);
            }
        }

        public string this[string keyword] => _parsetable.ContainsKey(keyword) ? _parsetable[keyword] : null;

        private void ParseInternal(Dictionary<string, string> parsetable, string connectionString, bool buildChain, IDictionary<string, string> synonyms, bool firstKey)
        {
            var keyvalues = ParseConnectionString(connectionString, buildChain, firstKey);
            foreach (var it in keyvalues)
            {
                string keyname = it.Key;
                string keyvalue = it.Value;

                ValidateKeyValuePair(keyname, keyvalue);

                string synonym;
                string realkeyname = null != synonyms ?
                    (synonyms.TryGetValue(keyname, out synonym) ? synonym : null) : keyname;

                if (!IsKeyNameValid(realkeyname))
                {
                    continue;
                    //throw new NotSupportedException(R.GetString(ResourceDescriber.KeywordNotSupported, keyname));
                }
                if (!firstKey || !parsetable.ContainsKey(realkeyname))
                {
                    parsetable[realkeyname] = keyvalue; // last key-value pair wins (or first)
                }
            }
        }

        protected virtual Dictionary<string, string> ParseConnectionString(string connectionString, bool buildChain, bool firstKey)
        {
            var parsetable = new Dictionary<string, string>();
            var connectionSplit = connectionString.Split(';');
            foreach (var jjjj in connectionSplit)
            {
                var keyvalues = jjjj.Split('=');
                if (keyvalues.Length < 2)
                {
                    continue;
                }

                string keyname = keyvalues[0];
                string keyvalue = keyvalues[1];

                if (!firstKey || !parsetable.ContainsKey(keyname))
                {
                    parsetable[keyname] = keyvalue; // last key-value pair wins (or first)
                }
            }
            return parsetable;
        }

        private static bool IsKeyNameValid(string keyname)
        {
            if (null != keyname)
            {
#if DEBUG
                bool compValue = _connectionStringValidKeyRegex.IsMatch(keyname);
                Debug.Assert(((0 < keyname.Length) && (';' != keyname[0]) && !char.IsWhiteSpace(keyname[0]) && (-1 == keyname.IndexOf('\u0000'))) == compValue, "IsValueValid mismatch with regex");
#endif
                // string.Contains(char) is .NetCore2.1+ specific
                return ((0 < keyname.Length) && (';' != keyname[0]) && !char.IsWhiteSpace(keyname[0]) && (-1 == keyname.IndexOf('\u0000')));
            }
            return false;
        }

        protected void ValidateKeyValuePair(string keyword, string value)
        {
            if ((null == keyword) || !_connectionStringValidKeyRegex.IsMatch(keyword))
            {
                throw new InvalidOperationException(String.Format("{0} 无效的键名", keyword));
            }
            if ((null != value) && !_connectionStringValidValueRegex.IsMatch(value))
            {
                throw new InvalidOperationException(String.Format("{0} 无效的值", keyword));
            }
        }
    }
}

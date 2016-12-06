using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace Organisation.IntegrationLayer
{
    internal class TokenMap
    {
        private Dictionary<string, SecurityToken> tokens = new Dictionary<string, SecurityToken>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void addToken(string key, SecurityToken token) {
            if (tokens.ContainsKey(key)) {
                tokens.Remove(key);
            }

            tokens.Add(key, token);
        }

        public SecurityToken getToken(string key) {
            SecurityToken token;
            tokens.TryGetValue(key, out token);

            if (token == null) {
                return token;
            }

            // do not return expired token
            if (token.ValidTo.CompareTo(DateTime.Now) <= 0)
            {
                token = null;
            }

            return token;
        }
    }
}

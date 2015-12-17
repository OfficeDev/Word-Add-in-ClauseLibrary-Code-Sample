// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.S2S.Tokens;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Represents a security token which contains multiple security keys that are generated using symmetric algorithms.
    /// </summary>
    public class MultipleSymmetricKeySecurityToken : SecurityToken
    {
        /// <summary>
        /// Initializes a new instance of the MultipleSymmetricKeySecurityToken class.
        /// </summary>
        /// <param name="keys">An enumeration of Byte arrays that contain the symmetric keys.</param>
        public MultipleSymmetricKeySecurityToken(IEnumerable<byte[]> keys)
            : this(UniqueId.CreateUniqueId(), keys)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultipleSymmetricKeySecurityToken class.
        /// </summary>
        /// <param name="tokenId">The unique identifier of the security token.</param>
        /// <param name="keys">An enumeration of Byte arrays that contain the symmetric keys.</param>
        public MultipleSymmetricKeySecurityToken(string tokenId, IEnumerable<byte[]> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            if (String.IsNullOrEmpty(tokenId))
            {
                throw new ArgumentException("Value cannot be a null or empty string.", "tokenId");
            }

            foreach (byte[] key in keys)
            {
                if (key.Length <= 0)
                {
                    throw new ArgumentException("The key length must be greater then zero.", "keys");
                }
            }

            id = tokenId;
            effectiveTime = DateTime.UtcNow;
            securityKeys = CreateSymmetricSecurityKeys(keys);
        }

        /// <summary>
        /// Gets the unique identifier of the security token.
        /// </summary>
        public override string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the cryptographic keys associated with the security token.
        /// </summary>
        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return securityKeys.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the first instant in time at which this security token is valid.
        /// </summary>
        public override DateTime ValidFrom
        {
            get { return effectiveTime; }
        }

        /// <summary>
        /// Gets the last instant in time at which this security token is valid.
        /// </summary>
        public override DateTime ValidTo
        {
            get
            {
                // Never expire
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the key identifier for this instance can be resolved to the specified key identifier.
        /// </summary>
        /// <param name="keyIdentifierClause">A SecurityKeyIdentifierClause to compare to this instance</param>
        /// <returns>true if keyIdentifierClause is a SecurityKeyIdentifierClause and it has the same unique identifier as the Id property; otherwise, false.</returns>
        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException("keyIdentifierClause");
            }

            // Since this is a symmetric token and we do not have IDs to distinguish tokens, we just check for the
            // presence of a SymmetricIssuerKeyIdentifier. The actual mapping to the issuer takes place later
            // when the key is matched to the issuer.
            if (keyIdentifierClause is SymmetricIssuerKeyIdentifierClause)
            {
                return true;
            }
            return base.MatchesKeyIdentifierClause(keyIdentifierClause);
        }

        #region private members

        private List<SecurityKey> CreateSymmetricSecurityKeys(IEnumerable<byte[]> keys)
        {
            List<SecurityKey> symmetricKeys = new List<SecurityKey>();
            foreach (byte[] key in keys)
            {
                symmetricKeys.Add(new InMemorySymmetricSecurityKey(key));
            }
            return symmetricKeys;
        }

        private string id;
        private DateTime effectiveTime;
        private List<SecurityKey> securityKeys;

        #endregion
    }
}

#region License 
// ClauseLibrary, https://github.com/OfficeDev/clauselibrary 
//   
// Copyright 2015(c) Microsoft Corporation 
//   
// All rights reserved. 
//   
// MIT License: 
//   
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
// following conditions: 
//   
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software. 
//   
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT 
// SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN 
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
// USE OR OTHER DEALINGS IN THE SOFTWARE. 
#endregion
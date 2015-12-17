// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.  
// See full license at the bottom of this file.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ClauseLibrary.Common
{
    /// <summary>
    /// Provides helper methods for encrypting and decrypting text.
    /// </summary>
    public static class EncryptionHelper
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private static readonly byte[] InitVectorBytes = Encoding.ASCII.GetBytes("pu5ln648jyr2xbpt");

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int Keysize = 256;

        /// <summary>
        /// Encrypts the specified plain text value with the specified pass phrase.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        public static string Encrypt(this string plainText, string passPhrase = "34087B9B4302456B9B4E68F74EADBEE2")
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
            {
                byte[] keyBytes = password.GetBytes(Keysize/8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, InitVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (
                                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor,
                                    CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts the specified cipher text value with the specified pass phrase.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        public static string Decrypt(this string cipherText, string passPhrase = "34087B9B4302456B9B4E68F74EADBEE2")
        {
            byte[] cipherTextBytes;
            try
            {
                cipherTextBytes = Convert.FromBase64String(cipherText);
            }
            catch (FormatException)
            {
                // not an encrypted string.
                return cipherText;
            }

            using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
            {
                byte[] keyBytes = password.GetBytes(Keysize/8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, InitVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (
                                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor,
                                    CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                try
                                {
                                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                                catch (CryptographicException)
                                {
                                    return string.Empty;
                                }
                            }
                        }
                    }
                }
            }
        }
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

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace PassGen
{
    public class PasswordGeneration
    {
        private int passwordLength;

        private List<char> numChars;
        private List<char> smallAlphaChars;
        private List<char> bigAlphaChars;
        private List<char> speczialKeys;

        private List<char> chars;
        private string salt;

        public PasswordGeneration(int pwLen, string strSalt, bool useNum, bool useAlpha, bool useSpecial)
        {
            passwordLength = pwLen;
            numChars = "0123456789".ToList();
            var alphas = "abcdefghijklmnopqrstuvwxyz";
            smallAlphaChars = alphas.ToList();
            bigAlphaChars = alphas.ToUpper().ToList();
            speczialKeys = @"@#&+*/\=-.!?:(){}<>".ToList();

            UpdatePwChars(useNum, useAlpha, useSpecial);
            salt = strSalt;
        }

        public void UpdatePwLen(int newPwLen)
        {
            passwordLength = newPwLen;
        }

        public void UpdateSalt(string newSalt)
        {
            salt = newSalt;
        }

        public void UpdatePwChars(bool useNum, bool useAlpha, bool useSpecial)
        {
            chars = new List<char>();
            if (useAlpha)
            {
                chars.AddRange(smallAlphaChars);
                chars.AddRange(bigAlphaChars);
            }
            if (useNum)
            {
                chars.AddRange(numChars);
            }
            if (useSpecial)
            {
                chars.AddRange(speczialKeys);
            }
        }


        public string Generate(string masterKey, string sitekey)
        {
            var part1 = GetHash(salt + masterKey + sitekey);
            var part2 = GetHash(sitekey + masterKey + salt);
            var part3 = GetHash(masterKey + salt + sitekey);

            var pw1 = HashToPw(part1);
            var pw2 = HashToPw(part2);
            var pw3 = HashToPw(part3);

            var endPW = BuildSingleString(pw1, pw2, pw3);
            
            return endPW.Substring(0, passwordLength);
        }

        private ulong GetHash(string passpharse)
        {
            HashAlgorithmProvider algProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
           
            CryptographicHash hash = algProvider.CreateHash();
            
            IBuffer buffMsg1 = CryptographicBuffer.ConvertStringToBinary(passpharse, BinaryStringEncoding.Utf16BE);
            hash.Append(buffMsg1);
            byte[] hashByteArray;
            CryptographicBuffer.CopyToByteArray(hash.GetValueAndReset(), out hashByteArray);

            return BitConverter.ToUInt64(hashByteArray, 0);
        }

        private string HashToPw(ulong hashAsInt)
        {
            var finalPass = "";
            while (hashAsInt > 0) // && finalPass.Length < passwordLength)
            {
                int pos = (int)(hashAsInt % (ulong)chars.Count);
                finalPass += chars[pos];
                hashAsInt = hashAsInt / (ulong)(chars.Count / 2);
            }

            return finalPass;
        }

        private string BuildSingleString(string part1, string part2, string part3)
        {
           var length = GetLongestString(part1, part2, part3);
            var singlestring = "";
            for (int i = 0; i < length; i++)
            {
                singlestring += GetCharAtIndex(part1, i);
                singlestring += GetCharAtIndex(part2, i);
                singlestring += GetCharAtIndex(part3, i);
            }

            return singlestring;
        }

        private string GetCharAtIndex(string masterkey, int i)
        {
            var charAtIndex = "";
            try
            {
                charAtIndex += masterkey[i];
            }
            catch (Exception)
            {
                return " ";
            }

            return charAtIndex;
        }

        private int GetLongestString(string masterkey, string sitekey, string salt)
        {
            var length = 0;
            if (masterkey.Length > sitekey.Length && masterkey.Length > salt.Length)
            {
                length = masterkey.Length;
            }
            else if (sitekey.Length > salt.Length)
            {
                length = sitekey.Length;
            }
            else
            {
                length = salt.Length;
            }

            return length;
        }
    }
}

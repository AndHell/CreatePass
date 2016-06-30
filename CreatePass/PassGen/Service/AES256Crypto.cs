using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace CreatePass
{
    public class AES256Crypto
    {
        public string Encrypt(string plaintext, string pw, string salt)
        {
            IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(plaintext, BinaryStringEncoding.Utf8);

            var keys = GetAES256Keys(pw, salt);

            // encrypt data buffer using symmetric key and derived salt material
            IBuffer resultBuffer = CryptographicEngine.Encrypt(keys.symmKey, plainBuffer, keys.SaltMaterial);

            return CryptographicBuffer.EncodeToBase64String(resultBuffer);
        }

        public string Decrypt(string encryptedData, string pw, string salt)
        {
            IBuffer cipherBuffer = CryptographicBuffer.DecodeFromBase64String(encryptedData);

            var keys = GetAES256Keys(pw, salt);
            
            // encrypt data buffer using symmetric key and derived salt material
            IBuffer resultBuffer = CryptographicEngine.Decrypt(keys.symmKey, cipherBuffer, keys.SaltMaterial);

            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, resultBuffer);
        }

        private AES256KeyHelper GetAES256Keys(string pw, string salt)
        {
            IBuffer pwBuffer = CryptographicBuffer.ConvertStringToBinary(pw, BinaryStringEncoding.Utf8);
            IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);

            // Derive key material for password size 32 bytes for AES256 algorithm
            KeyDerivationAlgorithmProvider keyDerivationProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha512);
            // using salt and 1000 iterations
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

            // create a key based on original key and derivation parmaters
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(keyMaterial);

            // derive buffer to be used for encryption salt from derived password key
            IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

            //string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
            //string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

            SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            // create symmetric key from derived password material
            CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

            return new AES256KeyHelper() { SaltMaterial = saltMaterial, symmKey = symmKey };
        }
    }

    public class AES256KeyHelper
    {
        public IBuffer SaltMaterial;
        public CryptographicKey symmKey;

    }
}
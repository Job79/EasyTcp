using System;
using EasyTcp.Encryption;
using NUnit.Framework;

namespace EasyTcp3.Test.Encryption
{
    public class EncryptAndDecrypt
    {
        [Test]
        public void TestEncryptionAndDecryption()
        {
            var encryption = new EasyEncrypt();
            var testData = "testData";
            var encrypted = encryption.Encrypt(testData);
            Console.WriteLine(encrypted);
            var decrypted = encryption.Decrypt(encrypted);
            Assert.AreEqual(testData,decrypted);
        }
    }
}
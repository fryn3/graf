using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FsmUtils
{
	public static class Encription
	{
		private static string _passPhrase = "k*]$EADN#&M*9k3aS)V<^8gQweXq4ec%";

		private static string _initVector = "qaqg1il9uzpczl88";

		private static int _keysize = 256;

		public static string Encrypt(string plainText)
		{
			byte[] initVectorBytes = Encoding.UTF8.GetBytes(_initVector);
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			PasswordDeriveBytes password = new PasswordDeriveBytes(_passPhrase, null);
			byte[] keyBytes = password.GetBytes(_keysize / 8);
			RijndaelManaged symmetricKey = new RijndaelManaged();
			symmetricKey.Mode = CipherMode.CBC;
			ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
			cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
			cryptoStream.FlushFinalBlock();
			byte[] cipherTextBytes = memoryStream.ToArray();
			memoryStream.Close();
			cryptoStream.Close();
			return Convert.ToBase64String(cipherTextBytes);
		}

		public static string Decrypt(string cipherText)
		{
			byte[] initVectorBytes = Encoding.UTF8.GetBytes(_initVector);
			byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
			PasswordDeriveBytes password = new PasswordDeriveBytes(_passPhrase, null);
			byte[] keyBytes = password.GetBytes(_keysize / 8);
			RijndaelManaged symmetricKey = new RijndaelManaged();
			symmetricKey.Mode = CipherMode.CBC;
			ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
			MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
			CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			byte[] plainTextBytes = new byte[cipherTextBytes.Length];
			int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			memoryStream.Close();
			cryptoStream.Close();
			return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
		}
	}
}

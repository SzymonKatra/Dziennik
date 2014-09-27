using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;

namespace Dziennik
{
    public class PasswordEncryption
    {
        private static readonly byte[] s_salt = { 230, 208, 11, 228, 231, 171, 162, 89, 71, 31, 45, 220, 152, 211, 135 };
        private static readonly byte[] s_xorKey = { 90, 83, 224, 28, 249, 135, 162, 239, 207, 147, 114, 132, 4, 208, 115, 58, 247, 43, 185, 99 };

        public static byte[] Encrypt(SecureString securePassword)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            byte[] buffer;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);

                IntPtr currentPointer = unmanagedString;

                int xorKeyIndex = 0;
                for (int i = 0; i < securePassword.Length * 2; i++)
                {
                    Marshal.WriteByte(currentPointer, (byte)(Marshal.ReadByte(currentPointer) ^ s_xorKey[xorKeyIndex]));
                    currentPointer = IntPtr.Add(currentPointer, 1);
                    xorKeyIndex = (xorKeyIndex >= s_xorKey.Length - 1 ? 0 : xorKeyIndex + 1);
                }

                buffer = new byte[securePassword.Length * 2];
                Marshal.Copy(unmanagedString,buffer,0,buffer.Length);

            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }

            return EncryptImpl(buffer);
        }
        public static byte[] Encrypt(string password)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(password);

            int xorKeyIndex = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= s_xorKey[xorKeyIndex];
                xorKeyIndex = (xorKeyIndex >= s_xorKey.Length - 1 ? 0 : xorKeyIndex + 1);
            }

            return EncryptImpl(buffer);
        }
        private static byte[] EncryptImpl(byte[] data)
        {
            byte[] dataBuffer = new byte[data.Length + s_salt.Length];
            Array.Copy(data, dataBuffer, data.Length);
            Array.Copy(s_salt, 0, dataBuffer, data.Length, s_salt.Length);

            byte[] hashResult;
            using (SHA512 sha = SHA512.Create())
            {
                hashResult = sha.ComputeHash(dataBuffer);
            }

            return hashResult;
        }

        public static bool Compare(SecureString securePassword, byte[] encrypted)
        {
            return CompareImpl(Encrypt(securePassword), encrypted);
        }
        public static bool Compare(string password, byte[] encrypted)
        {
            return CompareImpl(Encrypt(password), encrypted);
        }
        private static bool CompareImpl(byte[] compare, byte[] encrypted)
        {
            if (compare.Length != encrypted.Length) return false;

            for (int i = 0; i < encrypted.Length; i++)
            {
                if (compare[i] != encrypted[i]) return false;
            }

            return true;
        }
    }
}

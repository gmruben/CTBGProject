using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class EncryptionHelper
{
    public static string EncryptString(string ClearText)
    {
        string theKey = SystemInfo.deviceUniqueIdentifier;
        byte[] salt = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };
        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(theKey, salt);

        byte[] key = pdb.GetBytes(32);
        byte[] iv = pdb.GetBytes(16);

        byte[] clearTextBytes = Encoding.UTF8.GetBytes(ClearText);

        System.Security.Cryptography.SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, iv), CryptoStreamMode.Write);

        cs.Write(clearTextBytes, 0, clearTextBytes.Length);
        cs.Close();

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string DecryptString(string EncryptedText)
    {
        string theKey = SystemInfo.deviceUniqueIdentifier;
        byte[] salt = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };
        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(theKey, salt);

        byte[] key = pdb.GetBytes(32);
        byte[] iv = pdb.GetBytes(16);

        byte[] encryptedTextBytes = Convert.FromBase64String(EncryptedText);

        System.Security.Cryptography.SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, iv), CryptoStreamMode.Write);

        cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
        cs.Close();

        return Encoding.UTF8.GetString(ms.ToArray());
    }
}
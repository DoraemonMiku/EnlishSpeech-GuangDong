using System;


public class CheakerTools 
{
    /// <summary>
    /// Base64加密
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
   public static string Base64_Encode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }
    /// <summary>
    /// Base64解密
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Base64_Decode(string data)
    {
        return Convert.FromBase64String(data);
    }
}

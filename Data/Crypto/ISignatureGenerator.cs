namespace Data.Crypto
{
    public interface ISignatureGenerator
    {
        string GenerateHMACSHA256(string textToHash);
    }
}
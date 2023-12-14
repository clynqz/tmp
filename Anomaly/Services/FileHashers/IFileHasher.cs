namespace Anomaly.Services.FileHashers
{
    public interface IFileHasher
    {
        public string Hash(Stream fileStream);
    }
}

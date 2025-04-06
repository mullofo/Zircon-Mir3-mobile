namespace Patch
{
    public sealed class PKGInformation
    {
        public string APKVersion { get; set; }
        public string APKFileName { get; set; }
        public long APKCompressedLength { get; set; }
        public byte[] APKCheckSum { get; set; }
        public string BaseZipFileName { get; set; }
        public long BaseZipCompressedLength { get; set; }
        public byte[] BaseZipCheckSum { get; set; }
    }
}

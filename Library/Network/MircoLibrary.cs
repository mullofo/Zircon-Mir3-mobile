namespace Library.Network
{
    public class MicroLibraryImage
    {
        /// <summary>
        /// 素材便移位置
        /// </summary>
        public int Postion { get; set; }
        /// <summary>
        /// 对应index信息
        /// </summary>
        public byte[] ImageData { get; set; }

        public MicroLibraryImage()
        { }

        public MicroLibraryImage(System.IO.BinaryReader br)
        {
            Postion = br.ReadInt32();
            ImageData = br.ReadBytes(br.ReadInt32());
        }
    }
    public class MicroLibraryHeader
    {
        /// <summary>
        /// 素材文件总长度
        /// </summary>
        public long TotalLength { get; set; }
        /// <summary>
        /// 头信息
        /// </summary>
        public byte[] HeaderBytes { get; set; }
        public MicroLibraryHeader()
        { }

        public MicroLibraryHeader(System.IO.BinaryReader br)
        {
            TotalLength = br.ReadInt64();
            HeaderBytes = br.ReadBytes(br.ReadInt32());
        }
    }
    public class MicroSound
    {
        public int Current { get; set; }
        public int Max { get; set; }
        public byte[] Bytes { get; set; }
    }

}

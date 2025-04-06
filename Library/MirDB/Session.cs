//读取时解密
#define DECRYPT_WHEN_READ 
//写入时加密
#define ENCRYPT_WHEN_WRITE 


using Library.MirDB;
using Library.MirDB.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MirDB
{
    /// <summary>
    /// 密匙
    /// </summary>
    public sealed partial class Session
    {
        // todo 敏感数据应该用ProtectedData类加以保护 
        // 注意: 以下两组key应该完全相同，仅当需要读取他人加密数据库等情况时，才需要修改

        /// <summary>
        /// 原始库
        /// </summary>
        // 长度为32,用于生成key,供读取数据库时解密用
        //private const string AES_READ_KEYPHRASE = "aYXRiOjcx59dgyoyIpZXN4qbUqAJJl0V";
        // 长度为16，用于生成初始化向量,供读取数据库时解密用
        //private const string IV_READ = "9jYzdWjS8HxavETy";
        // 长度为32,用于生成key,供写入数据库时加密用
        //private const string AES_WRITE_KEYPHRASE = "aYXRiOjcx59dgyoyIpZXN4qbUqAJJl0V";
        // 长度为16，用于生成初始化向量,供写入数据库时加密用
        //private const string IV_WRITE = "9jYzdWjS8HxavETy";

        /// <summary>
        /// 145基础库
        /// </summary>
        // 长度为32,用于生成key,供读取数据库时解密用
        //private const string AES_READ_KEYPHRASE = "NczzClsexzGOnznT1F7smVtSspZvaKu2";
        // 长度为16，用于生成初始化向量,供读取数据库时解密用
        //private const string IV_READ = "FcfNZtzq2iWNdPWK";
        // 长度为32,用于生成key,供写入数据库时加密用
        //private const string AES_WRITE_KEYPHRASE = "NczzClsexzGOnznT1F7smVtSspZvaKu2";
        // 长度为16，用于生成初始化向量,供写入数据库时加密用
        //private const string IV_WRITE = "FcfNZtzq2iWNdPWK";

        /// <summary>
        /// 韩版基础库
        /// </summary>
        // 长度为32,用于生成key,供读取数据库时解密用
        //private const string AES_READ_KEYPHRASE = "mKoQmrbwKPDwsTPvAzt9ftkB62mjUrxb";
        // 长度为16，用于生成初始化向量,供读取数据库时解密用
        //private const string IV_READ = "kvACvsWDkTREVKfV";
        // 长度为32,用于生成key,供写入数据库时加密用
        //private const string AES_WRITE_KEYPHRASE = "mKoQmrbwKPDwsTPvAzt9ftkB62mjUrxb";
        // 长度为16，用于生成初始化向量,供写入数据库时加密用
        //private const string IV_WRITE = "kvACvsWDkTREVKfV";

        /// <summary>
        /// 全新多版本统一定制库
        /// </summary>
        // 长度为32,用于生成key,供读取数据库时解密用
        private const string AES_READ_KEYPHRASE = "BHxWVSM5tBY3pJQqctgl2wbRZhNih9yj";
        // 长度为16，用于生成初始化向量,供读取数据库时解密用
        private const string IV_READ = "AhQPfqQIyuxecllz";
        // 长度为32,用于生成key,供写入数据库时加密用
        private const string AES_WRITE_KEYPHRASE = "BHxWVSM5tBY3pJQqctgl2wbRZhNih9yj";
        // 长度为16，用于生成初始化向量,供写入数据库时加密用
        private const string IV_WRITE = "AhQPfqQIyuxecllz";

        /// <summary>
        /// 用于生成key,供读取数据库时解密用
        /// </summary>
        private byte[] AES_READ_KEY { get; set; }
        /// <summary>
        /// 用于生成初始化向量,供读取数据库时解密用
        /// </summary>
        private byte[] READ_IV { get; set; }
        /// <summary>
        /// 用于生成key,供写入数据库时加密用
        /// </summary>
        private byte[] AES_WRITE_KEY { get; set; }
        /// <summary>
        /// 用于生成初始化向量,供写入数据库时加密用
        /// </summary>
        private byte[] WRITE_IV { get; set; }

        private const string Extention = @".db";
        private const string TempExtention = @".TMP";
        private const string CompressExtention = @".gz";

        public string Root { get; private set; }
        public SessionMode Mode { get; private set; }
        public bool BackUp { get; set; } = true;
        public int BackUpDelay { get; set; }
        public string BackupRoot { get; set; }

        public bool XOREncrypt { get; set; }
        public string XORKey { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public string SystemFileName = "System";
        public string SystemPath => Root + SystemFileName + (IsMySql ? "MySql" : "") + Extention;
        private string SystemBackupPath => BackupRoot + @"System/";
        private byte[] SystemHeader;

        private string UsersPath => Root + "Users" + (IsMySql ? "MySql" : "") + Extention;
        private string UsersBackupPath => BackupRoot + @"Users/";
        private byte[] UsersHeader;

        private string ClientSystemFileName = "ClientSystem";
        public string ClientSystemPath => Root + ClientSystemFileName + ".db";
        private byte[] ClientSystemHeader;

        public bool IsMySql;
        public string[] MySqlParms;

        /// <summary>
        /// 异常时的事件处理输出
        /// </summary>
        public EventHandler<string> Output;

        internal Dictionary<Type, DBRelationship> Relationships = new Dictionary<Type, DBRelationship>();

        private Dictionary<Type, ADBCollection> Collections;

        /// <summary>
        /// 数据库对角所在程序集
        /// </summary>
        public Assembly[] Assemblies { get; private set; }

        public Session(SessionMode mode, Assembly[] assemblies, bool enableXorEncrypt, string xorKey = "", string root = @"./Database/", string backup = @"./Backup/")
        {
            Assemblies = assemblies;

            Root = root;
            BackupRoot = backup;
            Mode = mode;

            //如果是客户端就加载ClientSystem.db文件
            if (Mode == SessionMode.Client)
                SystemFileName = ClientSystemFileName;

            XOREncrypt = enableXorEncrypt;
            XORKey = xorKey;
            //初始化AES密钥
            InitializeCrypto();
        }
        /// <summary>
        /// 初始化加密
        /// </summary>
        private void InitializeCrypto()
        {
            AES_READ_KEY = System.Text.Encoding.ASCII.GetBytes(AES_READ_KEYPHRASE);
            READ_IV = System.Text.Encoding.ASCII.GetBytes(IV_READ);

            AES_WRITE_KEY = System.Text.Encoding.ASCII.GetBytes(AES_WRITE_KEYPHRASE);
            WRITE_IV = System.Text.Encoding.ASCII.GetBytes(IV_WRITE);
        }
        public void Init()
        {
#if SERVER || ServerTool
            if (IsMySql)
                InitializeSQL();
            else
                Initialize();
#else
            Initialize();
#endif
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            Collections = new Dictionary<Type, ADBCollection>();

            List<Type> types = Assemblies
               .Select(x => x.GetTypes())
               .SelectMany(x => x)
               .ToList();

            Type collectionType = typeof(DBCollection<>);

            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(typeof(DBObject))) continue;

                Collections[type] = (ADBCollection)Activator.CreateInstance(collectionType.MakeGenericType(type), this);
            }

            //System库全部要加载
            //客户端在实例化Session的时候已经将SystemFileName替换ClientSystemFileName，所以客户端加载的是ClientSystem.db文件
            InitializeSystem();
            //初始化ClientSystemmapping信息，  只有servertool工具才需要，因为保存要用到
            if (Mode == SessionMode.ServerTool)
                InitializeClientSystemMapping();

            //User库全部要加载
            InitializeUsers();

            Parallel.ForEach(Relationships, x => x.Value.ConsumeKeys(this));

            Relationships = null;
            /*
            DBCollection<ItemInfo> itemList = GetCollection<ItemInfo>();

            ItemInfo Female = (ItemInfo)itemList.GetObjectByIndex(1100);
            ItemInfo Male = (ItemInfo)itemList.GetObjectByIndex(1043);

            int maleIndex = itemList.Binding.IndexOf(Male );
            Female.Index = 1044;
            itemList.Binding.Remove(Female);
            itemList.Binding.Insert(maleIndex  + 1, Female);
               */

            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                pair.Value.OnLoaded();
        }
#if SERVER || ServerTool
        #region MYSQL
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitializeSQL()
        {
            Collections = new Dictionary<Type, ADBCollection>();

            List<Type> types = Assemblies
               .Select(x => x.GetTypes())
               .SelectMany(x => x)
               .ToList();

            Type collectionType = typeof(DBCollection<>);

            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(typeof(DBObject))) continue;

                Collections[type] = (ADBCollection)Activator.CreateInstance(collectionType.MakeGenericType(type), this);
            }

            InitializeSystemSQL();
            //初始化ClientSystemmapping信息，  只有servertool工具才需要，因为保存要用到
            if (Mode == SessionMode.ServerTool)
                InitializeClientSystemMapping();

            InitializeUsersSQL();

            Parallel.ForEach(Relationships, x => x.Value.ConsumeKeys(this));

            Relationships = null;

            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                pair.Value.OnLoaded();


        }
        private void InitializeSystemSQL()
        {
            List<DBMapping> mappings = new List<DBMapping>();
            //反射所有systemdb的类保存到SystemHeader，只有servertool工具才需要保存这些类属性
            if (Mode == SessionMode.ServerTool)
            {
                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (!pair.Value.IsSystemData) continue;
                    mappings.Add(pair.Value.Mapping);
                }

                // 初始化DB后amppings写入内存
                using (MemoryStream stream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(mappings.Count);
                    foreach (DBMapping mapping in mappings)
                        mapping.Save(writer);

                    SystemHeader = stream.ToArray();
                }
                mappings.Clear();
            }

            Parallel.ForEach(Collections, x =>
            {
                if (!x.Value.IsSystemData) return;
                x.Value.LoadSQL();
            });
        }

        private void InitializeUsersSQL()
        {
            List<DBMapping> mappings = new List<DBMapping>();
            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (pair.Value.IsSystemData) continue;
                mappings.Add(pair.Value.Mapping);
            }

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(mappings.Count);
                foreach (DBMapping mapping in mappings)
                    mapping.Save(writer);

                UsersHeader = stream.ToArray();
            }
            mappings.Clear();

            Parallel.ForEach(Collections, x =>
            {
                if (x.Value.IsSystemData) return;
                x.Value.LoadSQL();
            });
        }

        public void InitDataBase()
        {
            ////初始化表
            //SystemDB
            Parallel.ForEach(Collections, x =>
            {
                if (!x.Value.IsSystemData) return;
                x.Value.InitObjectToSQL();
            });
            //UserDB
            Parallel.ForEach(Collections, x =>
            {
                if (x.Value.IsSystemData) return;
                x.Value.InitObjectToSQL();
            });

            //创建外键
            //SystemDB
            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (!pair.Value.IsSystemData) continue;
                pair.Value.InitForeignKey();
            }
            //UserDB
            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (pair.Value.IsSystemData) continue;
                pair.Value.InitForeignKey();
            }
        }
        /// <summary>
        /// 保存System.db
        /// </summary>
        private void SaveSystemSQL()
        {
            //只有servertool才需要保存systemdb库
            if (Mode != SessionMode.ServerTool) return;

            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (!pair.Value.IsSystemData) continue;
                pair.Value.SaveObjectsSQL();

            }
        }
        /// <summary>
        /// 保存Users.db
        /// </summary>
        private void SaveUsersSQL()
        {
            //servertool工具不保存user库
            if (Mode == SessionMode.ServerTool) return;

            //单线程遍历
            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (pair.Value.IsSystemData) continue;
                pair.Value.SaveObjectsSQL();
            }

        }
        /// <summary>
        /// MySql数据转换到Z版二进制文件
        /// </summary>
        public bool SqlToFile()
        {
            try
            {
                Parallel.ForEach(Collections, x => x.Value.SqlToFile());

                SaveSystem();
                SaveClientSystem();
                SaveUsers();
                return true;
            }
            catch (Exception ex)
            {
                Output?.Invoke(null, ex.ToString());
                return false;
            }

        }
        #endregion
#endif
        /// <summary>
        /// 初始化系统
        /// </summary>
        private void InitializeSystem()
        {
            List<DBMapping> mappings = new List<DBMapping>();

            //反射所有systemdb的类保存到SystemHeader，只有servertool工具才需要保存这些类属性
            if (Mode == SessionMode.ServerTool)
            {
                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (!pair.Value.IsSystemData) continue;

                    mappings.Add(pair.Value.Mapping);
                }

                // 初始化DB后amppings写入内存
                using (MemoryStream stream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(mappings.Count);
                    foreach (DBMapping mapping in mappings)
                        mapping.Save(writer);

                    SystemHeader = stream.ToArray();
                }

                mappings.Clear();
            }
            //注意：运行环境中一个完好的System.db必须存在
            if (!File.Exists(SystemPath)) return;

            //读取时解密
#if DECRYPT_WHEN_READ
            //读取System.db
            byte[] encryptedDB = File.ReadAllBytes(SystemPath);
            //因为XOR发生在AES加密之后 所以这里XOR要在AES解密之前
            if (XOREncrypt)
            {
                encryptedDB = Crypto.XORUtils.XOR(encryptedDB, XORKey);
            }
            //解密
            byte[] decryptedDB = Crypto.AesUtils.Decrypt(encryptedDB, AES_READ_KEY, READ_IV);
            using (MemoryStream tempMemoryStream = new MemoryStream(decryptedDB))
            {
                //解密应该在此之前完成
                using (BinaryReader reader = new BinaryReader(tempMemoryStream))
                {
                    //文件头，记录多少DBObject类
                    int count = reader.ReadInt32();

                    //文件类属性区域，明文记录了类名，属性名
                    //循环每个DBObject类，解析它包含的属性
                    for (int i = 0; i < count; i++)
                        mappings.Add(new DBMapping(Assemblies, reader));

                    List<Task> loadingTasks = new List<Task>();
                    //文件类属性值区域
                    //循环解析后的DBObject类，然后取值
                    foreach (DBMapping mapping in mappings)
                    {
                        //读取当前类的所有值保存到数组 reader.ReadInt32()为长度
                        byte[] data = reader.ReadBytes(reader.ReadInt32());

                        ADBCollection value;
                        if (mapping.Type == null || !Collections.TryGetValue(mapping.Type, out value)) continue;

                        //解析读取的数组给相应的属性赋值
                        loadingTasks.Add(Task.Run(() => value.Load(data, mapping)));
                    }

                    if (loadingTasks.Count > 0)
                        Task.WaitAll(loadingTasks.ToArray());
                }
            }
#else
			//使用(二进制读取 = 新的二进制读取(文件 打开读取(系统路径))
            using (BinaryReader reader = new BinaryReader(File.OpenRead(SystemPath)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                    mappings.Add(new DBMapping(Assemblies,reader));

                List<Task> loadingTasks = new List<Task>();
                foreach (DBMapping mapping in mappings)
                {
                    byte[] data = reader.ReadBytes(reader.ReadInt32());

                    ADBCollection value;
                    if (mapping.Type == null || !Collections.TryGetValue(mapping.Type, out value)) continue;

                    loadingTasks.Add(Task.Run(() => value.Load(data, mapping)));
                }

                if (loadingTasks.Count > 0)
                    Task.WaitAll(loadingTasks.ToArray());
            }
#endif

        }
        /// <summary>
        /// 初始化ClientSystem Mapping信息
        /// </summary>
        private void InitializeClientSystemMapping()
        {
            List<DBMapping> mappings = new List<DBMapping>();
            //反射所有ClientSystemdb的类保存到ClientSystemHeader，只有servertool工具才需要保存这些类属性
            if (Mode != SessionMode.ServerTool) return;

            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (!pair.Value.IsSystemData || !pair.Value.IsClientSystemData) continue;

                mappings.Add(pair.Value.ClientMapping);
            }

            // 初始化DB后amppings写入内存
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(mappings.Count);
                foreach (DBMapping mapping in mappings)
                    mapping.Save(writer);

                ClientSystemHeader = stream.ToArray();
            }

            mappings.Clear();
        }

        /// <summary>
        /// 初始化用户
        /// </summary>
        private void InitializeUsers()
        {
            List<DBMapping> mappings = new List<DBMapping>();

            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (pair.Value.IsSystemData) continue;

                mappings.Add(pair.Value.Mapping);
            }

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(mappings.Count);
                foreach (DBMapping mapping in mappings)
                    mapping.Save(writer);

                UsersHeader = stream.ToArray();
            }
            mappings.Clear();

            if (!File.Exists(UsersPath)) return;

            //读取User.db
            //不需要加密解密
            using (BinaryReader reader = new BinaryReader(File.OpenRead(UsersPath)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                    mappings.Add(new DBMapping(Assemblies, reader));

                List<Task> loadingTasks = new List<Task>();
                foreach (DBMapping mapping in mappings)
                {
                    byte[] data = reader.ReadBytes(reader.ReadInt32());

                    ADBCollection value;
                    if (mapping.Type == null || !Collections.TryGetValue(mapping.Type, out value)) continue;

                    loadingTasks.Add(Task.Run(() => value.Load(data, mapping)));
                }

                if (loadingTasks.Count > 0) //如果（加载任务计数>0）
                    Task.WaitAll(loadingTasks.ToArray()); //任务全部等待（将任务加载到阵列（））；
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="commit"></param>
        public void Save(bool commit, SessionMode mode)
        {
#if SERVER || ServerTool
            if (!IsMySql)
                Parallel.ForEach(Collections, x => x.Value.SaveObjects());
            else
                Parallel.ForEach(Collections, x => x.Value.SaveSQLParam());
#else
            Parallel.ForEach(Collections, x => x.Value.SaveObjects());
#endif
            if (commit)
                Commit(mode);
        }
        /// <summary>
        /// 提交
        /// </summary>
        public void Commit(SessionMode mode)
        {
#if SERVER || ServerTool
            if (!IsMySql)
            {
                //只有ServerTool才保存system和clientsystem库
                if (mode == SessionMode.ServerTool)
                {
                    SaveSystem();
                    SaveClientSystem();
                }
                //ServerTool工具不保存user库
                if (mode != SessionMode.ServerTool)
                    SaveUsers();
            }
            else
            {
                if (mode == SessionMode.ServerTool)
                    SaveSystemSQL();
                if (mode != SessionMode.ServerTool)
                    SaveUsersSQL();
            }
#else
            //只有ServerTool才保存system和clientsystem库
            if (mode == SessionMode.ServerTool)
            {
                SaveSystem();
                SaveClientSystem();
            }
            //ServerTool工具不保存user库
            if (mode != SessionMode.ServerTool)
                SaveUsers();
#endif
        }
        /// <summary>
        /// 保存System.db
        /// </summary>
        private void SaveSystem()
        {
            //只有servertool才需要保存systemdb库
            if (Mode != SessionMode.ServerTool) return;

            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            //写入时加密
#if ENCRYPT_WHEN_WRITE
            //先把明文写进内存流
            // using 内的流会被自动关闭
            using (MemoryStream tempMemoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(tempMemoryStream))
                {
                    writer.Write(SystemHeader);

                    foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                    {
                        if (!pair.Value.IsSystemData) continue;
                        byte[] data = pair.Value.GetSaveData();

                        writer.Write(data.Length);
                        writer.Write(data);
                    }
                }
                // tempMemoryStream准备完毕 对其加密
                byte[] beforeEncrypt = tempMemoryStream.ToArray();
                // todo IV不应该重复使用
                var encryptedDB = Crypto.AesUtils.Encrypt(beforeEncrypt, AES_WRITE_KEY, WRITE_IV);

                //这里已经拿到AES加密过的数据了 下面看是否XOR加密
                if (XOREncrypt)
                {
                    encryptedDB = Crypto.XORUtils.XOR(encryptedDB, XORKey);
                }

                //写入System.db文件
                File.WriteAllBytes((SystemPath + TempExtention), encryptedDB);
            }
#else
            using (BinaryWriter writer = new BinaryWriter(File.Create(SystemPath + TempExtention)))
            {
                writer.Write(SystemHeader);

                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (!pair.Value.IsSystemData) continue;
                    byte[] data = pair.Value.GetSaveData();

                    writer.Write(data.Length);
                    writer.Write(data);
                }
            }
#endif

            if (BackUp && !Directory.Exists(SystemBackupPath))
                Directory.CreateDirectory(SystemBackupPath);

            if (File.Exists(SystemPath))
            {
                if (BackUp)
                {
                    using (FileStream sourceStream = File.OpenRead(SystemPath))
                    using (FileStream destStream = File.Create(SystemBackupPath + "System " + ToBackUpFileName(DateTime.Now) + Extention + CompressExtention))  //修改数据库备份时区 DateTime.UtcNow
                    using (GZipStream compress = new GZipStream(destStream, CompressionMode.Compress))
                        sourceStream.CopyTo(compress);
                }

                File.Delete(SystemPath);
            }

            File.Move(SystemPath + TempExtention, SystemPath);
        }
        /// <summary>
        /// 保存ClientSystem.db
        /// </summary>
        private void SaveClientSystem()
        {
            //只有servertool才需要保存ClientSystemDB库
            if (Mode != SessionMode.ServerTool) return;

            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            //写入时加密
#if ENCRYPT_WHEN_WRITE
            //先把明文写进内存流
            // using 内的流会被自动关闭
            using (MemoryStream tempMemoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(tempMemoryStream))
                {
                    writer.Write(ClientSystemHeader);

                    foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                    {
                        if (!pair.Value.IsSystemData || !pair.Value.IsClientSystemData) continue;
                        byte[] data = pair.Value.GetClientSaveData();

                        writer.Write(data.Length);
                        writer.Write(data);
                    }
                }
                // tempMemoryStream准备完毕 对其加密
                byte[] beforeEncrypt = tempMemoryStream.ToArray();
                // todo IV不应该重复使用
                var encryptedDB = Crypto.AesUtils.Encrypt(beforeEncrypt, AES_WRITE_KEY, WRITE_IV);

                //这里已经拿到AES加密过的数据了 下面看是否XOR加密
                if (XOREncrypt)
                {
                    encryptedDB = Crypto.XORUtils.XOR(encryptedDB, XORKey);
                }

                //写入System.db文件
                File.WriteAllBytes((ClientSystemPath + TempExtention), encryptedDB);
            }
#else
            using (BinaryWriter writer = new BinaryWriter(File.Create(ClientSystemPath + TempExtention)))
            {
                writer.Write(ClientSystemHeader);

                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (!pair.Value.IsSystemData || !pair.Value.IsClientSystemData) continue;
                    byte[] data = pair.Value.GetClientSaveData();

                    writer.Write(data.Length);
                    writer.Write(data);
                }
            }
#endif

            if (File.Exists(ClientSystemPath))
                File.Delete(ClientSystemPath);
            File.Move(ClientSystemPath + ".TMP", ClientSystemPath);
        }
        /// <summary>
        /// 保存Users.db
        /// </summary>
        private void SaveUsers()
        {
            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            using (BinaryWriter writer = new BinaryWriter(File.Create(UsersPath + TempExtention)))
            {
                writer.Write(UsersHeader);

                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (pair.Value.IsSystemData) continue;

                    byte[] data = pair.Value.GetSaveData();

                    writer.Write(data.Length);
                    writer.Write(data);
                }
            }
            if (BackUp && !Directory.Exists(UsersBackupPath))
                Directory.CreateDirectory(UsersBackupPath);

            if (File.Exists(UsersPath))
            {
                if (BackUp)
                {
                    using (FileStream sourceStream = File.OpenRead(UsersPath))
                    using (FileStream destStream = File.Create(UsersBackupPath + "Users " + ToBackUpFileName(DateTime.Now) + Extention + CompressExtention))  //修改角色库备份时区 DateTime.UtcNow
                    using (GZipStream compress = new GZipStream(destStream, CompressionMode.Compress))
                        sourceStream.CopyTo(compress);
                }

                File.Delete(UsersPath);
            }

            File.Move(UsersPath + TempExtention, UsersPath);
        }

        /// <summary>
        /// 获取数据库集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DBCollection<T> GetCollection<T>() where T : DBObject, new()
        {
            return (DBCollection<T>)Collections[typeof(T)];
        }
        /// <summary>
        /// 获取数据库集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ADBCollection GetCollection(Type type)
        {
            return Collections[type];
        }
        /// <summary>
        /// 获取数据库对象索引
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal DBObject GetObject(Type type, int index)
        {
            return Collections[type].GetObjectByIndex(index);
        }
        /// <summary>
        /// 获取数据库对象名字
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns>DBObject</returns>
        public DBObject GetObject(Type type, string fieldName, object value)
        {
            return Collections[type].GetObjectbyFieldName(fieldName, value);
        }
        /// <summary>
        /// 从一个已有对象黏贴到新对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="oldOb"></param>
        /// <param name="newOb"></param>
        /// <returns></returns>
        public void PasteObject(Type type, DBObject oldOb, DBObject newOb)
        {
            Dictionary<DBObject, DBObject> subClass = new Dictionary<DBObject, DBObject>();
            Collections[type].PasteObject(oldOb, newOb, subClass);
            subClass.Clear();
        }
        /// <summary>
        /// 从一个对象二进制流黏贴到新对象
        /// </summary>
        /// <param name="br"></param>
        /// <param name="sourceMapping"></param>
        /// <param name="newOb"></param>
        /// <returns></returns>
        public void PasteObject(BinaryReader br, DBMapping sourceMapping, DBObject newOb)
        {
            ADBCollection value;
            if (!Collections.TryGetValue(sourceMapping.Type, out value)) return;
            Dictionary<DBObject, int> subClass = new Dictionary<DBObject, int>();
            value.PasteObject(br, sourceMapping, newOb, subClass);
            subClass.Clear();
        }
        /// <summary>
        /// 复制对象并序列化到数组
        /// </summary>
        /// <param name="obList"></param>
        /// <returns>byte[]</returns>
        public byte[] CopyObject(List<DBObject> obList)
        {
            if (obList == null || obList.Count == 0) return null;

            using (MemoryStream mem = new MemoryStream())
            using (BinaryWriter wr = new BinaryWriter(mem))
            {
                wr.Write(obList.Count);
                foreach (DBObject ob in obList)
                {
                    ob.Collection.Mapping.Save(wr);
                    wr.Write(ob.CopyObject());
                }
                return mem.ToArray();
            }
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T CreateObject<T>() where T : DBObject, new()
        {
            return (T)Collections[typeof(T)].CreateObject();
        }
        /// <summary>
        /// 备份字符串到文件名
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static string ToFileName(DateTime time)
        {
            return $"{time.Year:0000}-{time.Month:00}-{time.Day:00} {time.Hour:00}-{time.Minute:00}";
        }
        /// <summary>
        /// 备份文件名的字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private string ToBackUpFileName(DateTime time)
        {
            if (BackUpDelay == 0)
                return ToFileName(time);

            time = new DateTime(time.Ticks - (time.Ticks % (BackUpDelay * TimeSpan.TicksPerMinute)));

            return $"{time.Year:0000}-{time.Month:00}-{time.Day:00} {time.Hour:00}-{time.Minute:00}";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ob"></param>
        internal void Delete(DBObject ob)
        {
            if (ob.IsDeleted) return;

            Collections[ob.ThisType].Delete(ob);

            //将属性IsDeleted设为true
            ob.OnDeleted();

            //获取DBObject所有属性
            PropertyInfo[] properties = ob.ThisType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

            //循环所有属性
            //Remove Internal Reference
            foreach (PropertyInfo property in properties)
            {
                //该属性是否是Association特性
                Association link = property.GetCustomAttribute<Association>();

                //如果该属性是DBObject对象，设为null
                if (property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    //如果该属性是Association特性且true，将一起删除关联的行
                    if (link != null && link.Aggregate)
                    {
                        DBObject tempOb = (DBObject)property.GetValue(ob);

                        tempOb?.Delete();
                        continue;
                    }

                    property.SetValue(ob, null);
                    continue;
                }

                if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>)) continue;

                //DBBindingList逻辑
                // For Example
                ///////Mapinfo类：///////
                // [Association("Regions",true)]
                // public DBBindingList<MapRegion> Regions { get; set; }
                ///////MapRegion类：/////
                // [Association("Regions")]
                // public MapInfo Map
                //以上2个对象存在DBBindingList关系，所以只要删除Mapinfo中的一行数据，下面MapRegion中的Map字段所关联的所有行将全部删除。

                //取出DBBindingList属性关联的
                IBindingList list = (IBindingList)property.GetValue(ob);

                if (link != null && link.Aggregate)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                        ((DBObject)list[i]).Delete();
                    continue;
                }

                list.Clear();
            }
        }
        /// <summary>
        /// 快速删除
        /// </summary>
        /// <param name="ob"></param>
        internal void FastDelete(DBObject ob)
        {
            if (ob.IsDeleted) return;

            ob.IsTemporary = true;

            ob.OnDeleted();
        }

        public bool IsDisposed { get; private set; }

        public event EventHandler Disposing;

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }

        ~Session()
        {
            Dispose(disposing: false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            this.Disposing?.Invoke(this, EventArgs.Empty);
            Root = null;
            Mode = SessionMode.None;
            BackUp = false;
            BackUpDelay = 0;
            BackupRoot = null;
            SystemHeader = null;
            ClientSystemHeader = null;
            UsersHeader = null;
            Relationships = null;
            foreach (KeyValuePair<Type, ADBCollection> collection in Collections)
            {
                collection.Value.Dispose();
            }
            Collections.Clear();
            Collections = null;
            IsDisposed = true;
            Disposing = null;
        }
    }
}

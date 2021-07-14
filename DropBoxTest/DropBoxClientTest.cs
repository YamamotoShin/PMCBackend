using DropBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DropBoxTest
{
    [TestClass]
    public class DropBoxClientTest
    {
        private DropBoxClient _DropBoxClient;

        [TestInitialize]
        public void Initialize()
        {
            _DropBoxClient = new DropBoxClient(Properties.Settings.Default.AccessToken);
        }

        /// <summary>
        /// ドロップボックスのファイル一覧を取得
        /// </summary>
        /// <param name="path">ドロップボックスのフォルダパス</param>
        private async Task<IEnumerable<string>> GetDropBoxFilePathsAsync(string path = null) 
        {
            var listFolder = await _DropBoxClient.ListFolder(path);
            var dropBoxFilePaths = listFolder
                .entries
                .Where(x => x.tag == "file")
                .Select(x => x.path_lower);

            return dropBoxFilePaths;
        }

        /// <summary>
        /// ローカルのファイルパスを取得
        /// </summary>
        private IEnumerable<string> GetLocalFileName() 
        {
            return Directory
                .GetFiles(nameof(Test01FileUpload), "*")
                .Select(filePath => Path.GetFileName(filePath));
        }

        /// <summary>
        /// ファイルアップロードテスト
        /// </summary>
        [TestMethod]
        public async Task Test01FileUpload()
        {
            var localFilePaths = Directory.GetFiles(nameof(Test01FileUpload), "*");
            if (!localFilePaths.Any()) 
            {
                Assert.IsTrue(false);
            }
            foreach (var localFilePath in localFilePaths)
            {
                var result = await _DropBoxClient.Upload(localFilePath, $"/{Path.GetFileName(localFilePath)}");
                if (!result) 
                {
                    Assert.IsTrue(result);
                }
            }

            Assert.IsTrue(true);
        }

        /// <summary>
        /// リスト取得テスト
        /// </summary>
        [TestMethod]
        public async Task Test02GetList() 
        {
            var dropBoxFilePaths = await GetDropBoxFilePathsAsync();
            var dropBoxFileNames = dropBoxFilePaths.Select(x => Path.GetFileName(x)).ToHashSet();
            if (!dropBoxFileNames.Any())
                Assert.IsTrue(false);

            var localFileNames = GetLocalFileName();
            if (!localFileNames.Any())
                Assert.IsTrue(false);

            Assert.IsTrue(localFileNames.All(localFileName => dropBoxFileNames.Contains(localFileName)));
        }

        /// <summary>
        /// ファイルダウンロードテスト
        /// </summary>
        [TestMethod]
        public void Test02FileDownLoadTestAsync()
        {

            Assert.IsTrue(true);
        }

        /// <summary>
        /// ファイル削除テスト
        /// </summary>
        [TestMethod]
        public async Task Test03FileDeleteTest() 
        {
            var dropBoxFilePaths = await GetDropBoxFilePathsAsync();
            if (!dropBoxFilePaths.Any())
                Assert.IsTrue(false);

            foreach (var dropBoxFilePath in dropBoxFilePaths)
            {
                var result = await _DropBoxClient.DeleteV2(dropBoxFilePath);
                if (!result)
                {
                    Assert.IsTrue(false);
                }
            }

            dropBoxFilePaths = await GetDropBoxFilePathsAsync();
            var dropboxFileNames = dropBoxFilePaths.Select(x => Path.GetFileName(x)).ToHashSet();
            Assert.IsTrue(!dropBoxFilePaths.Any());
        }
    }
}

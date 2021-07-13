using DropBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public async Task Test01FileUpload()
        {
            var localFilePaths = Directory.GetFiles(nameof(Test01FileUpload), "*");
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

        [TestMethod]
        public async Task Test02GetList() 
        {
            var listFolder = await _DropBoxClient.ListFolder();
            var dropBoxFileNames = listFolder
                .entries
                .Where(x => x.tag == "file")
                .Select(x => x.name)
                .ToHashSet();
            if (!dropBoxFileNames.Any())
                Assert.IsTrue(false);

            var localFileNames = Directory.GetFiles(nameof(Test01FileUpload), "*")
                .Select(filePath => Path.GetFileName(filePath));

            if (!localFileNames.Any())
                Assert.IsTrue(false);

            Assert.IsTrue(localFileNames.All(localFileName => dropBoxFileNames.Contains(localFileName)));
        }

        [TestMethod]
        public void Test02FileDownLoadTestAsync()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task Test03FileDeleteTest() 
        {
            var listFolder = await _DropBoxClient.ListFolder();
            if (!listFolder.entries.Any())
                Assert.IsTrue(false);

            var dropBoxFilePaths = listFolder
                .entries
                .Where(x => x.tag == "file")
                .Select(x => x.path_lower);

            foreach (var dropBoxFilePath in dropBoxFilePaths)
            {
                var result = await _DropBoxClient.DeleteV2(dropBoxFilePath);
                if (!result)
                {
                    Assert.IsTrue(false);
                }
            }

            listFolder = await _DropBoxClient.ListFolder();
            Assert.IsTrue(!listFolder.entries.Any(x => x.tag == "file"));
        }
    }
}

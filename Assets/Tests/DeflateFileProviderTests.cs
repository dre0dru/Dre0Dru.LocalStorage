using System;
using System.IO.Compression;
using System.Threading.Tasks;
using LocalStorage.Compression;
using LocalStorage.Providers;
using NUnit.Framework;

namespace LocalStorage.Tests
{
    [TestFixture]
    public class DeflateFileProviderTests
    {
        private static readonly IFileProvider FileProvider =
            new DeflateFileProvider(new FileProvider());

        [SetUp]
        public virtual void SetUp()
        {
            Setup.DeleteFile(Constants.FilePath);
        }

        [TearDown]
        public virtual void TearDown()
        {
            Setup.DeleteFile(Constants.FilePath);
        }

        [Test]
        public void GZipFileProvider_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new DeflateFileProvider(null);
            });
        }

        [TestCaseSource(typeof(Constants), nameof(Constants.FileProviderTestData))]
        public void GZipFileProvider_Write(byte[] data)
        {
            Assert.IsFalse(Setup.FileExists(Constants.FilePath));

            FileProvider.Write(data, Constants.FileName);

            Assert.IsTrue(Setup.FileExists(Constants.FilePath));
            Assert.AreEqual(data, Setup.ReadFromFile(Constants.FilePath).ReadDeflate());
        }

        [TestCaseSource(typeof(Constants), nameof(Constants.FileProviderTestData))]
        public void GZipFileProvider_WriteAsync(byte[] data)
        {
            Assert.IsFalse(Setup.FileExists(Constants.FilePath));

            Task.Run(async () => await FileProvider.WriteAsync(data, Constants.FileName))
                .GetAwaiter().GetResult();

            Assert.IsTrue(Setup.FileExists(Constants.FilePath));
            Assert.AreEqual(data, Setup.ReadFromFile(Constants.FilePath).ReadDeflate());
        }

        [TestCaseSource(typeof(Constants), nameof(Constants.FileProviderTestData))]
        public void GZipFileProvider_Read(byte[] data)
        {
            Setup.WriteToFile(Constants.FilePath, data.WriteDeflate());

            Assert.AreEqual(data, FileProvider.Read(Constants.FileName));
        }

        [TestCaseSource(typeof(Constants), nameof(Constants.FileProviderTestData))]
        public void GZipFileProvider_ReadAsync(byte[] data)
        {
            Setup.WriteToFile(Constants.FilePath, data.WriteDeflate());

            var result = Task.Run(async () => await FileProvider.ReadAsync(Constants.FileName))
                .GetAwaiter().GetResult();

            Assert.AreEqual(data, result);
        }
    }
}
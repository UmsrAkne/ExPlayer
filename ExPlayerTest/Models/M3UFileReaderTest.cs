using ExPlayer.Models;
using NUnit.Framework;

namespace ExPlayerTest.Models
{
    [TestFixture]
    public class M3UFileReaderTest
    {
        [Test]
        public void GetFileInfoWrappersTest()
        {
            string[] paths =
            {
                @"..\musics\a.mp3",
                @"# ..\musics\b.mp3",
                @" # ..\musics\b.mp3",
                @"   #  ..\musics\b.mp3",
                @"..\musics\c.mp3",
                @"innerMusics\d.mp3",
            };

            var basePath = "C:\\temp\\";

            var list = M3UFileReader.GetFileInfoWrappers(paths, basePath);
            Assert.AreEqual(@"C:\musics\a.mp3", list[0].FullName);
            Assert.AreEqual(@"C:\musics\c.mp3", list[1].FullName, "b.mp3 の部分はスキップされているはず");
            Assert.AreEqual(@"C:\temp\innerMusics\d.mp3", list[2].FullName);
        }
    }
}
using System.Collections.Generic;
using System.IO;
using ExPlayer.Models;
using NUnit.Framework;

namespace ExPlayerTest.Models
{
    [TestFixture]
    public class AudioProviderTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void HasNextTest_正常系要素0(bool loop)
        {
            var audioProvider = new AudioProvider() { Loop = loop, };
            Assert.IsFalse(audioProvider.HasNext());
        }

        [Test]
        public void HasNextTest_正常系1()
        {
            var audioProvider = new AudioProvider
            {
                FileInfoWrappers = new List<FileInfoWrapper>()
                {
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("a.mp3"), },
                },
            };

            Assert.IsTrue(audioProvider.HasNext());
        }

        [Test]
        public void HasNextTest_正常系2()
        {
            var audioProvider = new AudioProvider
            {
                FileInfoWrappers = new List<FileInfoWrapper>()
                {
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("a.mp3"), },
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("b.mp3"), },
                },
            };

            Assert.IsTrue(audioProvider.HasNext());
        }

        [Test]
        public void GetNextTest_正常系2_ループなし()
        {
            var audioProvider = new AudioProvider
            {
                FileInfoWrappers = new List<FileInfoWrapper>()
                {
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("a.mp3"), },
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("b.mp3"), },
                },
            };

            Assert.AreEqual("a.mp3",audioProvider.GetNext().Name);
            Assert.AreEqual("b.mp3",audioProvider.GetNext().Name);
            Assert.Null(audioProvider.GetNext());
        }

        [Test]
        public void GetNextTest_正常系2_ループ()
        {
            var audioProvider = new AudioProvider
            {
                Loop = true,
                FileInfoWrappers = new List<FileInfoWrapper>()
                {
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("a.mp3"), },
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("b.mp3"), },
                },
            };

            Assert.AreEqual("a.mp3",audioProvider.GetNext().Name);
            Assert.AreEqual("b.mp3",audioProvider.GetNext().Name);

            Assert.AreEqual("a.mp3",audioProvider.GetNext().Name);
            Assert.AreEqual("b.mp3",audioProvider.GetNext().Name);

            Assert.AreEqual("a.mp3",audioProvider.GetNext().Name);
        }

        [Test]
        public void GetNextTest_要素4_ループ()
        {
            var audioProvider = new AudioProvider
            {
                Loop = true,
                Index = 0,
                FileInfoWrappers = new List<FileInfoWrapper>()
                {
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("a.mp3"), },
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("b.mp3"), },
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("c.mp3"), },
                    new FileInfoWrapper() { FileSystemInfo = new FileInfo("d.mp3"), },
                },
            };

            Assert.AreEqual("a.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual("b.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual("c.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual("d.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual("a.mp3", audioProvider.GetNext().Name);
        }

        [Test]
        public void Ignoreを設定した場合のテスト()
        {
            var wrappers = new List<FileInfoWrapper>()
            {
                new FileInfoWrapper() { FileSystemInfo = new FileInfo("a.mp3"), },
                new FileInfoWrapper() { FileSystemInfo = new FileInfo("b.mp3"), },
                new FileInfoWrapper() { FileSystemInfo = new FileInfo("c.mp3"), },
                new FileInfoWrapper() { FileSystemInfo = new FileInfo("d.mp3"), },
            };

            var audioProvider = new AudioProvider
            {
                Loop = true,
                Index = 0,
                FileInfoWrappers = wrappers,
            };

            Assert.AreEqual("a.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual("b.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual(1, audioProvider.Index);
            wrappers[0].Ignore = true;

            Assert.AreEqual("c.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual("d.mp3", audioProvider.GetNext().Name);
            Assert.AreEqual("b.mp3", audioProvider.GetNext().Name, "a.mp3 はスルーされる");
            wrappers[1].Ignore = true; // 最後に取得したオブジェクトを無視してみる

            Assert.AreEqual("c.mp3", audioProvider.GetNext().Name);
            wrappers[3].Ignore = true; // 次に取得するはずだったオブジェクトを無視

            Assert.AreEqual("c.mp3", audioProvider.GetNext().Name, "c.mp3 以外は無視されているため、連続で c.mp3 が返却される");
            Assert.AreEqual("c.mp3", audioProvider.GetNext().Name, "c.mp3 以外は無視されているため、連続で c.mp3 が返却される");
            wrappers[2].Ignore = true;

            Assert.IsNull(audioProvider.GetNext(), "全てのファイルを無視したので、nullが返ってくる");
            Assert.IsNull(audioProvider.GetNext(), "全てのファイルを無視したので、nullが返ってくる");
            Assert.IsNull(audioProvider.GetNext(), "全てのファイルを無視したので、nullが返ってくる");
        }

        [Test]
        public void 全てIgnoreに設定した場合のテスト()
        {
            var wrappers = new List<FileInfoWrapper>()
            {
                new FileInfoWrapper() { FileSystemInfo = new FileInfo("a.mp3"), Ignore = true, },
                new FileInfoWrapper() { FileSystemInfo = new FileInfo("b.mp3"), Ignore = true,  },
                new FileInfoWrapper() { FileSystemInfo = new FileInfo("c.mp3"), Ignore = true,  },
            };

            var audioProvider = new AudioProvider
            {
                Loop = true,
                Index = 0,
                FileInfoWrappers = wrappers,
            };

            Assert.IsNull(audioProvider.GetNext());
            Assert.IsNull(audioProvider.GetNext());
            Assert.IsNull(audioProvider.GetNext());
            Assert.IsNull(audioProvider.GetNext());

            wrappers[0].Ignore = false;
            Assert.AreEqual("a.mp3", audioProvider.GetNext().Name, "Ignore を false にした状態なら値が返ってくるか確認する");
        }
    }
}
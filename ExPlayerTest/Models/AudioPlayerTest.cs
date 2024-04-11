using System.Collections.Generic;
using System.IO;
using ExPlayer.Models;
using NUnit.Framework;

namespace ExPlayerTest.Models
{
    [TestFixture]
    public class AudioPlayerTest
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
    }
}
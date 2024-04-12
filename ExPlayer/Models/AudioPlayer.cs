using System;
using System.IO;
using NAudio.Utils;
using NAudio.Vorbis;
using NAudio.Wave;

namespace ExPlayer.Models
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AudioPlayer : IDisposable
    {
        private readonly WaveOutEvent waveOut;
        private IWaveProvider reader;
        private bool waveOutEventIsEnabled;
        private string currentFileExtension = string.Empty;
        private FileInfoWrapper lastPlayFile;

        public AudioPlayer()
        {
            waveOut = new WaveOutEvent();
            waveOut.PlaybackStopped += (_, _) =>
            {
                if (waveOut.PlaybackState == PlaybackState.Stopped)
                {
                    CurrentFile = null;
                    PlayCompleted?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public event EventHandler PlayCompleted;

        public long Position => waveOutEventIsEnabled ? (long)waveOut.GetPositionTimeSpan().TotalSeconds : 0;

        public long Length { get; private set; }

        public FileInfoWrapper CurrentFile { get; private set; }

        public void Play(FileInfoWrapper audioFile)
        {
            if (audioFile.FileSystemInfo.Extension == ".mp3")
            {
                var m = new Mp3FileReader(audioFile.FullName);
                Length = (long)m.TotalTime.TotalSeconds;
                reader = m;
            }
            else if (audioFile.FileSystemInfo.Extension == ".ogg")
            {
                var v = new VorbisWaveReader(audioFile.FullName);
                Length = (long)v.TotalTime.TotalSeconds;
                reader = v;
            }
            else if (audioFile.FileSystemInfo.Extension == ".wav")
            {
                var w = new WaveFileReader(audioFile.FullName);
                Length = (long)w.TotalTime.TotalSeconds;
                reader = w;
            }
            else
            {
                reader = null;
            }

            if (reader == null)
            {
                // 入力されたファイルが非対応の拡張子だった場合
                return;
            }

            currentFileExtension = audioFile.FileSystemInfo.Extension;
            audioFile.Playing = true;
            if (lastPlayFile != null)
            {
                lastPlayFile.Playing = false;
            }

            lastPlayFile = audioFile;
            CurrentFile = audioFile;
            waveOutEventIsEnabled = true;
            waveOut.Init(reader);
            waveOut.Play();
        }

        public void Stop()
        {
            CurrentFile = null;
            waveOut.Stop();
            waveOutEventIsEnabled = false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Seek(long positionSeconds)
        {
            switch (currentFileExtension)
            {
                case ".mp3":
                    ((Mp3FileReader)reader).CurrentTime = TimeSpan.FromSeconds(positionSeconds);
                    break;
                case ".ogg":
                    ((VorbisWaveReader)reader).CurrentTime = TimeSpan.FromSeconds(positionSeconds);
                    break;
                case ".wav":
                    ((WaveFileReader)reader).CurrentTime = TimeSpan.FromSeconds(positionSeconds);
                    break;
            }
        }

        public int GetCurrentTime()
        {
            return currentFileExtension switch
            {
                ".mp3" => (int)((Mp3FileReader)reader).CurrentTime.TotalSeconds,
                ".ogg" => (int)((VorbisWaveReader)reader).CurrentTime.TotalSeconds,
                ".wav" => (int)((WaveFileReader)reader).CurrentTime.TotalSeconds,
                _ => 0,
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            waveOut.Dispose();
        }
    }
}
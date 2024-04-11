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

        public AudioPlayer()
        {
            waveOut = new WaveOutEvent();
            waveOut.PlaybackStopped += (_, _) =>
            {
                if (waveOut.PlaybackState == PlaybackState.Stopped)
                {
                    PlayCompleted?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public event EventHandler PlayCompleted;

        public long Position => waveOutEventIsEnabled ? (long)waveOut.GetPositionTimeSpan().TotalSeconds : 0;

        public long Length { get; private set; }

        public void Play(string audioFilePath)
        {
            if (new FileInfo(audioFilePath).Extension == ".mp3")
            {
                var m = new Mp3FileReader(audioFilePath);
                Length = (long)m.TotalTime.TotalSeconds;
                reader = m;
            }
            else if (new FileInfo(audioFilePath).Extension == ".ogg")
            {
                var v = new VorbisWaveReader(audioFilePath);
                Length = (long)v.TotalTime.TotalSeconds;
                reader = v;
            }
            else if (new FileInfo(audioFilePath).Extension == ".wav")
            {
                var w = new WaveFileReader(audioFilePath);
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

            currentFileExtension = new FileInfo(audioFilePath).Extension;
            waveOutEventIsEnabled = true;
            waveOut.Init(reader);
            waveOut.Play();
        }

        public void Stop()
        {
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
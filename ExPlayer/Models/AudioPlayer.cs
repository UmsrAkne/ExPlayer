using System;
using System.IO;
using NAudio.Utils;
using NAudio.Vorbis;
using NAudio.Wave;

namespace ExPlayer.Models
{
    public class AudioPlayer : IDisposable
    {
        private readonly WaveOutEvent waveOut = new ();
        private IWaveProvider reader;
        private bool waveOutEventIsEnabled;

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

        protected virtual void Dispose(bool disposing)
        {
            waveOut.Dispose();
        }
    }
}
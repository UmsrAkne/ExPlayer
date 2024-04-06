using System;
using System.IO;
using NAudio.Vorbis;
using NAudio.Wave;

namespace ExPlayer.Models
{
    public class AudioPlayer : IDisposable
    {
        private readonly WaveOutEvent waveOut = new ();
        private IWaveProvider reader;
        private bool waveOutEventIsEnabled;

        public long Position => waveOutEventIsEnabled ? waveOut.GetPosition() : 0;

        public void Play(string audioFilePath)
        {
            reader = new FileInfo(audioFilePath).Extension switch
            {
                ".mp3" => new Mp3FileReader(audioFilePath),
                ".ogg" => new VorbisWaveReader(audioFilePath),
                ".wav" => new WaveFileReader(audioFilePath),
                _ => null,
            };

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
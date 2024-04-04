using System;
using NAudio.Wave;

namespace ExPlayer.Models
{
    public class AudioPlayer : IDisposable
    {
        private readonly WaveOutEvent waveOut = new ();
        private IWaveProvider reader;

        public void Play(string audioFilePath)
        {
            reader = new Mp3FileReader(audioFilePath);
            waveOut.Init(reader);
            waveOut.Play();
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
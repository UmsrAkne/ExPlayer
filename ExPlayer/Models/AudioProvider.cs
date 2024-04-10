using System.Collections.Generic;

namespace ExPlayer.Models
{
    public class AudioProvider
    {
        public bool Loop { get; set; }

        public bool FirstCall { get; set; }

        public int Index { get; private set; }

        public List<FileInfoWrapper> FileInfoWrappers { private get; set; } = new ();

        public bool HasNext()
        {
            return true;
        }

        public FileInfoWrapper GetNext()
        {
            return null;
        }
    }
}
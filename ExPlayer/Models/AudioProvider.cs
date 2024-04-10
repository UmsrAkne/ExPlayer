using System.Collections.Generic;

namespace ExPlayer.Models
{
    public class AudioProvider
    {
        public bool Loop { get; set; }

        public bool FirstCall { get; set; } = true;

        public int Index { get; private set; }

        public List<FileInfoWrapper> FileInfoWrappers { private get; set; } = new ();

        public bool HasNext()
        {
            if (FileInfoWrappers.Count == 0)
            {
                return false;
            }

            if (FirstCall)
            {
                return true;
            }

            if (Index >= FileInfoWrappers.Count - 1)
            {
                return Loop;
            }

            return true;
        }

        public FileInfoWrapper GetNext()
        {
            return null;
        }
    }
}
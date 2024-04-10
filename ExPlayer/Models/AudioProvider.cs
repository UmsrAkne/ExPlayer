using System.Collections.Generic;
using System.Linq;

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
            var fws = FileInfoWrappers.Where(f => f.IsSoundFile()).ToList();

            if (fws.Count == 0)
            {
                return false;
            }

            if (FirstCall)
            {
                return true;
            }

            if (Index >= fws.Count - 1)
            {
                return Loop;
            }

            return true;
        }

        public FileInfoWrapper GetNext()
        {
            var fws = FileInfoWrappers.Where(f => f.IsSoundFile()).ToList();

            if (!HasNext())
            {
                return null;
            }

            if (Index >= fws.Count - 1)
            {
                Index = -1;
            }

            if (FirstCall)
            {
                FirstCall = false;
                return fws[Index];
            }

            Index++;
            return fws[Index];
        }
    }
}
using Framework.Attributes;
using System.Collections.Generic;
using Touch.Core.Comun;

namespace Touch.Core.Totems
{
    [TableName("sector")]
    public class SectorDelTotem: Sector
    {
        public List<Playlist> Playlists { get; set; }
    }
}
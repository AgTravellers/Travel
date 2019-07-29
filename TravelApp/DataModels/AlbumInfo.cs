using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApp.DataModels
{
    public class AlbumInfo
    {
        public string AlbumName { get; set; }

        public string ContainerName { get; set; }

		public int LastImageIndex { get; set; }

        public AlbumInfo(string albumname, string containername, string lastImageIndex)
        {
            this.AlbumName = albumname;
            this.ContainerName = containername;
			this.LastImageIndex = int.Parse(lastImageIndex);
        }
    }
}
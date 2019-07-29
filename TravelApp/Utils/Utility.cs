using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TravelApp.Utils
{
	public static class Utility
	{
		public static double FileSizeInMegabytes(HttpPostedFileBase file)
		{
			if (file != null)
			{
				return file.ContentLength / (1024 * 1024);
			}

			return 0;
		}
	}
}

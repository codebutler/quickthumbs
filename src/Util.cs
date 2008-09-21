//
// Util.cs: 
// 
// Authors:
//    Eric Butler <eric@extremeboredom.net>
//
// Copyright (C) 2006, Eric Butler

namespace QuickThumbs
{
	public static class Util
	{
		public static string CombinePath (string path1, string path2)
		{
			if (path2.StartsWith ("/") == true)
				path2 = path2.Substring (1);
			
			if (path1.EndsWith ("/") == false)
				return path1 + "/" + path2;
			else
				return path1 + path2;
		}
	}
}

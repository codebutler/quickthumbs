//
// Image.cs: 
// 
// Authors:
//    Eric Butler <eric@extremeboredom.net>
//
// Copyright (C) 2006, Eric Butler

using System;
using System.IO;

namespace QuickThumbs
{
	public class Image
	{
		int width, height;
		string filename;

		public Image (string filename)
		{
			this.filename = filename;

			using (MagickWand wand = new MagickWand()) {
				wand.ReadImage(filename);
				this.width = (int)wand.ImageWidth;
				this.height = (int)wand.ImageHeight;
			}
		}
		
		public int Width {
			get {
				return width;
			}
		}

		public int Height {
			get {
				return height;
			}
		}

		 public void SaveResizedImage (int newWidth, string outPath)
		 {
			 CreateDirectoryForFile(outPath);
			 using (MagickWand wand = new MagickWand()) {
				wand.ReadImage(filename);
				wand.TransformImage("", newWidth.ToString());
				wand.WriteImage (outPath);
			}
		 }

		 public void SaveResizedImage (int newWidth, int newHeight, string outPath)
		 {
			CreateDirectoryForFile(outPath);
			using (MagickWand wand = new MagickWand()) {
				wand.ReadImage(filename);

				if (width > newWidth && height > newHeight) {
					wand.ResizeImage((uint)newWidth, (uint)newHeight);
				} else {
					// XXX: Make a new image that's newwidth by newheight
					// and center old image in it
				}

				wand.WriteImage (outPath);
			}
		}

		 private static void CreateDirectoryForFile(string filename)
		 {
			string directory = filename.Substring(0, filename.LastIndexOf("/"));
			if (Directory.Exists(directory) == false) {
				Directory.CreateDirectory(directory);
			}
		 }
	}
}

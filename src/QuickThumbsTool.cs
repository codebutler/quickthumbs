//
// QuickThumbsTool.cs: 
// 
// Authors:
//    Eric Butler <eric@extremeboredom.net>
//
// Copyright (C) 2006, Eric Butler

using System;
using System.Xml;
using System.IO;

public class QuickThumbsTool
{
	QuickThumbs quickThumbs;
	
	string[] validExtensions = new string[] { ".png", ".jpeg", ".jpg", ".gif" };
	string[] sizes;

	string rootDirectory;
	
	public static void Main()
	{
		new QuickThumbsTool();
	}

	public QuickThumbsTool() 
	{
		if (File.Exists("bin/QuickThumbs.dll") == false)
			throw new Exception("You must run this tool from your QuickThumbs application root.");


		if (File.Exists("web.config") == false)
			throw new Exception("web.config was not found in the current directory.");

		quickThumbs = new QuickThumbs(Convert.ToInt32(GetSetting("ThumbnailWidth")), Convert.ToInt32(GetSetting("ThumbnailHeight")));
		
		sizes = GetSetting("Sizes").Split(' ');	
		
		rootDirectory = Environment.CurrentDirectory;
		DoDirectory (rootDirectory);

	}
	
	private void DoDirectory (string currentDirectory)
	{
		Console.WriteLine("\n\tLooking in directory " + currentDirectory  + "...\n");
		DirectoryInfo directory = new DirectoryInfo(currentDirectory);

		foreach (DirectoryInfo subDirectory in directory.GetDirectories()) {
			if (subDirectory.FullName != Path.Combine(rootDirectory, "bin")) {
				if (File.Exists(Path.Combine(subDirectory.FullName, ".nolisting")) == false) {
					if (subDirectory.Name.StartsWith(".") == false) {
						if ((subDirectory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden ) {
							DoDirectory(subDirectory.FullName);
						}
					}
				}
			}
		}
		
		foreach (FileInfo file in directory.GetFiles()) {
			
			try {
				if (Array.IndexOf(validExtensions,file.Extension.ToLower()) > -1) {
					string thumbFile = "bin/data/thumbs" + file.FullName.Substring(rootDirectory.Length) + "-thumb.jpg";
					
					ImageInfo info;
					
					if (File.Exists(thumbFile) == false || file.LastWriteTime > new FileInfo(thumbFile).LastWriteTime) {
						Console.WriteLine("\t\tGenerating thumbnail for " + file.Name + "...");
						quickThumbs.GenerateThumb(file.FullName, thumbFile);
					}

					string infoFile = "bin/data/info" + file.FullName.Substring(rootDirectory.Length) + ".txt";
					if (File.Exists(infoFile) == false || file.LastWriteTime > new FileInfo(infoFile).LastWriteTime) {
						Console.WriteLine("\t\tGenerating exif cache for " + file.Name + "...");
					}
					info = new ImageInfo (file.FullName, infoFile);

					foreach (string size in sizes) {
						if (Convert.ToInt32(size) < info.Width) {
							string smallerFile = "bin/data/small_images" + file.FullName.Substring(rootDirectory.Length) + "-" + size + ".jpg";
							if (File.Exists(smallerFile) == false || file.LastWriteTime > new FileInfo(smallerFile).LastWriteTime) {
								Console.WriteLine("\t\tCreating " + size + " copy of " + file.Name + "...");
								quickThumbs.ResizePhoto(file.FullName, smallerFile, Convert.ToInt32(size));
							}
						}
					}
				}
			} catch (Exception ex) {
				Console.Error.WriteLine ("Error proccessing " + file.FullName + ": " + ex.ToString ());
			}
		}
	}

	private string GetSetting(string key)
	{
		XmlDocument document = new XmlDocument();
		document.Load("web.config");
		return document.SelectSingleNode("/configuration/appSettings/add[@key=\"" + key + "\"]/@value").Value;
	}
}

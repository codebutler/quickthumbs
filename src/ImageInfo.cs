//
// ImageInfo.cs: 
// 
// Authors:
//    Eric Butler <eric@extremeboredom.net>
//
// Copyright (C) 2006, Eric Butler

using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Drawing;
using Exif;

public class ImageInfo {

	private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	
	public int Width = 0;
	public int Height = 0;

	public Hashtable exifTags = new Hashtable();
	
	public ImageInfo(string imagePath, string infoFile) {

		log.Info("ImageInfo(" + imagePath + ", " + infoFile + ")");

		try {

			if (Directory.Exists(infoFile.Substring(0,infoFile.LastIndexOf("/"))) == false)
				Directory.CreateDirectory(infoFile.Substring(0,infoFile.LastIndexOf("/")));

			
			if (System.IO.File.Exists(infoFile) == false || new FileInfo(imagePath).LastWriteTime > new FileInfo(infoFile).LastWriteTime) {
			
				log.Debug("...Creating new info file");

				using (StreamWriter sw = new StreamWriter(infoFile)) {


					try {
						log.Debug("looking up exif data for: " + imagePath);
						ExifData exif_info = new ExifData(imagePath);
						//ExifContent content = exif_info.GetContents(Exif.Ifd.Zero);
						ExifContent[] contents = exif_info.GetContents();
						
						foreach (ExifContent content in contents) {
							log.Debug("found " +  content.GetEntries().Length + " exif tags in this ExifContent!");
							foreach (ExifEntry entry in content.GetEntries()) {
								string tagName = entry.Name;
								string tagTitle = entry.Title;
								string tagValue = entry.Value;
								
								//int termix = tagValue.IndexOf('\0');
								//tagValue = tagValue.Substring(0, termix < 0 ? tagValue.Length : termix);

								log.Debug("EXIF: " + tagTitle + " (" + tagName + ") = " + tagValue);
								sw.WriteLine( tagName + ": " + tagValue);

								// TODO: Change this to tagName (Update web.config)
								if (exifTags[tagTitle] == null)
									exifTags.Add(tagTitle, tagValue);
							}
						}

						if (exifTags["width"] == null || exifTags["height"] == null) {
							Bitmap bmp = new Bitmap(imagePath);
							if (exifTags["width"] == null) {
								log.Debug("No width found in exif info ... pulling from image.");
								//exifTags.Add("width", bmp.Width);
								this.Width = bmp.Width;
								sw.WriteLine( "width: " + bmp.Width);
							}
							if (exifTags["height"] == null) {
								log.Debug("No height found in exif info ... pulling from image.");
								//exifTags.Add("height", bmp.Height);
								this.Height = bmp.Height;
								sw.WriteLine( "height: " + bmp.Height);
							}
						}
						

					} catch (Exception ex) {		
						if (ex is DllNotFoundException)
							log.Debug("EXIF not avaliable (libexif not installed)!", ex);
						else 
							log.Debug("Error while reading exif data:\n " + ex.ToString());
						Bitmap bmp = new Bitmap(imagePath);
						if (exifTags["width"] == null) {
							log.Debug("exif not avaliable.. saving from image.");
							this.Width = bmp.Width;
							sw.WriteLine( "width: " + bmp.Height);
							
						}
						if (exifTags["height"] == null) {
							log.Debug("exif not avaliable.. saving from image.");
							this.Height = bmp.Height;
							sw.WriteLine( "height: " + bmp.Height);
						}

					}
					sw.Close();
					exifTags.Clear();
				} 
			} else {
				log.Debug("not creating new info file");
			}

			using (StreamReader sr = new StreamReader(infoFile)) {

				string[] lines = sr.ReadToEnd().Split(Environment.NewLine.ToCharArray());
		
				for (int x=0; x < lines.Length; x++) {

					string currentLine = lines[x];
		
					if (currentLine.Length > 4 && currentLine.IndexOf(": ") > -1) {
						string propertyName = currentLine.Substring(0, currentLine.IndexOf(":")).ToLower();
						string propertyValue = currentLine.Substring(propertyName.Length + 2);

						 switch (propertyName.ToLower()) {
							case "width":
								this.Width = Convert.ToInt32(propertyValue);
							break;
							case "height":
								this.Height = Convert.ToInt32(propertyValue);
							break;
							default:
								if (exifTags[propertyName] == null)
									exifTags.Add(propertyName, propertyValue);
							break;
						 }
					}
				}
				if (Width == 0) {
					log.Debug("Width not found in exif data!");
				}
				if (Height == 0) {
					log.Debug("Height not found in exif data!");
				}
				sr.Close();
			}
		} catch (Exception ex) {

			if (File.Exists (infoFile)) 
				File.Delete (infoFile);
			
			log.Error("Exception in ImageInfo constructor!", ex);
			throw ex;
		}
	}
}

//
// QuickThumbsHandler.cs: 
// 
// Authors:
//    Eric Butler <eric@extremeboredom.net>
//
// Copyright (C) 2006, Eric Butler

using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using Exif;

//[assembly: log4net.Config.XmlConfigurator(Watch=true)]

namespace QuickThumbs
{
	public class QuickThumbsHandler : IHttpHandler
	{

		public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private log4net.Appender.MemoryAppender appender;

		public QuickThumbsHandler () {
			appender = new log4net.Appender.MemoryAppender();
			
			log4net.Config.BasicConfigurator.Configure (appender);
		}

		public static int ThumbWidth {
			get {
				return Convert.ToInt32(GetSetting("ThumbnailWidth"));
			}
		}
		
		public static int ThumbHeight {
			get {
				return Convert.ToInt32(GetSetting("ThumbnailHeight"));
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			try {

				appender.Clear();
				log.Info("ProcessRequest started");

				string[] validExtensions = new string[] { ".png", ".jpeg", ".jpg", ".gif" };
				
				Uri url = context.Request.Url;
				string urlOnlyString = url.GetLeftPart(UriPartial.Path);
					
				string filePath = context.Server.MapPath(context.Server.UrlDecode(context.Request.Url.LocalPath));

				string fileExtension = context.Request.Url.ToString(); fileExtension = fileExtension.Substring(fileExtension.LastIndexOf(".") +1).ToLower();
				if (fileExtension.IndexOf("?") > -1) fileExtension = fileExtension.Substring(0,fileExtension.IndexOf("?"));
				string directory = context.Server.MapPath(context.Server.UrlDecode(context.Request.Url.LocalPath));

				string relativePath = HttpUtility.UrlDecode (context.Request.Path.Substring (context.Request.ApplicationPath.Length));

				if (relativePath.StartsWith (Path.DirectorySeparatorChar.ToString ()) == false)
					relativePath = Path.DirectorySeparatorChar.ToString () + relativePath;
						
				string smallDir = "/bin/data/small_images";

		//		string web_themeDir = Util.CombinePath (context.Request.ApplicationPath, "bin/templates/") + GetSetting("template");

				string web_cssFile = Util.CombinePath (context.Request.ApplicationPath,  "bin/templates/") + GetSetting("template") + "/" + GetSetting("theme") + ".css";
				
				string local_templateFile = Path.Combine (Path.Combine (context.Server.MapPath("/bin/templates/"), GetSetting("template")),  "listing.tpl");
				
				string local_binDir = Path.Combine(context.Request.PhysicalApplicationPath,"bin");


				Template tpl = null;

				if (context.Request.QueryString["GetSpecialImage"] != null) {

					string special = context.Request.QueryString["GetSpecialImage"];
					
					// TODO: I don't think this is very efficient:
					//
					log.Debug(System.Reflection.Assembly.GetExecutingAssembly ().FullName);

					System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream (special);

					if (s != null) {
		
						byte[] image = new byte[s.Length];
						s.Read(image, 0, Convert.ToInt32(s.Length));

						context.Response.ContentType = "image/" + special.Substring(special.IndexOf(".") +1);
						context.Response.Cache.SetCacheability(HttpCacheability.Public);
						context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
						context.Response.BinaryWrite (image);
						context.Response.End();
						s.Close();
						image = null;
					} else {
						throw new Exception("Invalid special image: " + special);
					}
					
					return;

				} else if (Directory.Exists(filePath)) {

					
					// This is a directory
					DirectoryInfo d = new DirectoryInfo(directory);
					
					if (File.Exists(Path.Combine(d.FullName, ".nolisting")))
						throw new HttpException(403, "You are not allowed to browse the requested directory. Please specify a filename instead.");
					
					if (directory.StartsWith(local_binDir) == true | d.Name.StartsWith(".") == true & (d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
						throw new HttpException(403, "You are not allowed to access the requested directory.");


					tpl = new Template(local_templateFile);

					tpl.setField ("PAGE_TITLE", context.Request.Url.ToString());
					tpl.setField ("PAGE_CSS", web_cssFile);
					
					int dirCount = 0;
					int fileCount = 0;

					// Show Folders
					tpl.selectSection("DIR_ITEM");
					foreach (DirectoryInfo subDir in d.GetDirectories()) {
						if (subDir.FullName.StartsWith(local_binDir) == false & subDir.Name.StartsWith(".") == false & (subDir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden) {
							dirCount += 1;
							tpl.setField("DIR_URL",  context.Request.Url.ToString() + subDir.Name);
							tpl.setField("DIR_NAME", subDir.Name);
							tpl.appendSection();
						}
					}
					tpl.deselectSection();

					tpl.setField("PAGE_DIRCOUNT", dirCount);

					if (d.GetDirectories().Length > 0 & d.GetFiles().Length > 0) {
						tpl.selectSection("LISTS_SEPORATOR");
						tpl.appendSection();
						tpl.deselectSection();
					} 

					// Show Images
					tpl.selectSection("PIC_ITEM");
					foreach (FileInfo file in d.GetFiles()) {
						try {
							if (Array.IndexOf(validExtensions,file.Extension.ToLower()) > -1) {

								fileCount += 1;

								string fileName = file.Name;
								if (Convert.ToBoolean(GetSetting("ShowExtension")) == false)
									fileName = fileName.Substring(0, file.Name.Length - file.Extension.Length);

								if (Convert.ToBoolean(GetSetting("ConvertUnderscores")) == true)
									fileName = fileName.Replace("_", " ");


								//string thumbFile = Util.CombinePath ("/bin/data/thumbs", HttpUtility.UrlDecode (context.Request.Path.Substring(context.Request.ApplicationPath.Length)) + file.Name + "-thumb.jpg");
								string thumbFile = Util.CombinePath ("/bin/data/thumbs", relativePath + file.Name + "-thumb.jpg");

								string local_thumbFile = context.Server.MapPath (thumbFile);

								if (File.Exists(local_thumbFile) == false || file.LastWriteTime > new FileInfo(local_thumbFile).LastWriteTime) {
									// string localFile = context.Server.MapPath(context.Request.Path.Substring(context.Request.ApplicationPath.Length) + file.Name);
									string localFile = context.Server.MapPath (Util.CombinePath (HttpUtility.UrlDecode (context.Request.Path), file.Name));
									//quickThumbs.GenerateThumb (localFile, local_thumbFile);
									Image image = new Image(localFile);
									image.SaveResizedImage(ThumbWidth, ThumbHeight, local_thumbFile);
								}
								
								tpl.setField ("PIC_URL", context.Request.Url.GetLeftPart (UriPartial.Path) + file.Name + "?ViewImage");
								tpl.setField ("PIC_NAME", fileName);
				//				tpl.setField ("PIC_THUMB_URL", context.Request.Url.ToString() + file.Name + "?Size=thumb"); //context.Request.ApplicationPath + thumbFile);
								tpl.setField ("PIC_THUMB_URL", context.Request.Url.GetLeftPart(UriPartial.Path) + file.Name + "?Size=thumb"); //context.Request.ApplicationPath + thumbFile);
								tpl.setField ("PIC_SIZE", FormatBytes(file.Length));

								tpl.setField ("PIC_THUMB_WIDTH", ThumbWidth);
								tpl.setField ("PIC_THUMB_HEIGHT", ThumbHeight);

								
								tpl.appendSection();
							}
						} catch (Exception ex) {
							context.Response.Write("<pre>" + ex + "</pre>");
							log.Error("Failed to process file \"" + file.FullName + "\"!", ex);
						}
					}
					tpl.deselectSection();
					
					tpl.setField("PAGE_IMAGECOUNT", fileCount);

					
				} else {
					if (File.Exists(filePath)) {
						if (context.Request.Url.Query.IndexOf("ViewImage") > -1) {
							
							FileInfo f = new FileInfo(filePath);

							string fileName = f.Name;
							if (Convert.ToBoolean(GetSetting("ShowExtension")) == false)
								fileName = fileName.Substring(0, f.Name.Length - f.Extension.Length);

							if (Convert.ToBoolean(GetSetting("ConvertUnderscores")) == true)
								fileName = fileName.Replace("_", " ");

							
							string infoFile = context.Server.MapPath(Util.CombinePath ("/bin/data/info", relativePath + ".txt"));

							ImageInfo info = new ImageInfo (filePath, infoFile);

							tpl = new Template (context.Server.MapPath( Util.CombinePath ("/bin/templates",  GetSetting("template") + "/image.tpl")));
							
							
							tpl.setField ("PAGE_TITLE", fileName);
							tpl.setField ("PAGE_CSS", Util.CombinePath (context.Request.ApplicationPath,  "bin/templates/") + GetSetting("template") + "/" + GetSetting("theme") + ".css");

							tpl.setField ("PAGE_URL", url.ToString().Substring(0, url.ToString().Length - url.Query.Length));
							tpl.setField ("PARENT_URL", urlOnlyString.Substring(0, urlOnlyString.LastIndexOf("/")));


							string[] sizes = GetSetting("Sizes").Split(' ');
							
							int imageWidth = -1; //Convert.ToInt32(sizes[0]);

							

							foreach (string size in sizes) {
								//int width = Convert.ToInt32(size);
								//int height = width * (int)((int)info.Height / (int)info.Width);

								int width = Convert.ToInt32 (size);
								int height = Convert.ToInt32 ( (double)width * ((double)info.Height / (double)info.Width) );
								if (width > 0) {
			
									if (width < info.Width) {
			
										tpl.selectSection("SIZE_ITEM");
										tpl.setField("SIZE_URL", "?ViewImage&amp;Size=" + width);

										//if (context.Request.QueryString["Size"] != null && width.ToString() == context.Request.QueryString["Size"])
										//	tpl.setField("SIZE_TITLE", "<b>" + width + "x" + height + "</b>");
										//else
											tpl.setField("SIZE_TITLE", width + "x" + height);

										tpl.appendSection();
										tpl.deselectSection();
			
										if (imageWidth == -1)
											imageWidth = width;
									}
								}
							}

							if (imageWidth == -1)
								imageWidth = info.Width;

							tpl.selectSection("SIZE_ITEM");
							//tpl.setField("SIZE_URL", url.ToString().Substring(0, url.ToString().Length - url.Query.Length));
							tpl.setField("SIZE_URL", "?ViewImage&amp;Size=" + info.Width);
							tpl.setField("SIZE_TITLE", "Original Size");
							tpl.appendSection();
							tpl.deselectSection();

						
							context.Response.ContentType = "text/html";


							if (context.Request.QueryString["Size"] != null)
								imageWidth = Convert.ToInt32(context.Request.QueryString["Size"]);
							
							if (Array.IndexOf(sizes, imageWidth.ToString()) == -1 && imageWidth != info.Width)
								throw new Exception("The specified size (" + imageWidth.ToString() + ") is not allowed.");
							

							/* Back/Forward Buttons */

							// DirectoryInfo parentDirectory = new DirectoryInfo(parentDirectoryPath);
							// parentDirectory.GetFiles()
							// Array.IndexOf(parentDirectories.GetFiles(), currentDirectory
							
							// string smallFile = Util.CombinePath (smallDir, relativePath +  "-" + imageWidth + ".jpg");

							string localSmallFile = context.Server.MapPath (Util.CombinePath (smallDir, relativePath + "-" + imageWidth + ".jpg"));

							if (imageWidth != info.Width) {
								if (File.Exists(localSmallFile) == false || f.LastWriteTime > new FileInfo(localSmallFile).LastWriteTime) {
									//string localFile = context.Server.MapPath (HttpUtility.UrlDecode (context.Request.Path.Substring(context.Request.ApplicationPath.Length)));
									string localFile = context.Server.MapPath (relativePath);
			
									try {
										//quickThumbs.ResizePhoto (localFile, localSmallFile, imageWidth);
										Image image = new Image(localFile);
										image.SaveResizedImage(imageWidth, localSmallFile);
									} catch (Exception ex) { 
										tpl.setField("PIC_SRC", "?GetSpecialImage=error.png");
										tpl.setField("PIC_DESCRIPTION", "<b>Error:</b> 	" + ex.Message);
										localSmallFile = null;
									}
								}
			
								if (localSmallFile != null) {
									tpl.setField ("PIC_URL", f.Name + "?Size=" + imageWidth);
									tpl.setField ("PIC_SRC", f.Name + "?Size=" + imageWidth);
								}
							} else {
								tpl.setField ("PIC_URL", f.Name);
								tpl.setField ("PIC_SRC", f.Name);
							}

							tpl.selectSection("EXIF_ITEM");
							tpl.setField("EXIF_NAME", "Original Width");
							tpl.setField("EXIF_VALUE", info.Width);
							tpl.appendSection();
							tpl.deselectSection();

							tpl.selectSection("EXIF_ITEM");
							tpl.setField("EXIF_NAME", "Original Height");
							tpl.setField("EXIF_VALUE", info.Height);
							tpl.appendSection();
							tpl.deselectSection();

							IDictionaryEnumerator i = info.exifTags.GetEnumerator();

							string[] tagsToShow = GetSetting("Tags").Replace(" ","").ToLower().Split(',');

							while (i.MoveNext()) {
								tpl.selectSection("EXIF_ITEM");

								string tagName = i.Key.ToString();
								string tagValue = i.Value.ToString();

								if (Array.IndexOf(tagsToShow,tagName.ToLower())  > -1) {
									Exif.Tag tag = (Exif.Tag)Enum.Parse(typeof(Exif.Tag),tagName,true);
									tpl.setField("EXIF_NAME", ExifUtil.GetTagTitle(tag));
									tpl.setField("EXIF_VALUE", tagValue);
									tpl.appendSection();
								}
								
								tpl.deselectSection();
							}
							
							
						} else {
							if (context.Request.QueryString["Size"] != null) {

								string[] sizes = GetSetting("Sizes").Split(' ');
															
								string fileName = "";

								if (context.Request.QueryString ["Size"] == "thumb") {
									fileName = relativePath; //HttpUtility.UrlDecode (context.Request.Path.Substring(context.Request.ApplicationPath.Length));
									fileName = Util.CombinePath ("/bin/data/thumbs", fileName + "-thumb.jpg");
									//fileName = Util.CombinePath ("/bin/data/thumbs", HttpUtility.UrlDecode (context.Request.Path) + "-thumb.jpg");
									fileName = context.Server.MapPath (fileName);
								} else {

									int imageWidth = Convert.ToInt32(sizes[0]);
									if (context.Request.QueryString["Size"] != null)
										imageWidth = Convert.ToInt32(context.Request.QueryString["Size"]);


									if (Array.IndexOf(sizes, imageWidth.ToString()) == -1)
										throw new Exception("The specified size is not allowed.");
								
									string smallFile = Util.CombinePath (smallDir, relativePath + "-" + imageWidth + ".jpg");
										//context.Request.Path.Substring(context.Request.ApplicationPath.Length) 
										//+  "-" + imageWidth + ".jpg");

									fileName = context.Server.MapPath (smallFile);

									if (File.Exists (fileName) == false) {
										//quickThumbs.ResizePhoto (context.Server.MapPath (relativePath), fileName, imageWidth);
										string localFile = context.Server.MapPath (relativePath);
										Image image = new Image(localFile);
										image.SaveResizedImage(imageWidth, fileName);
									}
								}

								context.Response.ContentType = "image/jpg";
								context.Response.WriteFile(fileName);
								context.Response.End();
								return;

							} else {
								if (Array.IndexOf(validExtensions,"." + fileExtension.ToLower()) > -1) {
									context.Response.ContentType = "image/" + fileExtension; 
								} else if (fileExtension.ToLower() == "css") {
									context.Response.ContentType = "text/css";
									context.Response.WriteFile(filePath);
									return;
								} else {
									throw new Exception("Files of type '" + fileExtension + "' are not supported.");
								}

								context.Response.WriteFile (filePath);
								context.Response.End();
								return;
							}
						}
					} else {
						// This is a file that doesn't exist.
						throw new HttpException(404, "The specified file doesn't exist.");
					}
				}
			
				tpl.setField("QUICKTHUMBS_URL", "http://extremeboredom.net/projects/quickthumbs/");
				tpl.setField("QUICKTHUMBS_VERSIONSTRING", "QuickThumbs 0.3");

				context.Response.Write(tpl.getContent());
			//	context.Response.Flush();

						
			} catch (Exception ex) {
				context.Response.Write("<pre>" + ex + "</pre>");
				log.Error("Exception in ProcessRequest!", ex);
			}
		
			if (context.Request.QueryString["debug"] != null && context.Request.QueryString["debug"] == "yes") {

				context.Response.Write("<ul id=\"debuglog\">");
				foreach (log4net.Core.LoggingEvent thisEvent in appender.GetEvents()) {
					if (thisEvent.ExceptionObject != null)
						context.Response.Write("<li><b>" + thisEvent.RenderedMessage + ":</b><br/><pre>" + thisEvent.ExceptionObject.ToString() + "</pre></li>");
					else
						context.Response.Write("<li><pre>" + thisEvent.RenderedMessage + "</pre></li>");
					Console.Error.WriteLine (thisEvent.RenderedMessage);
				}
				context.Response.Write("</ul>");
			}

			context.Response.End();
		}
		
			
		public bool IsReusable {
			get {
				return true;
			}
		}

		


		public static string FormatBytes(decimal bytes) {
			if (bytes >= 1099511627776)
				return Math.Round((bytes / 1024 / 1024 / 1024 / 1024),2).ToString() + " TB";
			else if (bytes >= 1073741824)
				return Math.Round((bytes / 1024 / 1024 / 1024),2).ToString() + " GB";
			else if (bytes >= 1024)
				return Math.Round((bytes / 1024 / 1024),2).ToString() + " MB";
			else if (bytes < 1024)
				return bytes.ToString() + " Bytes";
			else
				return "0 Bytes";
		}

		public static string GetSetting(string key) {
			try {
				return System.Configuration.ConfigurationSettings.AppSettings[key].ToString();
			} catch (Exception ex) {
				log.Info("Error in GetSetting(" + key + ")!", ex);
				throw ex;
			}
		}
	}
}

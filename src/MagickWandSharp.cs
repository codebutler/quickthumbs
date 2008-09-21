//
// MagickWandSharp.cs: 
// See http://www.imagemagick.org/script/magick-wand.php for more information
// 
// Authors:
//    Eric Butler <eric@extremeboredom.net>
//
// Copyright (C) 2006, Eric Butler
//

using System;
using System.Runtime.InteropServices;



public class MagickWand : IDisposable
{
	IntPtr magick_wand;

	public MagickWand ()
	{
		MagickWandInterop.MagickWandGenesis();

		magick_wand = MagickWandInterop.NewMagickWand();

	}

	public void ReadImage (string filename)
	{
		IntPtr filename_ptr = Marshal.StringToHGlobalAuto(filename);
		int status = MagickWandInterop.MagickReadImage(magick_wand, filename_ptr);
		if (status == MagickWandInterop.MagickFalse) {
			throw new Exception ("Failed to read image.");
		}
	}

	public void ResizeImage (uint width, uint height)
	{
		MagickWandInterop.FilterTypes type = MagickWandInterop.FilterTypes.LanczosFilter;
		MagickWandInterop.MagickResizeImage(magick_wand, width, height, type, 0.0);
	}

	public void TransformImage (string crop, string geometry)
	{
		IntPtr crop_ptr = Marshal.StringToHGlobalAuto(crop);
		IntPtr geometry_ptr = Marshal.StringToHGlobalAuto(geometry);
		magick_wand = MagickWandInterop.MagickTransformImage(magick_wand, crop_ptr, geometry_ptr);
	}

	public void WriteImage (string filename)
	{
		IntPtr filename_ptr = Marshal.StringToHGlobalAuto(filename);
		bool result = MagickWandInterop.MagickWriteImage (magick_wand, filename_ptr);
		if (!result) {
			throw new Exception ("Failed to write image.");
		}
	}

	public long ImageWidth {
		get {
			return MagickWandInterop.MagickGetImageWidth(magick_wand);
		}
	}

	public long ImageHeight {
		get {
			return MagickWandInterop.MagickGetImageHeight(magick_wand);
		}
	}

	public void Dispose ()
	{
		magick_wand = MagickWandInterop.DestroyMagickWand(magick_wand);
		MagickWandInterop.MagickWandTerminus();
	}


	private static class MagickWandInterop
	{
		[DllImport("libWand.so.9")]
		public static extern void MagickWandGenesis ();

		[DllImport("libWand.so.9")]
		public static extern void MagickWandTerminus ();

		[DllImport("libWand.so.9")]
		public static extern IntPtr NewMagickWand ();

		[DllImport("libWand.so.9")]
		public static extern int MagickReadImage (IntPtr magick_wand, IntPtr file_name);

		[DllImport("libWand.so.9")]
		public static extern int MagickResizeImage (IntPtr magick_wand, uint columns, uint rows, FilterTypes filter, double blur);

		[DllImport("libWand.so.9")]
		public static extern bool MagickWriteImage (IntPtr magick_wand, IntPtr file_name);
	
		[DllImport("libWand.so.9")]
		public static extern IntPtr DestroyMagickWand (IntPtr magick_wand);

		[DllImport("libWand.so.9")]
		public static extern int MagickGetImageWidth (IntPtr magick_wand);

		[DllImport("libWand.so.9")]
		public static extern int MagickGetImageHeight (IntPtr magick_wand);

		[DllImport("libWand.so.9")]
		public static extern IntPtr MagickTransformImage (IntPtr magick_wand, IntPtr crop, IntPtr geometry);
		
		public static readonly int MagickFalse = 0;

		public enum FilterTypes : int
		{
		  UndefinedFilter = 0,
		  PointFilter,
		  BoxFilter,
		  TriangleFilter,
		  HermiteFilter,
		  HanningFilter,
		  HammingFilter,
		  BlackmanFilter,
		  GaussianFilter,
		  QuadraticFilter,
		  CubicFilter,
		  CatromFilter,
		  MitchellFilter,
		  LanczosFilter,
		  BesselFilter,
		  SincFilter
		} 
	}
}

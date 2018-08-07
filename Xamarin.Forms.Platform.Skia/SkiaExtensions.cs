using SkiaSharp;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Skia
{
	public static class SkiaExtensions
	{
		public static SKColor ToSKColor(this Color color)
		{
			return new SKColor((byte)(byte.MaxValue * color.R), 
				(byte)(byte.MaxValue * color.G), 
				(byte)(byte.MaxValue * color.B),
				(byte)(byte.MaxValue * color.A));
		}
	}
}

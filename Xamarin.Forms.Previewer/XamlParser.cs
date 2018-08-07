using System;
using Xamarin.Forms.Xaml;
namespace  Xamarin.Forms.Previewer
{
	public class XamlParser
	{
		public static Element ParseXaml(string xaml)
		{
			//TODO: Determine what type it is.
			return new ContentPage().LoadFromXaml(xaml);
		}
	}
}

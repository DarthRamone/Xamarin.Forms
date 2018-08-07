using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Skia;
using Xamarin.Forms.Previewer;

using Rectangle = Xamarin.Forms.Rectangle;

namespace PreviewerWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			Forms.Init();

			InitializeComponent();
			XamlEntry.Text = XamlParser.XamlSimpleString;
			Previewer.Redraw += Previewer_Redraw;
		}

		private async void Previewer_Redraw(object sender, EventArgs e)
		{
			await Render();
		}

		private async void XamlEntry_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			await Render();
		}

		async Task Render()
		{

			var element = XamlParser.ParseXaml(XamlEntry.Text);
			//TODO: get sizes
			await Previewer.Draw(element, 480, 600);
		}
	}
}

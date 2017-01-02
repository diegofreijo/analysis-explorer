﻿using System;
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

namespace Explorer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var main = new MainViewModel();
			this.DataContext = main;

			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\method.png");
			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\reference.png");
			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\field.png");
			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\class.png");
			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\namespace.png");
			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\assembly.png");
			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\none.png");
			//Test(@"C:\Users\Edgar\Projects\AnalysisExplorer\Explorer\Images\close.png");

			RegisterCommandShortcuts(main);
		}

		private void RegisterCommandShortcuts(MainViewModel main)
		{
			foreach (var command in main.Commands)
			{
				if (command == null) continue;
				var inputBinding = new InputBinding(command, command.Shortcut);
				this.InputBindings.Add(inputBinding);
			}
		}

		private void GraphArea_LayoutAlgorithmTypeChanged(object sender, EventArgs e)
		{
			var graphArea = sender as DependencyObject;
			var zoomControl = graphArea.FindVisualParent<GraphX.Controls.ZoomControl>();

			if (zoomControl != null)
			{
				zoomControl.ZoomToFill();
			}
		}

		// Borrar referencia a System.Drawing.dll
		//private void Test(string fileName)
		//{
		//	var myBitmap = new System.Drawing.Bitmap(fileName);

		//	// Get the color of a background pixel.
		//	var backColor = myBitmap.GetPixel(0, 0);

		//	// Make backColor transparent for myBitmap.
		//	myBitmap.MakeTransparent(backColor);

		//	myBitmap.Save(fileName);
		//}
	}
}

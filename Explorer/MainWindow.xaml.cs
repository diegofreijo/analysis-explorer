﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\method.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\reference.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\field.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\static_field.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\class.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\namespace.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\assembly.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\none.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\close.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\save.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\open.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\options.png");
            //Test(@"C:\Users\Edgar\Projects\analysis-explorer\Explorer\Images\assembly2.png");

            RegisterCommandShortcuts(main.Commands);
			main.PropertyChanged += Main_PropertyChanged;
		}

		private void Main_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(MainViewModel.ToolBarCommands))
			{
				var main = this.DataContext as MainViewModel;
				RegisterCommandShortcuts(main.ToolBarCommands);
			}
		}

		private void RegisterCommandShortcuts(IEnumerable<IUICommand> commands)
		{
			this.InputBindings.Clear();

			foreach (var item in commands)
			{
				if (item.IsSeparator) continue;

				var command = item as MenuCommand;
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

		private void TreeView_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var main = this.DataContext as MainViewModel;
				var fileNames = e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string>;
				fileNames = fileNames.Where(fn => fn.EndsWith(".exe") || fn.EndsWith(".dll"));

				foreach (var fileName in fileNames)
				{
					main.LoadAssembly(fileName);
				}
			}
		}

		private void TreeView_PreviewDrop(object sender, DragEventArgs e)
		{
			var ok = false;

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
				ok = fileNames.Any(fn => fn.EndsWith(".exe") || fn.EndsWith(".dll"));
			}

			e.Handled = !ok;
		}

        //// Borrar referencia a System.Drawing.dll
        //private void Test(string fileName)
        //{
        //    var myBitmap = new System.Drawing.Bitmap(fileName);

        //    // Get the color of a background pixel.
        //    var backColor = myBitmap.GetPixel(0, 0);

        //    // Make backColor transparent for myBitmap.
        //    myBitmap.MakeTransparent(backColor);

        //    myBitmap.Save(fileName);
        //}
    }
}

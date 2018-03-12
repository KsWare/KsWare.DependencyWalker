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

namespace KsWare.DependencyWalker {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();
			Dispatcher.BeginInvoke(new Action(Target));

		}

		private void Target() {
			var ai = AssemblyWalker.LoadAssembly(
				@"D:\Develop\Extern\GitHub.KsWare\KsWare.Presentation\master\src\KsWare.Presentation.UITestApp\bin\Debug\KsWare.Presentation.UITestApp.exe");
			AssemblyWalker.LoadDependencies(ai, true);
			TreeView.ItemsSource = new[] {ai};
		}
	}
}

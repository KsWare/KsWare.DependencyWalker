using System;
using System.Windows;

namespace KsWare.DependencyWalker.UI {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();
			Dispatcher.BeginInvoke(new Action(Target));

		}

		private void Target() {
//			var ai = AssemblyWalker.LoadAssembly(
//				@"D:\Develop\Extern\GitHub.KsWare\KsWare.Presentation\master\src\KsWare.Presentation.UITestApp\bin\Debug\KsWare.Presentation.UITestApp.exe");
//			AssemblyWalker.LoadDependencies(ai, true);
//			TreeView.ItemsSource = new[] {ai};

//			Die Abhängigkeit von
//				Assembly "XYZ" kann nicht aufgelöst werden, da sie nicht vorher geladen wurde. Beim 
//				Verwenden der ReflectionOnly - APIs müssen  bhängige Assemblys vorher geladen oder über
//				das ReflectionOnlyAssemblyResolve - Ereignis bei Bedarf geladen werden.'
//
//			AssemblyWalker.LoadDependencies(ai,true);
//			ai.Types = AssemblyWalker.GetExportedTypes(ai.Assembly, true);
//			TypesTreeView.ItemsSource = new[] {ai};
		}
	}
}

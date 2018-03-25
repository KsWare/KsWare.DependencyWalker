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
using KsWare.DependencyWalker.AppDomainWorkers;

namespace KsWare.DependencyWalker {
	/// <summary>
	/// Interaction logic for TestlView.xaml
	/// </summary>
	public partial class TestlView : UserControl {
		public TestlView() {
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
			SampleAppDomainWorker.Execute();
		}
	}
}

using KsWare.Presentation.ViewModelFramework;

namespace KsWare.DependencyWalker.UI {

	public class MainWindowVM : WindowVM {

		public MainWindowVM() {
			RegisterChildren(() => this);
		}
	}

}

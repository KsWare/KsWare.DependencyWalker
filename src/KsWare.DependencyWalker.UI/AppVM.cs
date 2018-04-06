using KsWare.Presentation.ViewModelFramework;

namespace KsWare.DependencyWalker.UI {

	public class AppVM : ApplicationVM {

		public AppVM() {
			RegisterChildren(() => this);
			StartupUri = typeof(MainWindowVM);
		}

		
	}

}

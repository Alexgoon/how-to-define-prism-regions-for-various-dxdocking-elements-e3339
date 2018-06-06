using System.Collections.Specialized;
using System.ComponentModel.Composition;
using DevExpress.Xpf.Docking;
using Prism.Regions;

namespace PrismOnDXDocking.Infrastructure.Adapters {
    [Export(typeof(LayoutGroupAdapter)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class LayoutGroupAdapter : RegionAdapterBase<LayoutGroup> {
        [ImportingConstructor]
		public LayoutGroupAdapter(IRegionBehaviorFactory behaviorFactory) : 
            base(behaviorFactory) {
		}
		protected override IRegion CreateRegion() {
			 return new AllActiveRegion();
		}
       protected override void Adapt(IRegion region, LayoutGroup regionTarget) {
            region.Views.CollectionChanged += (s, e) => OnViewsCollectionChanged(region, regionTarget, s, e);
            regionTarget.Items.CollectionChanged += (s, e) => OnItemsCollectionChanged(region, regionTarget, s, e);
        }

        bool _lockItemsChanged;
        bool _lockViewsChanged;

        void OnItemsCollectionChanged(IRegion region, LayoutGroup regionTarget, object sender, NotifyCollectionChangedEventArgs e) {
            if (_lockItemsChanged)
                return;

            if (e.Action == NotifyCollectionChangedAction.Remove) {
                _lockViewsChanged = true;
                var lp = (LayoutPanel)e.OldItems[0];
                var view = lp.Content;
                lp.Content = null;
                region.Remove(view);
                _lockViewsChanged = false;
            }
        }

        void OnViewsCollectionChanged(IRegion region, LayoutGroup regionTarget, object sender, NotifyCollectionChangedEventArgs e) {
            if (_lockViewsChanged)
                return;

            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var view in e.NewItems) {
                    var panel = new LayoutPanel { Content = view };
                    _lockItemsChanged = true;
                    regionTarget.Items.Add(panel);
                    _lockItemsChanged = false;

                    regionTarget.SelectedTabIndex = regionTarget.Items.Count - 1;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var view in e.OldItems) {
                    LayoutPanel viewPanel = null;
                    foreach (LayoutPanel panel in regionTarget.Items) {
                        if (panel.Content == view) {
                            viewPanel = panel;
                            break;
                        }
                    }
                    if (viewPanel == null) continue;
                    viewPanel.Content = null;
                    _lockItemsChanged = true;
                    regionTarget.Items.Remove(viewPanel);
                    _lockItemsChanged = false;
                    regionTarget.SelectedTabIndex = regionTarget.Items.Count - 1;
                }
            }
        }
    }
}

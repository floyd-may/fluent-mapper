using System.Windows.Media;
using Common.Interfaces;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public class DriverViewModel<TEquipment> : OperatorViewModelBase<TEquipment>, IDriverViewModel
    {
        public DriverViewModel(ICacheableDriver<TEquipment> @operator) : base(@operator)
        {
        }

        public string Title { get { return "Driver"; } }
        public Color TitleColor { get { return Colors.Green; } }
    }
}
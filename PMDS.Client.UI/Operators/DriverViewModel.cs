using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Media;
using Common.Interfaces;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public class DriverViewModel<TEquipment> : OperatorViewModelBase<TEquipment>, IDriverViewModel
    {
        private readonly IDriverEditor _editor;

        public DriverViewModel(ICacheableDriver<TEquipment> @operator, IDriverEditor editor, IScheduler scheduler) : base(@operator, scheduler)
        {
            _editor = editor;
        }

        public string Title { get { return "Driver"; } }
        public Color TitleColor { get { return Colors.Green; } }
        
        protected override Task<bool> DoEdit()
        {
            return _editor.EditDriver(_operator.DriverId);
        }
    }

    public interface IDriverViewModelFactory<TEquipment>
    {
        IDriverViewModel Get(ICacheableDriver<TEquipment> @operator);
    }
}
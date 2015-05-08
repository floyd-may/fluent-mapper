using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Media;
using Common.Interfaces;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public class CarrierViewModel<TEquipment> : OperatorViewModelBase<TEquipment>, ICarrierViewModel
    {
        private readonly ICarrierEditor _carrierEditor;

        public CarrierViewModel(ICacheableDriver<TEquipment> @operator, ICarrierEditor carrierEditor, IScheduler scheduler)
            : base(@operator, scheduler)
        {
            _carrierEditor = carrierEditor;
        }

        public override string Title { get { return "Carrier"; } }
        public override Color TitleColor { get { return Colors.Blue; } }
        
        protected override Task<bool> DoEdit()
        {
            return _carrierEditor.EditCarrier(_operator.DriverId);
        }
    }

    public interface ICarrierViewModelFactory<TEquipment>
    {
        ICarrierViewModel Create(ICacheableDriver<TEquipment> operatorInstance);
    }
}
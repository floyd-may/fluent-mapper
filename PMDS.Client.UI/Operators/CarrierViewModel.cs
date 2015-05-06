using Common.Interfaces;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public class CarrierViewModel<TEquipment> : OperatorViewModelBase<TEquipment>, ICarrierViewModel
    {
        public CarrierViewModel(ICacheableDriver<TEquipment> @operator)
            : base(@operator)
        {
        }

        public string Title { get { return "Carrier"; } }
    }
}
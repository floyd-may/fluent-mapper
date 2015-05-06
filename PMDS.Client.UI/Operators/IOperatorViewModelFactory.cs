using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public interface IOperatorViewModelFactory<TOperator, TEquipment>
        where TOperator : ICacheableDriver<TEquipment>
    {
        IOperatorViewModel Create(ICacheableDriver<TEquipment> @operator);
    }
}
using System.Linq;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public abstract class OperatorViewModelBase<TEquipment> : IOperatorViewModel
    {
        private readonly ICacheableDriver<TEquipment> _operator;

        protected OperatorViewModelBase(ICacheableDriver<TEquipment> @operator)
        {
            _operator = @operator;
        }

        public string FirstName { get { return _operator.FirstName; } }
        public string LastName { get { return _operator.LastName; } }
        public string EmployeeId { get { return _operator.EmployeeId; } }
        public string Phone { get { return _operator.Phone; } }
        public bool IsActive { get { return _operator.IsActive; } }

        public bool PassesFilter(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return true;

            var upperFilterText = filterText.Trim().ToUpper();
            return new[] {FirstName, LastName, EmployeeId, Phone}
                .Where(x => x != null)
                .Any(x => x.ToUpper().Contains(upperFilterText));
        }
    }
}
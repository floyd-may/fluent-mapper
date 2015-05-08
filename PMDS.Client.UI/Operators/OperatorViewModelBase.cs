using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Common.Silverlight.Commanding;
using PDMS.Client.AppServices.Caching.Drivers;

namespace PMDS.Client.UI.Operators
{
    public abstract class OperatorViewModelBase<TEquipment> : IOperatorViewModel
    {
        protected readonly ICacheableDriver<TEquipment> _operator;
        private readonly IScheduler _scheduler;
        private readonly ICommand _editCommand;

        protected OperatorViewModelBase(ICacheableDriver<TEquipment> @operator, IScheduler scheduler)
        {
            _operator = @operator;
            _scheduler = scheduler;

            _editCommand = new CustomDelegateCommand(x => Edit());
        }

        public event EventHandler Edited;

        public string FirstName { get { return _operator.FirstName; } }
        public string LastName { get { return _operator.LastName; } }
        public string EmployeeId { get { return _operator.EmployeeId; } }
        public string Phone { get { return _operator.Phone; } }
        public bool IsActive { get { return _operator.IsActive; } }
        public bool IsHazmatCertified { get { return _operator.IsHazmatCertified; } }
        public byte[] Photo { get { return _operator.Photo; } }

        public abstract string Title { get; }
        public abstract Color TitleColor { get; }

        public bool PassesFilter(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return true;

            var upperFilterText = filterText.Trim().ToUpper();
            return new[] {FirstName, LastName, EmployeeId, Phone}
                .Where(x => x != null)
                .Any(x => x.ToUpper().Contains(upperFilterText));
        }

        public void Edit()
        {
            Observable.FromAsync(DoEdit)
                .ObserveOn(_scheduler)
                .Where(x => x)
                .Subscribe(x =>
                {
                    var handler = Edited;
                    if (null != handler)
                        handler(this, EventArgs.Empty);
                });
        }

        public ICommand EditCommand {
            get { return _editCommand; }
        }

        protected abstract Task<bool> DoEdit();
    }
}
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Extension
{
    public class AsyncDelegateCommand : DelegateCommandBase
    {
        private readonly Func<Task> _executeMethod;
        private readonly Func<bool> _canExecuteMethod;
        private bool _isExecuting;

        public AsyncDelegateCommand(Func<Task> executeMethod)
            : this(executeMethod, () => true)
        {
        }

        public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        {
            _executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            _canExecuteMethod = canExecuteMethod ?? throw new ArgumentNullException(nameof(canExecuteMethod));
        }

        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }

        protected override bool CanExecute(object parameter)
        {
            return !IsExecuting && _canExecuteMethod();
        }

        protected override async void Execute(object parameter)
        {
            IsExecuting = true;
            try
            {
                await _executeMethod();
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }
}

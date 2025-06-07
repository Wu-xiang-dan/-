using NoteBook.DTOS;
using NoteBook.ViewModels;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.ViewModels.Dialogs
{
    class AddWaitUcViewModel : IDialogAware
    {
        public string Title => "添加待办事项";
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public event Action<IDialogResult> RequestClose;
        public WaitVieModel WaitInfo { get; set; }
        public bool CanCloseDialog() => true;
        public AddWaitUcViewModel()
        {
            WaitInfo = new WaitVieModel();
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
        }
        public void SaveCommandExecute()
        {
            if (string.IsNullOrEmpty(WaitInfo.Content)||string.IsNullOrEmpty(WaitInfo.Title))
            {
                return;
            }
            DialogParameters pairs=new DialogParameters();
            pairs.Add("WaitDTO", WaitInfo);
            var result = new DialogResult(ButtonResult.OK,pairs);
            RequestClose?.Invoke(result);
        }
        public void CancelCommandExecute()
        {
            var result = new DialogResult(ButtonResult.Cancel);
            RequestClose?.Invoke(result);
        }

        public void OnDialogClosed()
        {
          
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
           
        }
    }
}

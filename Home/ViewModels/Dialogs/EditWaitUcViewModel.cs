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
    class EditWaitUcViewModel:IDialogAware
    {
        public string Title => "修改待办事项";
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public event Action<IDialogResult> RequestClose;
        public WaitVieModel WaitInfo { get; set; }
        public bool CanCloseDialog() => true;
       public DialogParameters pairs = new DialogParameters();
        public EditWaitUcViewModel()
        {
            WaitInfo = new WaitVieModel();
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
        }
        public void SaveCommandExecute()
        {
            if (string.IsNullOrEmpty(WaitInfo.Content) || string.IsNullOrEmpty(WaitInfo.Title))
            {
                return;
            }
            pairs.Add("newValue", WaitInfo);
            var result = new DialogResult(ButtonResult.OK, pairs);
            RequestClose?.Invoke(result);
        }
        public void CancelCommandExecute()
        {
            // 执行取消操作
            // 这里可以添加取消逻辑，例如清理输入框等
            // 然后关闭对话框并返回结果
            var result = new DialogResult(ButtonResult.Cancel);
            RequestClose?.Invoke(result);
        }

        public void OnDialogClosed()
        {
            // 清理资源，不需要抛出异常
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            WaitVieModel oldValue = parameters.GetValue<WaitVieModel>("oldValue");
            pairs.Add("oldValue",oldValue);
            if(oldValue!=null)
            {
                WaitInfo.Id = oldValue.Id;
                WaitInfo.Content = oldValue.Content;
                WaitInfo.Title = oldValue.Title;
                WaitInfo.Status = oldValue.Status;
            }

        }
    }
}

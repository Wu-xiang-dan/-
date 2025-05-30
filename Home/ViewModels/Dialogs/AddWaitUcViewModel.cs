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
            pairs.Add("WaitInfoDTO", WaitInfo);
            var result = new DialogResult(ButtonResult.OK,pairs);
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
            // 处理传入参数（如果有），不需要抛出异常
        }
    }
}

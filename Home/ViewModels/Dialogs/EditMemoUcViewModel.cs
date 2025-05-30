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
    class EditMemoUcViewModel:IDialogAware
    {
        public string Title => "修改备忘录";
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public event Action<IDialogResult> RequestClose;
        public MemoViewModel MemoInfo { get; set; }
        public bool CanCloseDialog() => true;
        public DialogParameters pairs = new DialogParameters();
        public EditMemoUcViewModel()
        {
            MemoInfo = new MemoViewModel();
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
        }
        public void SaveCommandExecute()
        {
            if (string.IsNullOrEmpty(MemoInfo.Content) || string.IsNullOrEmpty(MemoInfo.Title))
            {
                return;
            }
            pairs.Add("newValue", MemoInfo);
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
            MemoViewModel oldValue = parameters.GetValue<MemoViewModel>("oldValue");
            pairs.Add("oldValue", oldValue);
            if (oldValue != null)
            {
                MemoInfo.MemoID = oldValue.MemoID;
                MemoInfo.Content = oldValue.Content;
                MemoInfo.Title = oldValue.Title;      
            }

        }
    }
}

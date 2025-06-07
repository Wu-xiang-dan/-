using Newtonsoft.Json;
using NoteBook.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Data
{
    public class JsonSerializerService : IJsonSerializerService
    {
        public async Task<List<WaitVieModel>> LoadingWaitsJsonAsync()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "NoteBookApp");
            Directory.CreateDirectory(appFolder); // 提前创建目录
            string jsonPath = Path.Combine(appFolder, "Waits.json");
            if (!File.Exists(jsonPath))// 文件不存在，返回空列表（首次运行时初始化）           
                return new List<WaitVieModel>();

            try
            {
                string json = await File.ReadAllTextAsync(jsonPath);
                return JsonConvert.DeserializeObject<List<WaitVieModel>>(json) ?? new List<WaitVieModel>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON解析错误，重置文件：{ex.Message}");
                File.Delete(jsonPath); // 删除损坏文件
                return new List<WaitVieModel>();
            }
        }
        public async Task<List<MemoViewModel>> LoadingMemosJsonAsync()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "NoteBookApp");
            Directory.CreateDirectory(appFolder); // 提前创建目录
            string jsonPath = Path.Combine(appFolder, "Memos.json");
            if (!File.Exists(jsonPath))
                return new List<MemoViewModel>();

            try
            {
                string json = await File.ReadAllTextAsync(jsonPath);
                return JsonConvert.DeserializeObject<List<MemoViewModel>>(json) ?? new List<MemoViewModel>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON解析错误，重置文件：{ex.Message}");
                File.Delete(jsonPath); // 删除损坏文件
                return new List<MemoViewModel>();
            }
        }
        public void SaveWaitsToJson(List<WaitVieModel> waits)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "NoteBookApp");
            Directory.CreateDirectory(appFolder); // 确保目录存在
            string jsonPath = Path.Combine(appFolder, "Waits.json");

            try
            {
                string json = JsonConvert.SerializeObject(waits, Formatting.Indented);
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception)
            {
               
            }
        }
        public void  SaveMemosToJson(List<MemoViewModel> memo)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "NoteBookApp");
            Directory.CreateDirectory(appFolder); // 确保目录存在
            string jsonPath = Path.Combine(appFolder, "Memos.json"); // 修正为Memos.json
            try
            {
                string json = JsonConvert.SerializeObject(memo, Formatting.Indented);
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception)
            {
                
            }
        }
    }
}

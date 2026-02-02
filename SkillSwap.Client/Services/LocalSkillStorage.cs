using Blazored.LocalStorage;
using SkillSwap.Client.Models;

namespace SkillSwap.Client.Services
{
    public class LocalSkillStore
    {
        private const string TeachKey = "skills_teach_v1";
        private const string NeedKey = "skills_need_v1";
        private readonly ILocalStorageService _ls;

        public LocalSkillStore(ILocalStorageService ls)
        {
            _ls = ls;
        }

        public async Task<List<int>> GetTeachAsync() =>
            await _ls.GetItemAsync<List<int>>(TeachKey) ?? new List<int>();

        public async Task<List<int>> GetNeedAsync() =>
            await _ls.GetItemAsync<List<int>>(NeedKey) ?? new List<int>();

        public async Task SaveTeachAsync(List<int> ids) =>
            await _ls.SetItemAsync(TeachKey, ids);

        public async Task SaveNeedAsync(List<int> ids) =>
            await _ls.SetItemAsync(NeedKey, ids);

        public async Task ClearAsync()
        {
            await _ls.RemoveItemAsync(TeachKey);
            await _ls.RemoveItemAsync(NeedKey);
        }
    }
}
using SkillSwap.Client.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SkillSwap.Client.Services
{
    public interface ISkillService
    {
        Task<ApiResponse<List<Skill>>> GetAllSkillsAsync();
        Task<ApiResponse<Skill>> CreateSkillAsync(Skill skill);
    }

    public class SkillService : ISkillService
    {
        private readonly HttpClient _httpClient;

        public SkillService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<List<Skill>>> GetAllSkillsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/skills");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // Try to parse as PagedResult first
                    try
                    {
                        var pagedResult = JsonSerializer.Deserialize<PagedResult<Skill>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (pagedResult?.Items != null)
                        {
                            return new ApiResponse<List<Skill>> 
                            { 
                                Success = true, 
                                Data = pagedResult.Items 
                            };
                        }
                    }
                    catch
                    {
                        var skills = JsonSerializer.Deserialize<List<Skill>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        return new ApiResponse<List<Skill>> 
                        { 
                            Success = true, 
                            Data = skills ?? new List<Skill>() 
                        };            
                    }
                }
                return new ApiResponse<List<Skill>> 
                { 
                    Success = false, 
                    ErrorMessage = $"Failed to load skills: {response.StatusCode}" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Skill>> 
                { 
                    Success = false, 
                    ErrorMessage = ex.Message 
                };
            }
        }

        public async Task<ApiResponse<Skill>> CreateSkillAsync(Skill skill)
        {
            try
            {
                var createDto = new 
                {
                    Name = skill.Name,
                    Description = skill.Description
                };
                var response = await _httpClient.PostAsJsonAsync("api/skills", createDto);
                if (response.IsSuccessStatusCode)
                {
                    var createdSkill = await response.Content.ReadFromJsonAsync<Skill>();
                    return new ApiResponse<Skill> { Success = true, Data = createdSkill };
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return new ApiResponse<Skill> { Success = false, ErrorMessage = error?.Message ?? "Error creating skill" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Skill> { Success = false, ErrorMessage = ex.Message };
            }
        }
        public class PagedResult<T>
        {
            public List<T> Items { get; set; } = new();
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public int TotalPages { get; set; }
        }
    }
}
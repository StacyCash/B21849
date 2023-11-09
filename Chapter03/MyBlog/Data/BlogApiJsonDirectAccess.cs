using Data.Models.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Data.Models;
using System.ComponentModel.Design;

namespace Data;
public class BlogApiJsonDirectAccess : IBlogApi
{
    BlogApiJsonDirectAccessSetting _settings;
    public BlogApiJsonDirectAccess(IOptions<BlogApiJsonDirectAccessSetting> option)
    {
        _settings = option.Value;

        ManageDataPaths();
    }
    private void ManageDataPaths()
    {
        CreateDirectoryIfNotExists(_settings.DataPath);
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.BlogPostsFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.CategoriesFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.TagsFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.CommentsFolder}");
    }

    private static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private List<T> Load<T>(string folder)
    {
        var list = new List<T>();
        foreach (var f in Directory.GetFiles($@"{_settings.DataPath}\{folder}"))
        {
            var json = File.ReadAllText(f);
            var blogPost = JsonSerializer.Deserialize<T>(json);
            if (blogPost is not null)
            {
                list.Add(blogPost);
            }
        }

        return list;
    }

    private async Task SaveAsync<T>(string folder, string filename, T item)
    {
        var filepath = $@"{_settings.DataPath}\{folder}\{filename}.json";
        await File.WriteAllTextAsync(filepath, JsonSerializer.Serialize<T>(item));
    }

    private void Delete(string folder, string filename)
    {
        var filepath = $@"{_settings.DataPath}\{folder}\{filename}.json";
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
    }


    public async Task<int> GetBlogPostCountAsync()
    {
        var list = Load<BlogPost>(_settings.BlogPostsFolder);
        return await Task.FromResult(list.Count);
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync(int numberofposts, int startindex)
    {
        var list = Load<BlogPost>(_settings.BlogPostsFolder);
        return await Task.FromResult(list.Skip(startindex).Take(numberofposts).ToList());
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var list = Load<Category>(_settings.CategoriesFolder);
        return await Task.FromResult(list);
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        var list = Load<Category>(_settings.CategoriesFolder);
        return await Task.FromResult(list.FirstOrDefault(c => c.Id == id));
    }


    public async Task<List<Tag>> GetTagsAsync()
    {
        var list = Load<Tag>(_settings.TagsFolder);
        return await Task.FromResult(list);
    }

    public async Task<Tag?> GetTagAsync(string id)
    {
        var list = Load<Tag>(_settings.TagsFolder);
        return await Task.FromResult(list.FirstOrDefault(t => t.Id == id));
    }

    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        var list = Load<BlogPost>(_settings.BlogPostsFolder);
        return await Task.FromResult(list.FirstOrDefault(bp => bp.Id == id));
    }

   



    public async Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.BlogPostsFolder, item.Id, item);
        return item;
    }

    public async Task<Category?> SaveCategoryAsync(Category item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.CategoriesFolder, item.Id, item);
        return item;
    }

    public async Task<Tag?> SaveTagAsync(Tag item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.TagsFolder, item.Id, item);
        return item;
    }
    public async Task<Comment?> SaveCommentAsync(Comment item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync(_settings.CommentsFolder, item.Id, item);
        return item;
    }



    public async Task DeleteBlogPostAsync(string id)
    {
        var blogPost = GetBlogPostAsync(id);
        if (blogPost is not null)
        {
            Delete(_settings.BlogPostsFolder, id);
        }

        var comments = await GetCommentsAsync(id);
        foreach (var comment in comments)
        {
            if (comment.Id != null)
            {
                Delete(_settings.CommentsFolder, comment.Id);
            }
        }
    }

    public Task DeleteCategoryAsync(string id)
    {
        var category = GetCategoryAsync(id);
        if (category is not null)
        {
            Delete(_settings.CategoriesFolder, id);
        }
        return Task.CompletedTask;
    }

    public Task DeleteTagAsync(string id)
    {
        Delete(_settings.TagsFolder, id);
        return Task.CompletedTask;
    }

    public Task DeleteCommentAsync(string id)
    {
        Delete(_settings.CommentsFolder, id);
        return Task.CompletedTask;
    }

    public async Task<List<Comment>> GetCommentsAsync(string blogPostId)
    {
        var list = Load<Comment>(_settings.CommentsFolder);
        return await Task.FromResult(list.Where(t => t.BlogPostId == blogPostId).ToList());
    }
}

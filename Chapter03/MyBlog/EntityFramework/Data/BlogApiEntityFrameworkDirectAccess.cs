using Data.Entities;
using Data.Models;
using Data.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class BlogApiEntityFrameworkDirectAccess(IDbContextFactory<BlogDbContext> factory) : IBlogApi
{
    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var item = await context
            .BlogPosts
            .Include(blogpost => blogpost.Category)
            .Include(blogpost => blogpost.Tags)
            .FirstOrDefaultAsync(blogpost => blogpost.Id == id);
        return item;
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync(int numberofposts, int startindex)
    {
        await using var context = await factory.CreateDbContextAsync();
        var blogposts = await context
            .BlogPosts
            .OrderByDescending(blogpost => blogpost.PublishDate)
            .Skip(startindex)
            .Take(numberofposts)
            .ToListAsync();
        return blogposts;
    }

    public async Task<int> GetBlogPostCountAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        var blogpostCount = await context.BlogPosts.CountAsync();
        return blogpostCount;

    }


    public async Task<List<Category>> GetCategoriesAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        var categories = await context
            .Categories
            .ToListAsync();
        return categories;
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var category = await context
            .Categories
            .FirstOrDefaultAsync(category => category.Id == id);
        return category;
    }

    public async Task<Tag?> GetTagAsync(string id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var tag = await context
            .Tags
            .FirstOrDefaultAsync(tag => tag.Id == id);
        return tag;
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        var tags = await context
            .Tags
            .ToListAsync();
        return tags;
    }

    private async Task DeleteItemAsync<T>(T item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        await using var context = await factory.CreateDbContextAsync();
        context.Remove(item);
        await context.SaveChangesAsync();
    }

    public async Task DeleteBlogPostAsync(string id)
    {
        var item=GetBlogPostAsync(id);
        await DeleteItemAsync(item);
    }

    public async Task DeleteCategoryAsync(string id)
    {
        var item=GetCategoryAsync(id);
        await DeleteItemAsync(item);
    }

    public async Task DeleteTagAsync(string id)
    {
        var item=GetTagAsync(id);
        await DeleteItemAsync(item);
    }

    public async Task DeleteCommentAsync(string id)
    {
        var item=GetCommentAsync(id);
        await DeleteItemAsync(item);
    }

    public async Task<Comment?> GetCommentAsync(string id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var comment = await context.Comments.FirstOrDefaultAsync(comment => comment.Id == id);
        return comment;
    }

    private async Task<T> CreateItem<T>(T item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item)); await using var context = await factory.CreateDbContextAsync();
        context.Add(item);
        await context.SaveChangesAsync();
        return item;
    }
    
    private async Task<T> UpdateItem<T>(T item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        await using var context = await factory.CreateDbContextAsync();
        context.Update(item);
        await context.SaveChangesAsync();
        return item;
    }



    public async Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        if (item.Id is null)
        {
            return await CreateItem(item);
        }
        return await UpdateItem(item);
    }

    public async Task<Category?> SaveCategoryAsync(Category item)
    {
        if (item.Id is null)
        {
            return await CreateItem(item);
        }
        return await UpdateItem(item);
    }

    public async Task<Tag?> SaveTagAsync(Tag item)
    {
        if (item.Id is null)
        {
            return await CreateItem(item);
        }
        return await UpdateItem(item);
    }

    public async Task<Comment?> SaveCommentAsync(Comment item)
    {
        if (item.Id is null)
        {
            return await CreateItem(item);
        }
        return await UpdateItem(item);
    }

    public async Task<List<Comment>> GetCommentsAsync(string blogPostId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context
            .Comments
            .Where(comment => comment.BlogPostId == blogPostId)
            .ToListAsync();
    }
}
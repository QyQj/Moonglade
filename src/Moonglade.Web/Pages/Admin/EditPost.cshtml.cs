using Microsoft.AspNetCore.Mvc.RazorPages;
using Moonglade.Core.CategoryFeature;
using Moonglade.Core.PostFeature;

namespace Moonglade.Web.Pages.Admin;

public class EditPostModel(IMediator mediator, ITimeZoneResolver timeZoneResolver, IBlogConfig blogConfig) : PageModel
{
    public PostEditModel ViewModel { get; set; } = new()
    {
        IsOutdated = false,
        IsPublished = false,
        Featured = false,
        EnableComment = true,
        FeedIncluded = true
    };

    public List<CategoryCheckBox> CategoryList { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        var cats = await mediator.Send(new GetCategoriesQuery());

        if (id is null)
        {
            if (cats.Count > 0)
            {
                var cbCatList = cats.Select(p =>
                    new CategoryCheckBox
                    {
                        Id = p.Id,
                        DisplayText = p.DisplayName,
                        IsChecked = false
                    });

                CategoryList = cbCatList.ToList();
            }

            ViewModel.Author = blogConfig.GeneralSettings.OwnerName;

            return Page();
        }

        var post = await mediator.Send(new GetPostByIdQuery(id.Value));
        if (null == post) return NotFound();

        ViewModel = new()
        {
            PostId = post.Id,
            IsPublished = post.IsPublished,
            EditorContent = post.PostContent,
            Author = post.Author,
            Slug = post.Slug,
            Title = post.Title,
            EnableComment = post.CommentEnabled,
            FeedIncluded = post.IsFeedIncluded,
            LanguageCode = post.ContentLanguageCode,
            Abstract = post.ContentAbstract.Replace("\u00A0\u2026", string.Empty),
            Featured = post.IsFeatured,
            OriginLink = post.OriginLink,
            HeroImageUrl = post.HeroImageUrl,
            IsOutdated = post.IsOutdated
        };

        if (post.PubDateUtc is not null)
        {
            ViewModel.PublishDate = timeZoneResolver.ToTimeZone(post.PubDateUtc.GetValueOrDefault());
        }

        var tagStr = post.Tags
            .Select(p => p.DisplayName)
            .Aggregate(string.Empty, (current, item) => current + item + ",");

        tagStr = tagStr.TrimEnd(',');
        ViewModel.Tags = tagStr;

        if (cats.Count > 0)
        {
            var cbCatList = cats.Select(p =>
                new CategoryCheckBox
                {
                    Id = p.Id,
                    DisplayText = p.DisplayName,
                    IsChecked = post.PostCategory.Any(q => q.CategoryId == p.Id)
                });
            CategoryList = cbCatList.ToList();
        }

        return Page();
    }
}

public class CategoryCheckBox
{
    public Guid Id { get; set; }
    public string DisplayText { get; set; }
    public bool IsChecked { get; set; }
}
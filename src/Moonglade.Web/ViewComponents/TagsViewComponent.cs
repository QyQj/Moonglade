﻿using Moonglade.Core.TagFeature;

namespace Moonglade.Web.ViewComponents;

public class TagsViewComponent(IBlogConfig blogConfig, IMediator mediator) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tags = await mediator.Send(new GetHotTagsQuery(blogConfig.GeneralSettings.HotTagAmount));
        return View(tags);
    }
}
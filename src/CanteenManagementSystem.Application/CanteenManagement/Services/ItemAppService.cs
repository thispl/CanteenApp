using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service for Item management
/// </summary>
[Authorize]
public class ItemAppService : ApplicationService, IItemAppService
{
    private readonly IItemRepository _itemRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ItemAppService(
        IItemRepository itemRepository,
        IGuidGenerator guidGenerator)
    {
        _itemRepository = itemRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<ItemDto?> GetAsync(Guid id)
    {
        var item = await _itemRepository.GetAsync(id);
        return ObjectMapper.Map<Item, ItemDto>(item);
    }

    public virtual async Task<PagedResultDto<ItemDto>> GetListAsync(ItemListFilterDto input)
    {
        var count = await _itemRepository.GetCountAsync(input.Filter);
        var items = await _itemRepository.GetListAsync(
            input.Filter,
            CancellationToken.None);

        var pagedItems = items
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<ItemDto>(
            count,
            ObjectMapper.Map<List<Item>, List<ItemDto>>(pagedItems));
    }

    public virtual async Task<ItemDto> CreateAsync(CreateItemDto input)
    {
        if (input.Price < 0)
        {
            throw new UserFriendlyException("Price must be greater than or equal to 0.");
        }

        var item = new Item(
            _guidGenerator.Create(),
            input.Description,
            input.Price);

        await _itemRepository.InsertAsync(item);

        return ObjectMapper.Map<Item, ItemDto>(item);
    }

    public virtual async Task<ItemDto> UpdateAsync(Guid id, UpdateItemDto input)
    {
        var item = await _itemRepository.GetAsync(id);

        if (input.Price < 0)
        {
            throw new UserFriendlyException("Price must be greater than or equal to 0.");
        }

        item.SetDescription(input.Description);
        item.SetPrice(input.Price);

        await _itemRepository.UpdateAsync(item);

        return ObjectMapper.Map<Item, ItemDto>(item);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _itemRepository.DeleteAsync(id);
    }
}

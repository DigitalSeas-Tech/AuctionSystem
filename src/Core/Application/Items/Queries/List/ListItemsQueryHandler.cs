﻿namespace Application.Items.Queries.List
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common.Helpers;
    using Common.Interfaces;
    using Common.Models;
    using Domain.Entities;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class ListItemsQueryHandler : IRequestHandler<ListItemsQuery, PagedResponse<ListItemsResponseModel>>
    {
        private readonly IAuctionSystemDbContext context;
        private readonly IMapper mapper;

        public ListItemsQueryHandler(IAuctionSystemDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PagedResponse<ListItemsResponseModel>> Handle(
            ListItemsQuery request,
            CancellationToken cancellationToken)
        {
            var skipCount = (request.PageNumber - 1) * request.PageSize;
            var queryable = this.context.Items.AsQueryable();

            var totalItemsCount = await this.context.Items.CountAsync(cancellationToken);
            if (request?.Filters == null)
            {
                return PaginationHelpers.CreatePaginatedResponse(request, await queryable
                    .Skip(skipCount)
                    .Take(request.PageSize)
                    .ProjectTo<ListItemsResponseModel>(this.mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken), totalItemsCount);
            }

            queryable = AddFiltersOnQuery(request.Filters, queryable);
            var itemsList = await queryable
                .Skip(skipCount)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = itemsList
                .Select(this.mapper.Map<ListItemsResponseModel>)
                .ToList();

            var result = PaginationHelpers.CreatePaginatedResponse(request, items, totalItemsCount);
            return result;
        }

        private static IQueryable<Item> AddFiltersOnQuery(ListAllItemsQueryFilter filters, IQueryable<Item> queryable)
        {
            if (!string.IsNullOrEmpty(filters?.Title))
            {
                queryable = queryable.Where(i => i.Title.ToLower().Contains(filters.Title.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters?.UserId))
            {
                queryable = queryable.Where(i => i.UserId == filters.UserId);
            }

            if (filters?.GetLiveItems != null)
            {
                queryable = queryable.Where(i => i.StartTime < DateTime.UtcNow && i.EndTime > DateTime.UtcNow);
            }

            if (filters?.StartingPrice != null)
            {
                queryable = queryable.Where(i => i.StartingPrice >= filters.StartingPrice);
            }

            if (filters?.StartTime != null)
            {
                queryable = queryable.Where(i => i.StartTime >= filters.StartTime);
            }

            if (filters?.EndTime != null)
            {
                queryable = queryable.Where(i => i.EndTime <= filters.EndTime);
            }

            if (filters?.MinimumPicturesCount != null)
            {
                queryable = queryable.Where(i => i.Pictures.Count >= filters.MinimumPicturesCount);
            }

            if (filters?.SubCategoryId != Guid.Empty)
            {
                queryable = queryable.Where(i => i.SubCategoryId == filters.SubCategoryId);
            }

            return queryable;
        }
    }
}
﻿namespace Application.Items.Commands.UpdateItem
{
    using System;
    using MediatR;

    public class UpdateItemCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal StartingPrice { get; set; }

        public decimal MinIncrease { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Guid SubCategoryId { get; set; }
    }
}
﻿namespace Application.Pictures.Queries
{
    using System;
    using Common.Models;
    using MediatR;

    public class GetPictureDetailsQuery : IRequest<MultiResponse<PictureDetailsResponseModel>>
    {
        public GetPictureDetailsQuery(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; }
    }
}

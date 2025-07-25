﻿using MediatR;
using Microsoft.AspNetCore.Http;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Images.UploadImage;

public record UploadImageCommand(string EntityName, Guid EntityId, IFormFile Image):IRequest<Result<ImageFullResponse?>>;
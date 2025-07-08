using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Infrastructure.Services;

[ServiceImplementation]
[RegisterAs<IPdfGenerator>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class PdfGenerator:IPdfGenerator
{
    public async Task<PdfResult> GeneratePdfAsync(Booking booking,CancellationToken cancellationToken=default)
    {
        return await Task.Run(() =>
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.Header().Column(header =>
                    {
                        header.Item().Text("Hotel Booking Invoice")
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        header.Item().Text($"Confirmation Number: {booking.ConfirmationNumber ?? "-"}")
                            .FontSize(12)
                            .FontColor(Colors.Grey.Darken2);
                    });

                    page.Content().PaddingVertical(0.5f, Unit.Centimetre).Column(content =>
                    {
                        content.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        content.Spacing(10);

                        content.Item().Text($"Hotel: {booking.Hotel?.Name ?? "N/A"}")
                            .FontSize(14);

                        content.Item().Text($"Guest: {booking.User?.FirstName + booking.User?.LastName}")
                            .FontSize(14);

                        content.Item().Text($"Check-In Date: {booking.CheckIn:dd MMM yyyy}")
                            .FontSize(14);
                        content.Item().Text($"Check-Out Date: {booking.CheckOut:dd MMM yyyy}")
                            .FontSize(14);

                        content.Item().Text($"Status: {booking.Status}")
                            .FontSize(14);

                        content.Item().Text($"Total Cost: ${booking.TotalCost}")
                            .FontSize(14)
                            .Bold();

                        content.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        content.Item().Text("Rooms:")
                            .FontSize(14)
                            .Bold();


                        if (booking.Rooms.Count != 0)
                        {
                            foreach (var room in booking.Rooms)
                            {
                                content.Item()
                                    .Text(
                                        $"- Room: {room.Number}, Room Type: {room.RoomType!.Name}, Adult Capacity: {room.AdultCapacity}, Children Capacity: {room.ChildrenCapacity}")
                                    .FontSize(14);
                            }
                        }
                        else
                        {
                            content.Item().Text("No room details available.")
                                .FontSize(14)
                                .Italic()
                                .FontColor(Colors.Grey.Darken1);
                        }
                    });
                });
            });
            var fileName = $"Invoice-{booking.ConfirmationNumber}.pdf";
            return new PdfResult(document.GeneratePdf(),fileName);
        }, cancellationToken);
    }
}
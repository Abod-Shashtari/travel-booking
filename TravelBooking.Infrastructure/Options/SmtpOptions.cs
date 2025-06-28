﻿namespace TravelBooking.Infrastructure.Options;

public class SmtpOptions
{
    public string Host { get; set; }=string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; }=string.Empty;
    public string Password { get; set; }=string.Empty;
    public string FromEmail { get; set; }=string.Empty;
}
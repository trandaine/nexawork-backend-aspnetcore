namespace NexaWork.Authentication.Services
{
    public static class EmailTemplates
    {
        public static string GetVerificationEmailHtml(string code, string title, string description)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>{title}</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, Helvetica, sans-serif; background-color: #ffffff; color: #333333;"">
    <div style=""max-width: 600px; margin: 0 auto; padding-bottom: 20px;"">
        <div style=""background-color: #194bf0; padding: 25px 30px; text-align: left;"">
            <h1 style=""color: #ffffff; margin: 0; font-size: 26px; font-weight: bold; letter-spacing: 1px;"">
                NexaWork
            </h1>
        </div>
        <div style=""padding: 40px 30px;"">
            <h2 style=""font-size: 22px; margin-top: 0; margin-bottom: 30px; color: #222222;"">
                {title}
            </h2>
            <div style=""background-color: #c9f5f9; padding: 30px; margin-bottom: 35px;"">
                <p style=""margin-top: 0; margin-bottom: 18px; font-size: 16px; line-height: 1.5;"">
                    Hi,
                </p>
                <p style=""margin-top: 0; margin-bottom: 18px; font-size: 16px; line-height: 1.5;"">
                    {description}
                </p>
                <h2 style=""font-size: 32px; margin-top: 0; margin-bottom: 30px; color: #194bf0; letter-spacing: 4px; text-align: center;"">
                    {code}
                </h2>
                <p style=""margin-top: 0; margin-bottom: 0; font-size: 16px; line-height: 1.5;"">
                    If this wasn't you, please ignore this email. Have a great day!
                </p>
            </div>
            <p style=""margin-top: 0; margin-bottom: 15px; font-size: 15px; color: #555555;"">
                Do not reply to this email. This message was sent from an unmonitored email address.
            </p>
            <p style=""margin-top: 0; margin-bottom: 40px; font-size: 15px; color: #555555;"">
                For inquiries, please contact our Technical Support at
                <a href=""https://www.nexawork.com/support"" style=""color: #74C6D4; text-decoration: none;"">www.nexawork.com/support</a>.
            </p>
            <p style=""margin-top: 0; margin-bottom: 8px; font-size: 15px;"">Sincerely,</p>
            <p style=""margin-top: 0; margin-bottom: 0; font-size: 15px;"">NexaWork Teams</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
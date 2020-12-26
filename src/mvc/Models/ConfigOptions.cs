namespace mvc.Models
{

    public class ConfigOptions
    {

        public Auth Auth { get; set; }
        public Apis Apis { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class Auth
    {
        public string Authority { get; set; }
        public string ClientID { get; set; }
        public string Secret { get; set; }
    }

    public class Weather
    {
        public string URL { get; set; }
    }

    public class Apis
    {
        public Weather Weather { get; set; }
    }

    


}

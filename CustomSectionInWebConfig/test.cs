NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("EnvironmentConfiguration");
string userName = section["Environment"];

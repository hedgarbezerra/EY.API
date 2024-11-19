﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class AuthenticationOptions
    {
        public const string SettingsKey = "Authentication";

        public Auth0Options Auth0 { get; set; }
        public KeycloakOptions Keycloak { get; set; }

    }

    public class Auth0Options
    {
        public const string Schema = "Auth0";
    }

    public class KeycloakOptions
    {
        public const string Schema = "Keycloak";

    }
}

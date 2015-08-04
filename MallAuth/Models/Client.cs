using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MallAuth.Models
{
    public class Client
    {
        public string Id { get; set; }

        public string Secret { get; set; }

        public string Name { get; set; }

        public ApplicationTypes ApplicationType { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public string AllowedOrigin { get; set; }
    }

    public enum ApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApp.DataModels
{
    public class UserProfile
    {
        public string UserId { get; set; }

        public string ImageContainerId { get; set; }

        public UserProfile(string userID, string imageContainerID)
        {
            this.UserId = userID;
            this.ImageContainerId = imageContainerID;
        }
    }
}
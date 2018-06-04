using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageService.Communication.Client;

namespace ImageServiceWeb.Models
{
    public class HomePageModel
    {
        public IClient TcpClient
        {
            get
            {
                return SingletonClient.Instance;
            }
        }

        public List<Student> Students { get; } = new List<Student>()
        {
            new Student { FirstName = "Dan", LastName = "Teplitski", ID = 312895147 },
            new Student { FirstName = "Bar", LastName = "Katz", ID = 1111}
        };
    }
}
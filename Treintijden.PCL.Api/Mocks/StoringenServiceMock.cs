using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ActueelNS.Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using ActueelNS.Services.Models;

namespace ActueelNS.Services.Mocks
{
    public class StoringenServiceMock : IStoringenService
    {
        public Task<List<Storing>> GetStoringen(string station)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pajoohesh_V2.Data.Initializer
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}

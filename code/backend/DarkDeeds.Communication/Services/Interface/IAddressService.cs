using System;

namespace DarkDeeds.Communication.Services.Interface
{
    interface IAddressService
    {
        bool TryGetAddress(out Uri uri);
    }
}
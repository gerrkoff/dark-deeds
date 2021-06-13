using System;

namespace DarkDeeds.Communication.Services.Interface
{
    internal interface IAddressService
    {
        bool TryGetAddress(out Uri uri);
    }
}
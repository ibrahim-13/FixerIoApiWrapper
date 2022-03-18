﻿using FixerIoApiWrapper;

var wrapper = new FixerApiWrapper("API_KEY_HERE", new()
{
#if DEBUG
    EnableApiResponseLogging = true,
#else
    EnableApiResponseLogging = false,
#endif
});
_ = await wrapper.GetSymbolsAsync();
Console.WriteLine("Hello, World!");
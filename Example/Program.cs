using FixerIoApiWrapper;

var wrapper = new FixerApiWrapper("API_KEY_HERE");
_ = await wrapper.GetSymbolsAsync();
Console.WriteLine("Hello, World!");
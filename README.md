#WygwamToolkit

Toolkit published by the [Wygwam](http://www.wygwam.com) team.

You can use the nuget package: [https://nuget.org/packages/Wygwam.Toolkit/](https://nuget.org/packages/Wygwam.Toolkit/)

#Summary

Provides some useful stuff for your developments on WinRT and Windows Phone.
You can use our Interfaces or Abstract Classes to get some tools like:

##AStorageManager

Implemented on Windows Phone 8 and WinRT (*StorageManager* classes)

Allows you to manage your file and settings in a easy way
```csharp
Task<bool> SaveSettingAsync(string key, object data)
Task<T> LoadSettingAsync<T>(string key)
Task<bool> ClearSettingAsync(string key)
Task<bool> SaveDataAsync(string path, object data)
Task<T> LoadDataAsync<T>(string path)
Task<bool> ClearDataAsync(string path)
```
##ILocationManager

Implemented on Windows Phone 8 and WinRT (*LocationManager* classes)

Allows you to manage geoposition in a easy way
```csharp
GeoPosition LastKnowPosition { get; set; }
Task<GeoPosition> GetLocationAsync();
Task<bool> AskForRightAsync();
```
##ANetworkManager

Implemented on Windows Phone 8 and WinRT (*NetworkManager* classes)

Allows you to know if network is available
```csharp
bool IsNetworkAvailable { get; }
```
